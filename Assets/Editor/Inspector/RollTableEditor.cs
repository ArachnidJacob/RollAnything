using System;
using System.Collections;
using System.Collections.Generic;
using RollAnything;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace RollAnything
{
    [CustomEditor(typeof(RollTableAsset))]
    public class RollTableEditor : Editor
    {
//        [NonSerialized] bool m_Initialized;
//
//        [SerializeField]
//        TreeViewState _treeViewState; // Serialized in the window layout file so it survives assembly reloading
//
//        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
//
//        RollTableAsset mRollTableAsset;
//
//        private RollTableAsset _mRollTableAsset
//        {
//            get
//            {
//                if (mRollTableAsset != null) return mRollTableAsset;
//                mRollTableAsset = (RollTableAsset) target;
//                if (!m_Initialized)
//                {
//                    m_Initialized = true;
//                }
//                return mRollTableAsset;
//            }
//        }
//
//
//        const float _toolBarHeight = 20f;
//
//        const float _spacing = 2f;
//
//        private Rect _treeViewRect
//        {
//            get { return GUILayoutUtility.GetRect(0, 10000, 0, RollTreeView.totalHeight + 10 + 2 * _spacing); }
//        }
//
//
//        RollTableView treeView;
//
//        public RollTableView RollTreeView
//        {
//            get
//            {
//                if (treeView != null) return treeView;
//                if (_treeViewState == null)
//                    _treeViewState = new TreeViewState();
//
//                bool firstInit = m_MultiColumnHeaderState == null;
//                var headerState =
//                    RollTableView.CreateDefaultMultiColumnHeaderState(Screen.width - 20);
//                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
//                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
//                m_MultiColumnHeaderState = headerState;
//
//                var multiColumnHeader = new MyMultiColumnHeader(headerState);
//                if (firstInit)
//                    multiColumnHeader.ResizeToFit();
//
//                var treeModel = _mRollTableAsset.TableModel;
//
//                return treeView = new RollTableView(_treeViewState, multiColumnHeader, treeModel);
//            }
//        }
//
//        SearchField searchField;
//
//        private SearchField _searchField
//        {
//            get
//            {
//                if (searchField != null) return searchField;
//                searchField = new SearchField();
//                searchField.downOrUpArrowKeyPressed += treeView.SetFocusAndEnsureSelectedItem;
//                return searchField;
//            }
//        }
//
//
//        List<RollEntry> GetData()
//        {
//            if (_mRollTableAsset != null && _mRollTableAsset.RollEntries != null)
//                return _mRollTableAsset.RollEntries;
//            return _mRollTableAsset.RollEntries = new List<RollEntry>();
//        }
//
//
//        public override void OnInspectorGUI()
//        {
//            DrawToolBarLayout();
//
//            DrawSearchBar();
//
//            DrawTreeView();
//
//            BottomToolBarLayout();
//        }
//
//
//        void DrawToolBarLayout()
//        {
//            using (new EditorGUILayout.HorizontalScope())
//            {
//                var style = "miniButton";
//                if (GUILayout.Button("Test Roll"))
//                {
//                    RollTreeView.TestRoll();
//                }
//                string lastRolledLabel = "NONE";
//
//                if (RollTreeView.LastRolled != null)
//                    lastRolledLabel = RollTreeView.LastRolled.name;
//                GUILayout.Label("Last Rolled: " + lastRolledLabel);
//
//                GUILayout.FlexibleSpace();
//
//                if (GUILayout.Button("Add Item", style))
//                {
//                    Undo.RecordObject(_mRollTableAsset, "Add Item To Asset");
//
//                    // Add item as child of selection
//                    var selection = RollTreeView.GetSelection();
//                    TreeElement parent = (selection.Count == 1 ? RollTreeView.treeModel.Find(selection[0]) : null) ??
//                                         RollTreeView.treeModel.root;
//                    int depth = parent != null ? parent.depth + 1 : 0;
//                    int id = RollTreeView.treeModel.GenerateUniqueID();
//                    var element = new RollEntry("Item " + id, depth, id);
//                    RollTreeView.treeModel.AddElement(element, parent, 0);
//
//                    // Select newly created element
//                    RollTreeView.SetSelection(new[] {id}, TreeViewSelectionOptions.RevealAndFrame);
//                }
//
//                if (GUILayout.Button("Remove Item", style))
//                {
//                    Undo.RecordObject(_mRollTableAsset, "Remove Item From Asset");
//                    var selection = RollTreeView.GetSelection();
//                    RollTreeView.treeModel.RemoveElements(selection);
//                }
//            }
//        }
//
//        void DrawSearchBar()
//        {
//            using (new EditorGUILayout.HorizontalScope())
//            {
//                RollTreeView.searchString =
//                    _searchField.OnGUI(GUILayoutUtility.GetRect(0, 10000, 0, _toolBarHeight),
//                        RollTreeView.searchString);
//            }
//        }
//
//        /// <summary>
//        /// Tree View Drawer
//        /// </summary>
//        /// <param name="previousRect"></param>
//        /// <returns></returns>
//        void DrawTreeView()
//        {
//            using (new EditorGUILayout.HorizontalScope())
//            {
//                RollTreeView.OnGUI(_treeViewRect);
//            }
//        }
//
//        void BottomToolBarLayout()
//        {
//            using (new EditorGUILayout.HorizontalScope())
//            {
//                var style = "miniButton";
//                if (GUILayout.Button("Expand All", style))
//                {
//                    RollTreeView.ExpandAll();
//                }
//
//                if (GUILayout.Button("Collapse All", style))
//                {
//                    RollTreeView.CollapseAll();
//                }
//
//                GUILayout.FlexibleSpace();
//
//                GUILayout.Label(_mRollTableAsset != null ? AssetDatabase.GetAssetPath(_mRollTableAsset) : string.Empty);
//
//                GUILayout.FlexibleSpace();
//
//                if (GUILayout.Button("Set sorting", style))
//                {
//                    var myColumnHeader = (MyMultiColumnHeader) RollTreeView.multiColumnHeader;
//                    myColumnHeader.SetSortingColumns(new int[] {4, 3, 2}, new[] {true, false, true});
//                    myColumnHeader.mode = MyMultiColumnHeader.Mode.LargeHeader;
//                }
//
//
//                GUILayout.Label("Header: ", "minilabel");
//                if (GUILayout.Button("Large", style))
//                {
//                    var myColumnHeader = (MyMultiColumnHeader) RollTreeView.multiColumnHeader;
//                    myColumnHeader.mode = MyMultiColumnHeader.Mode.LargeHeader;
//                }
//                if (GUILayout.Button("Default", style))
//                {
//                    var myColumnHeader = (MyMultiColumnHeader) RollTreeView.multiColumnHeader;
//                    myColumnHeader.mode = MyMultiColumnHeader.Mode.DefaultHeader;
//                }
//                if (GUILayout.Button("No sort", style))
//                {
//                    var myColumnHeader = (MyMultiColumnHeader) RollTreeView.multiColumnHeader;
//                    myColumnHeader.mode = MyMultiColumnHeader.Mode.MinimumHeaderWithoutSorting;
//                }
//
//                GUILayout.Space(10);
//
//                if (GUILayout.Button("values <-> controls", style))
//                {
//                    RollTreeView.showControls = !RollTreeView.showControls;
//                }
//            }
//        }
//    }
//
//
//    internal class MyMultiColumnHeader : MultiColumnHeader
//    {
//        Mode m_Mode;
//
//        public enum Mode
//        {
//            LargeHeader,
//            DefaultHeader,
//            MinimumHeaderWithoutSorting
//        }
//
//        public MyMultiColumnHeader(MultiColumnHeaderState state)
//            : base(state)
//        {
//            mode = Mode.DefaultHeader;
//        }
//
//        public Mode mode
//        {
//            get { return m_Mode; }
//            set
//            {
//                m_Mode = value;
//                switch (m_Mode)
//                {
//                    case Mode.LargeHeader:
//                        canSort = true;
//                        height = 37f;
//                        break;
//                    case Mode.DefaultHeader:
//                        canSort = true;
//                        height = DefaultGUI.defaultHeight;
//                        break;
//                    case Mode.MinimumHeaderWithoutSorting:
//                        canSort = false;
//                        height = DefaultGUI.minimumHeight;
//                        break;
//                }
//            }
//        }
//
//        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
//        {
//            // Default column header gui
//            base.ColumnHeaderGUI(column, headerRect, columnIndex);
//
//            // Add additional info for large header
//            if (mode == Mode.LargeHeader)
//            {
//                // Show example overlay stuff on some of the columns
//                if (columnIndex > 2)
//                {
//                    headerRect.xMax -= 3f;
//                    var oldAlignment = EditorStyles.largeLabel.alignment;
//                    EditorStyles.largeLabel.alignment = TextAnchor.UpperRight;
//                    GUI.Label(headerRect, 36 + columnIndex + "%", EditorStyles.largeLabel);
//                    EditorStyles.largeLabel.alignment = oldAlignment;
//                }
//            }
//        }
//    }
    }
}