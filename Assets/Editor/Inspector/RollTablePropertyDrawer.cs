﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace RollAnything
{
    [CustomPropertyDrawer(typeof(RollTable))]
    public class RollTablePropertyDrawer : PropertyDrawer
    {
        bool m_Initialized;

        [SerializeField]
        TreeViewState _treeViewState; // Serialized in the window layout file so it survives assembly reloading

        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;


        const float _toolBarHeight = 20f;

        const float _spacing = 2f;


        private Rect _propertyBaseRect, _toolbarRect, _searchBarRect, _treeviewRect, _bottomToolbarRect;
        private SerializedProperty activeProperty;
        private SerializedProperty rollEntryList;
        private RollTableModel tableModel;

        public RollTableModel TableModel
        {
            get
            {
                if (tableModel != null) return tableModel;
                tableModel = new RollTableModel(GetData());
                return tableModel;
            }
        }


        private SerializedProperty expandedRollTable = null;

        private SerializedProperty ExpandedRollTable
        {
            get
            {
                if (expandedRollTable != null) return expandedRollTable;
                expandedRollTable = activeProperty.FindPropertyRelative("expandedtable");
                return expandedRollTable;
            }
        }


        RollTableView rollTableTreeView;

        public RollTableView RollTableTreeView
        {
            get
            {
                if (rollTableTreeView != null) return rollTableTreeView;
                if (_treeViewState == null)
                    _treeViewState = new TreeViewState();

                bool firstInit = m_MultiColumnHeaderState == null;
                var headerState =
                    RollTableView.CreateDefaultMultiColumnHeaderState(Screen.width - 20);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new MyMultiColumnHeader(headerState);
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = TableModel;

                return rollTableTreeView = new RollTableView(_treeViewState, multiColumnHeader, treeModel);
            }
        }

        SearchField searchField;

        private SearchField _searchField
        {
            get
            {
                if (searchField != null) return searchField;
                searchField = new SearchField();
                searchField.downOrUpArrowKeyPressed += RollTableTreeView.SetFocusAndEnsureSelectedItem;
                return searchField;
            }
        }

        private List<RollEntry> data;

        List<RollEntry> GetData()
        {
            int arraySize = rollEntryList.arraySize;
            if (data != null && arraySize == data.Count) return data;

            activeProperty.serializedObject.Update();
            data = new List<RollEntry>();

            if (arraySize <= 0)
                data.Add(new RollEntry(null, "Root", -1, 0, 0));


            for (int i = 0; i < arraySize; i++)
            {
                var tableEntry = rollEntryList.GetArrayElementAtIndex(i);

                data.Add(PropertyToRollEntry(tableEntry));
            }

            rollEntryList.SetValueDirect(data);
            activeProperty.serializedObject.ApplyModifiedProperties();

            return data;
        }

        RollEntry PropertyToRollEntry(SerializedProperty tableEntryProperty)
        {
            Object rollObject = tableEntryProperty.FindPropertyRelative("MyObject").objectReferenceValue;
            string name = tableEntryProperty.FindPropertyRelative("m_Name").stringValue;
            int depth = tableEntryProperty.FindPropertyRelative("m_Depth").intValue;
            int id = tableEntryProperty.FindPropertyRelative("m_ID").intValue;
            int weight = tableEntryProperty.FindPropertyRelative("Weight").intValue;
//            int guaranteeBonus = tableEntryProperty.FindPropertyRelative("m_GuaranteeBonus").intValue;

            return new RollEntry(rollObject, name, depth, id, weight);
            ;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + _fullPropertyHeight;
        }


        private Rect[] RectsToConsider => new Rect[5]
        {
            _propertyBaseRect,
            _toolbarRect,
            _treeviewRect,
            _searchBarRect,
            _bottomToolbarRect
        };

        private float _fullPropertyHeight;

        private float CalculatePropertyheight
        {
            get
            {
                float rectHeight = 0;
                for (int i = 0; i < RectsToConsider.Length; i++)
                {
                    rectHeight += RectsToConsider[i].height;
                }

                return rectHeight;
            }
        }


        public override void OnGUI(Rect propertyRect, SerializedProperty property, GUIContent label)
        {
            activeProperty = property;
            rollEntryList = activeProperty.FindPropertyRelative("_rollEntries");

            _propertyBaseRect = new Rect(propertyRect.x, propertyRect.y, propertyRect.width, _toolBarHeight);

            ExpandedRollTable.boolValue =
                EditorGUI.Foldout(_propertyBaseRect, ExpandedRollTable.boolValue, label);


            if (expandedRollTable.boolValue)
            {
                using (new EditorGUI.IndentLevelScope(1))
                {
                    //EditorGUILayout.PropertyField();
                    _toolbarRect = DrawToolBarLayout(_propertyBaseRect);

                    _searchBarRect = DrawSearchBar(_toolbarRect);

                    _treeviewRect = DrawTreeView(_searchBarRect);

                    _bottomToolbarRect = BottomToolBarLayout(_treeviewRect);

                    _fullPropertyHeight = CalculatePropertyheight;
                }
            }

            else
            {
                _fullPropertyHeight = 0;
            }
        }

        Rect DivideRectHorizontal(Rect fullSizeRect, float scale = 3, float widthPosition = 0)
        {
            float scaledWidth = (fullSizeRect.width / scale) + 1;
            Vector2 pos = new Vector2(fullSizeRect.position.x + (scaledWidth * widthPosition), fullSizeRect.position.y);

            Vector2 area = new Vector2(scaledWidth, fullSizeRect.height);
            return new Rect(pos, area);
        }

        void AddItem(Object objectToAdd = null)
        {
            Undo.RecordObject(activeProperty.serializedObject.targetObject, "Add Item To Asset");

            // Add item as child of selection
            var selection = RollTableTreeView.GetSelection();
            TreeElement parent = (selection.Count == 1 ? RollTableTreeView.treeModel.Find(selection[0]) : null) ??
                                 RollTableTreeView.treeModel.root;
            int depth = parent != null ? parent.depth + 1 : 0;
            int id = RollTableTreeView.treeModel.GenerateUniqueID();
            var element = new RollEntry(objectToAdd, objectToAdd.name, depth = depth, id);
            RollTableTreeView.treeModel.AddElement(element, parent, 0);

            // Select newly created element
            RollTableTreeView.SetSelection(new[] {id}, TreeViewSelectionOptions.RevealAndFrame);
        }

        Rect DrawToolBarLayout(Rect previousRect)
        {
            int buttonDivision = 6;

            Rect toolBarRect = new Rect(previousRect.x, previousRect.y + previousRect.height, previousRect.width,
                _toolBarHeight);

            Rect testButtonRect = DivideRectHorizontal(toolBarRect, buttonDivision, 0);

            var style = "miniButton";
            if (GUI.Button(testButtonRect, "Test Roll"))
            {
                RollTableTreeView.TestRoll();
            }

            string lastRolledLabel = "NONE";
            if (RollTableTreeView.LastRolled != null)
                lastRolledLabel = RollTableTreeView.LastRolled.name;

            Rect rollLabelRect = DivideRectHorizontal(toolBarRect, buttonDivision, 1);
            GUI.Label(rollLabelRect, "Last Rolled: " + lastRolledLabel);

            //TODO Add Dragndrop area

//            Rect addItemRect = DivideRectHorizontal(toolBarRect, buttonDivision, 4);
//            if (GUI.Button(addItemRect, "Add Item", style))
//            {
//                AddItem();
//            }

            Rect removeItemRect = DivideRectHorizontal(toolBarRect, buttonDivision, 5);
            if (GUI.Button(removeItemRect, "Remove Item", style))
            {
                Undo.RecordObject(activeProperty.serializedObject.targetObject, "Remove Item From Asset");
                var selection = RollTableTreeView.GetSelection();
                RollTableTreeView.treeModel.RemoveElements(selection);
            }
            return toolBarRect;
        }

        Rect DrawSearchBar(Rect previousRect)
        {
            Rect searchBarRect = new Rect(previousRect.x, previousRect.y + previousRect.height, previousRect.width,
                _toolBarHeight);
            RollTableTreeView.searchString =
                _searchField.OnGUI(searchBarRect,
                    RollTableTreeView.searchString);
            return searchBarRect;
        }

        /// <summary>
        /// Tree View Drawer
        /// </summary>
        /// <param name="previousRect"></param>
        /// <returns></returns>
        Rect DrawTreeView(Rect previousRect)
        {
            Rect treeViewRect = new Rect(previousRect.x, previousRect.y + previousRect.height, previousRect.width,
                RollTableTreeView.totalHeight + 10 + 2 * _spacing);
            RollTableTreeView.OnGUI(treeViewRect);
            return treeViewRect;
        }

        Rect BottomToolBarLayout(Rect previousRect)
        {
            int buttonDivision = 10;
            Rect bottomToolbar = new Rect(previousRect.x, previousRect.y + previousRect.height, previousRect.width,
                _toolBarHeight);
            Rect expandButtonRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 0);
            var style = "miniButton";
            if (GUI.Button(expandButtonRect, "Expand All", style))
            {
                RollTableTreeView.ExpandAll();
            }
            Rect collapseButtonRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 1);
            if (GUI.Button(collapseButtonRect, "Collapse All", style))
            {
                RollTableTreeView.CollapseAll();
            }
            Rect propertyLabelRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 2);
            GUI.Label(propertyLabelRect, activeProperty.serializedObject.context != null
                ? AssetDatabase.GetAssetPath(activeProperty.serializedObject.context)
                : string.Empty);
            Rect setSortingRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 3);
            if (GUI.Button(setSortingRect, "Set sorting", style))
            {
                var myColumnHeader = (MyMultiColumnHeader) RollTableTreeView.multiColumnHeader;
                myColumnHeader.SetSortingColumns(new int[] {4, 3, 2}, new[] {true, false, true});
                myColumnHeader.mode = MyMultiColumnHeader.Mode.LargeHeader;
            }
            Rect headerRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 5);
            GUI.Label(headerRect, "Header: ", "minilabel");
            Rect largeButtonRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 6);
            if (GUI.Button(largeButtonRect, "Large", style))
            {
                var myColumnHeader = (MyMultiColumnHeader) RollTableTreeView.multiColumnHeader;
                myColumnHeader.mode = MyMultiColumnHeader.Mode.LargeHeader;
            }
            Rect mediumButtonRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 7);
            if (GUI.Button(mediumButtonRect, "Default", style))
            {
                var myColumnHeader = (MyMultiColumnHeader) RollTableTreeView.multiColumnHeader;
                myColumnHeader.mode = MyMultiColumnHeader.Mode.DefaultHeader;
            }
            Rect smallButtonRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 8);
            if (GUI.Button(smallButtonRect, "No sort", style))
            {
                var myColumnHeader = (MyMultiColumnHeader) RollTableTreeView.multiColumnHeader;
                myColumnHeader.mode = MyMultiColumnHeader.Mode.MinimumHeaderWithoutSorting;
            }
            Rect valueControlRect = DivideRectHorizontal(bottomToolbar, buttonDivision, 9);
            if (GUI.Button(valueControlRect, "values <-> controls", style))
            {
                RollTableTreeView.showControls = !RollTableTreeView.showControls;
            }
            return bottomToolbar;
        }
    }

    internal class MyMultiColumnHeader : MultiColumnHeader
    {
        Mode m_Mode;

        public enum Mode
        {
            LargeHeader,
            DefaultHeader,
            MinimumHeaderWithoutSorting
        }

        public MyMultiColumnHeader(MultiColumnHeaderState state)
            : base(state)
        {
            mode = Mode.DefaultHeader;
        }

        public Mode mode
        {
            get { return m_Mode; }
            set
            {
                m_Mode = value;
                switch (m_Mode)
                {
                    case Mode.LargeHeader:
                        canSort = true;
                        height = 37f;
                        break;
                    case Mode.DefaultHeader:
                        canSort = true;
                        height = DefaultGUI.defaultHeight;
                        break;
                    case Mode.MinimumHeaderWithoutSorting:
                        canSort = false;
                        height = DefaultGUI.minimumHeight;
                        break;
                }
            }
        }

        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
// Default column header gui
            base.ColumnHeaderGUI(column, headerRect, columnIndex);

// Add additional info for large header
            if (mode == Mode.LargeHeader)
            {
// Show example overlay stuff on some of the columns
                if (columnIndex > 2)
                {
                    headerRect.xMax -= 3f;
                    var oldAlignment = EditorStyles.largeLabel.alignment;
                    EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
                    GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
                    EditorStyles.largeLabel.alignment = oldAlignment;
                }
            }
        }
    }
}