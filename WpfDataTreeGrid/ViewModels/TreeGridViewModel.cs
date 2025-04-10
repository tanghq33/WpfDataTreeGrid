using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfDataTreeGrid.Infrastructure;
using WpfDataTreeGrid.Models;

namespace WpfDataTreeGrid.ViewModels
{
    /// <summary>
    /// ViewModel for managing a hierarchical tree grid collection
    /// </summary>
    /// <typeparam name="T">The type of items in the tree grid (must implement IHierarchicalItem)</typeparam>
    public class TreeGridViewModel<T> : ObservableObject where T : class, IHierarchicalItem
    {
        private readonly TreeGridManager _treeGridManager;

        /// <summary>
        /// Creates a new instance of the TreeGridViewModel class
        /// </summary>
        public TreeGridViewModel()
        {
            _treeGridManager = new TreeGridManager();
        }

        /// <summary>
        /// Creates a new instance of the TreeGridViewModel class with the specified items
        /// </summary>
        /// <param name="items">The items to add to the tree grid</param>
        public TreeGridViewModel(IEnumerable<T> items) : this()
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    AddItem(item);
                }
            }
        }

        /// <summary>
        /// Gets the flattened items in the tree grid
        /// </summary>
        public ObservableCollection<IHierarchicalItem> FlattenedItems => _treeGridManager.FlattenedItems;

        /// <summary>
        /// Gets the root items in the tree grid
        /// </summary>
        public ObservableCollection<IHierarchicalItem> RootItems => _treeGridManager.RootItems;

        /// <summary>
        /// Adds an item to the tree grid
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddItem(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _treeGridManager.AddRootItem(item);
        }

        /// <summary>
        /// Removes an item from the tree grid
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was removed, false otherwise</returns>
        public bool RemoveItem(T item)
        {
            if (item == null)
            {
                return false;
            }

            return _treeGridManager.RemoveRootItem(item);
        }

        /// <summary>
        /// Clears all items from the tree grid
        /// </summary>
        public void Clear()
        {
            _treeGridManager.Clear();
        }

        /// <summary>
        /// Rebuilds the flattened view from the root items
        /// </summary>
        public void RebuildFlattenedView()
        {
            _treeGridManager.RebuildFlattenedView();
        }
    }

    /// <summary>
    /// Non-generic version of TreeGridViewModel for use in XAML
    /// </summary>
    public class TreeGridViewModel : TreeGridViewModel<HierarchicalItem>
    {
    }
} 