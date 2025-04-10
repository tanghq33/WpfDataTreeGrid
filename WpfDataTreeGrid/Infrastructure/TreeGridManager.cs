using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WpfDataTreeGrid.Models;

namespace WpfDataTreeGrid.Infrastructure
{
    /// <summary>
    /// Manager class that handles tree grid operations
    /// </summary>
    public class TreeGridManager
    {
        /// <summary>
        /// The flattened view of the hierarchical data
        /// </summary>
        public FlattenedTreeCollection FlattenedItems { get; } = new FlattenedTreeCollection();

        /// <summary>
        /// The root items in the tree structure
        /// </summary>
        public ObservableCollection<IHierarchicalItem> RootItems { get; } = new ObservableCollection<IHierarchicalItem>();

        /// <summary>
        /// Initializes a new instance of the TreeGridManager class
        /// </summary>
        public TreeGridManager()
        {
            // Subscribe to changes in the root items collection
            RootItems.CollectionChanged += RootItems_CollectionChanged;
        }

        /// <summary>
        /// Adds a root item to the tree
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddRootItem(IHierarchicalItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            RootItems.Add(item);
        }

        /// <summary>
        /// Removes a root item from the tree
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was removed, false otherwise</returns>
        public bool RemoveRootItem(IHierarchicalItem item)
        {
            if (item == null)
            {
                return false;
            }

            return RootItems.Remove(item);
        }

        /// <summary>
        /// Clears all items from the tree
        /// </summary>
        public void Clear()
        {
            RootItems.Clear();
            FlattenedItems.Clear();
        }

        /// <summary>
        /// Rebuilds the flattened view from the root items
        /// </summary>
        public void RebuildFlattenedView()
        {
            FlattenedItems.RebuildFrom(RootItems);
        }

        /// <summary>
        /// Handles changes to the root items collection
        /// </summary>
        private void RootItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // Add new items to the flattened view
                    foreach (IHierarchicalItem item in e.NewItems)
                    {
                        FlattenedItems.Add(item);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Remove items from the flattened view
                    foreach (IHierarchicalItem item in e.OldItems)
                    {
                        FlattenedItems.Remove(item);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // Clear the flattened view
                    FlattenedItems.Clear();
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Replace items in the flattened view
                    foreach (IHierarchicalItem item in e.OldItems)
                    {
                        FlattenedItems.Remove(item);
                    }
                    foreach (IHierarchicalItem item in e.NewItems)
                    {
                        FlattenedItems.Add(item);
                    }
                    break;
            }
        }
    }
} 