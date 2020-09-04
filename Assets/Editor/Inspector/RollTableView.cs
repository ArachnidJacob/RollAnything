using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RollAnything;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Object = UnityEngine.Object;

public class RollTableView : TreeViewWithTreeModel<RollEntry>
{
    public RollTableView(TreeViewState state, RollTableModel model) : base(state, model)
    {
        showBorder = true;
        showAlternatingRowBackgrounds = true;
        Init(model);
    }


    const float kRowHeights = 20f;
    const float kToggleWidth = 18f;
    public bool showControls = true;

    static Texture2D[] s_TestIcons =
    {
        EditorGUIUtility.FindTexture("Folder Icon"),
        EditorGUIUtility.FindTexture("GameObject Icon")
    };

    // All columns
    enum MyColumns
    {
        Icon,
        Name,
        Object,
        Weight,
        DropChance,
        //GuaranteeBonus,
    }

    public enum SortOption
    {
        Icon,
        Name,
        Object,
        Weight,
        DropChance,
        // GuaranteeBonus,
    }

    // Sort options per column
    SortOption[] m_SortOptions =
    {
        SortOption.Icon,
        SortOption.Name,
        SortOption.Object,
        SortOption.Weight,
        SortOption.DropChance,
        // SortOption.GuaranteeBonus
    };

    protected void Init(RollTableModel model)
    {
        base.Init(model);
        //treeChanged += RecalcDropChance;
    }

    private RollTableModel TableModel => (RollTableModel) m_TreeModel;


    public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
    {
        if (root == null)
            throw new NullReferenceException("root");
        if (result == null)
            throw new NullReferenceException("result");

        result.Clear();

        if (root.children == null)
            return;

        Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
        for (int i = root.children.Count - 1; i >= 0; i--)
            stack.Push(root.children[i]);

        while (stack.Count > 0)
        {
            TreeViewItem current = stack.Pop();
            result.Add(current);

            if (current.hasChildren && current.children[0] != null)
            {
                for (int i = current.children.Count - 1; i >= 0; i--)
                {
                    stack.Push(current.children[i]);
                }
            }
        }
    }

    public RollTableView(TreeViewState state, MultiColumnHeader multicolumnHeader, RollTableModel model)
        : base(state, multicolumnHeader, model)
    {
        Assert.AreEqual(m_SortOptions.Length, Enum.GetValues(typeof(MyColumns)).Length,
            "Ensure number of sort options are in sync with number of MyColumns enum values");

        // Custom setup
        rowHeight = kRowHeights;
        columnIndexForTreeFoldouts = 1;
        showAlternatingRowBackgrounds = true;
        showBorder = true;
        customFoldoutYOffset =
            (kRowHeights - EditorGUIUtility.singleLineHeight) *
            0.5f; // center foldout in the row since we also center content. See RowGUI
        extraSpaceBeforeIconAndLabel = kToggleWidth;
        multicolumnHeader.sortingChanged += OnSortingChanged;

        Reload();
    }


    // Note we We only build the visible rows, only the backend has the full tree information. 
    // The treeview only creates info for the row list.
    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
        var rows = base.BuildRows(root);
        SortIfNeeded(root, rows);
        return rows;
    }

    public void TestRoll()
    {
        SetSelection(new[] {TableModel.TestRoll().id}, TreeViewSelectionOptions.RevealAndFrame);
    }

    public RollEntry LastRolled
    {
        get { return TableModel.LastRolled; }
    }

//    TreeViewItem<RollEntry> GetItemFromEntry(RollEntry rollentry)
//         {
//        var viewContainingEntry =
//            GetRows().First(rollItem => (TreeViewItem<RollEntry>)rollItem.);
//        return (TreeViewItem<RollEntry>) viewContainingEntry;
//    }

    void OnSortingChanged(MultiColumnHeader multiColumnHeader)
    {
        SortIfNeeded(rootItem, GetRows());
    }

    void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
    {
        if (rows.Count <= 1)
            return;

        if (multiColumnHeader.sortedColumnIndex == -1)
        {
            return; // No column to sort for (just use the order the data are in)
        }

        // Sort the roots of the existing tree items
        SortByMultipleColumns();
        TreeToList(root, rows);
        Repaint();
    }

    void SortByMultipleColumns()
    {
        var sortedColumns = multiColumnHeader.state.sortedColumns;

        if (sortedColumns.Length == 0)
            return;

        var myTypes = rootItem.children.Cast<TreeViewItem<RollEntry>>();
        var orderedQuery = InitialOrder(myTypes, sortedColumns);
        for (int i = 1; i < sortedColumns.Length; i++)
        {
            SortOption sortOption = m_SortOptions[sortedColumns[i]];
            bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

            switch (sortOption)
            {
                case SortOption.Name:
                    orderedQuery = orderedQuery.ThenBy(l => l.data.name, ascending);
                    break;
                case SortOption.Weight:
                    orderedQuery = orderedQuery.ThenBy(l => l.data.weight, ascending);
                    break;
                case SortOption.DropChance:
                    orderedQuery = orderedQuery.ThenBy(l => l.data.localDropChance, ascending);
                    break;
//                case SortOption.GuaranteeBonus:
//                    orderedQuery = orderedQuery.ThenBy(l => l.data.m_GuaranteeBonus, ascending);
//                    break;
            }
        }

        rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
    }

    IOrderedEnumerable<TreeViewItem<RollEntry>> InitialOrder(IEnumerable<TreeViewItem<RollEntry>> myTypes,
        int[] history)
    {
        SortOption sortOption = m_SortOptions[history[0]];
        bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
        switch (sortOption)
        {
            case SortOption.Name:
                return myTypes.Order(l => l.data.name, ascending);
            case SortOption.Weight:
                return myTypes.Order(l => l.data.weight, ascending);
            case SortOption.DropChance:
                return myTypes.Order(l => l.data.localDropChance, ascending);
//            case SortOption.GuaranteeBonus:
//                return myTypes.Order(l => l.data.m_GuaranteeBonus, ascending);
            default:
                Assert.IsTrue(false, "Unhandled enum");
                break;
        }

        // default
        return myTypes.Order(l => l.data.name, ascending);
    }


    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (TreeViewItem<RollEntry>) args.item;

        for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
        {
            CellGUI(args.GetCellRect(i), item, (MyColumns) args.GetColumn(i), ref args);
        }
    }

    void CellGUI(Rect cellRect, TreeViewItem<RollEntry> item, MyColumns column, ref RowGUIArgs args)
    {
        // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
        CenterRectUsingSingleLineHeight(ref cellRect);

        switch (column)
        {
            case MyColumns.Icon:
            {
                GUI.DrawTexture(cellRect, s_TestIcons[0], ScaleMode.ScaleToFit);
            }
                break;

            case MyColumns.Name:
            {
                // Do toggle
                Rect toggleRect = cellRect;
                toggleRect.x += GetContentIndent(item);
                toggleRect.width = kToggleWidth;
//                if (toggleRect.xMax < cellRect.xMax)
//                    item.data.enabled = EditorGUI.Toggle(toggleRect, item.data.enabled); // hide when outside cell rect

                // Default icon and label
                args.rowRect = cellRect;
                base.RowGUI(args);
            }
                break;
            case MyColumns.Object:
            case MyColumns.Weight:
            case MyColumns.DropChance:

            {
                if (showControls)
                {
                    using (var check = new EditorGUI.ChangeCheckScope())
                    {
                        cellRect.xMin += 5f; // When showing controls make some extra spacing
                        if (column == MyColumns.Object)
                        {
                            item.data.myObject = EditorGUI.ObjectField(cellRect, item.data.myObject,
                                typeof(UnityEngine.Object), true);
                        }
                        if (column == MyColumns.Weight)
                        {
                            item.data.weight =
                                EditorGUI.IntField(cellRect, item.data.weight);
                        }
                        //TODO DropChance Direct edit : half reduces entire tree proportionally, half increases m_Weight proportionately
                        if (column == MyColumns.DropChance)
                        {
                            EditorGUI.LabelField(cellRect,
                                item.data.localDropChance.ToString());
                        }

                        if (check.changed)
                        {
                            TableModel.UpdateModel();
                        }
                    }
//                    if (column == MyColumns.GuaranteeBonus)
//                        item.data.m_GuaranteeBonus = EditorGUI.IntField(cellRect, item.data.m_GuaranteeBonus);
                }
                else
                {
                    string value = "Missing";
                    if (column == MyColumns.Object)
                    {
                        if (item.data != null && item.data.myObject != null)
                            value = item.data.myObject.ToString();
                    }

                    if (column == MyColumns.Weight)
                        value = item.data.weight.ToString("f5");
                    if (column == MyColumns.DropChance)
                        value = item.data.localDropChance.ToString("f5");
//                    if (column == MyColumns.GuaranteeBonus)
//                        value = item.data.m_GuaranteeBonus.ToString("f5");

                    DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                }
            }
                break;
        }
    }


    // Rename
    //--------

    protected override bool CanRename(TreeViewItem item)
    {
        // Only allow rename if we can show the rename overlay with a certain width (label might be clipped by other columns)
        Rect renameRect = GetRenameRect(treeViewRect, 0, item);
        return renameRect.width > 30;
    }

    protected override void RenameEnded(RenameEndedArgs args)
    {
        // Set the backend name and reload the tree to reflect the new model
        if (args.acceptedRename)
        {
            var element = treeModel.Find(args.itemID);
            element.name = args.newName;
            Reload();
        }
    }

    protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
    {
        Rect cellRect = GetCellRectForTreeFoldouts(rowRect);
        CenterRectUsingSingleLineHeight(ref cellRect);
        return base.GetRenameRect(cellRect, row, item);
    }

    // Misc
    //--------

    protected override bool CanMultiSelect(TreeViewItem item)
    {
        return true;
    }

    const string k_ObjectDragID = "ObjectDragging";


    protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
    {
        Debug.LogFormat("Id's: \n {0}", args.draggedItemIDs);
        if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
        {
            DragAndDrop.SetGenericData(k_ObjectDragID, DragAndDrop.objectReferences);
        }
        else
        {
            base.SetupDragAndDrop(args);
        }
    }

    protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
    {
        Object[] draggedObjects = DragAndDrop.objectReferences;
        Debug.LogFormat("pos: {0}\nindex: {1}\nparent: {2}\ndrop:{3}\nobjs: {4}",
            args.dragAndDropPosition, args.insertAtIndex, args.parentItem, args.performDrop, draggedObjects);

        if (draggedObjects != null && draggedObjects.Length > 0)
        {
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                {
                    // bool validDrag = ValidDrag(args.parentItem, entryObjects);
                    if (args.performDrop)
                    {
                        var parentData = ((TreeViewItem<RollEntry>) args.parentItem).data;
                        if (draggedObjects.Length > 1)
                            TableModel.AddObjectsToTree(draggedObjects, parentData, args.insertAtIndex);
                        else
                            TableModel.AddObjectToTree(draggedObjects.First(), parentData, args.insertAtIndex);
                    }
                    return DragAndDropVisualMode.Move; //: DragAndDropVisualMode.None;
                }

                case DragAndDropPosition.OutsideItems:
                {
                    if (args.performDrop)
                    {
                        var parentData = m_TreeModel.root;
                        if (draggedObjects.Length > 1)
                            TableModel.AddObjectsToTree(draggedObjects, parentData, 0);
                        else
                            TableModel.AddObjectToTree(draggedObjects.First(), parentData, 0);
                    }
                    return DragAndDropVisualMode.Move;
                }
                default:
                    Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
                    return DragAndDropVisualMode.None;
            }
        }

        return base.HandleDragAndDrop(args);
    }


    public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
    {
        var columns = new[]
        {
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel"),
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. "),
                contextMenuText = "Asset",
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Right,
                width = 30,
                minWidth = 30,
                maxWidth = 60,
                autoResize = false,
                allowToggleVisibility = true
            },

            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Name"),
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 150,
                minWidth = 60,
                autoResize = false,
                allowToggleVisibility = false
            },
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Object",
                    "The Unity Object this roll points at."),
                contextMenuText = "Type",
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Right,
                width = 100,
                minWidth = 50,
                maxWidth = 110,
                autoResize = false,
                allowToggleVisibility = true
            },
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Weight", "Flat Rolling Weight"),
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 100,
                minWidth = 50,
                autoResize = true
            },
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("DropChance", "Calculated Drop Chance in context of this RollTable"),
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 100,
                minWidth = 50,
                autoResize = true,
                allowToggleVisibility = true
            },
//            new MultiColumnHeaderState.Column
//            {
//                headerContent =
//                    new GUIContent("GuaranteeBonus",
//                        "Added Weight per missed roll that gets added to this item for pseudo randomness."),
//                headerTextAlignment = TextAlignment.Center,
//                sortedAscending = true,
//                sortingArrowAlignment = TextAlignment.Left,
//                width = 120,
//                minWidth = 60,
//                autoResize = true
//            }
        };

        Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length,
            "Number of columns should match number of enum values: You probably forgot to update one of them.");

        var state = new MultiColumnHeaderState(columns);
        return state;
    }
}

static class MyExtensionMethods
{
    public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector,
        bool ascending)
    {
        if (ascending)
        {
            return source.OrderBy(selector);
        }
        else
        {
            return source.OrderByDescending(selector);
        }
    }

    public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector,
        bool ascending)
    {
        if (ascending)
        {
            return source.ThenBy(selector);
        }
        else
        {
            return source.ThenByDescending(selector);
        }
    }
}