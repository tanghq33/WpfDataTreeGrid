using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using WpfDataTreeGrid.Models;

namespace WpfDataTreeGrid.Infrastructure
{
    /// <summary>
    /// A collection that maintains a flattened view of a hierarchical tree structure
    /// </summary>
    public class FlattenedTreeCollection : ObservableCollection<IHierarchicalItem>
    {
        private readonly Dictionary<IHierarchicalItem, bool> _itemRegistry = new();
        private bool _internalUpdate;

        /// <summary>
        /// Creates a new instance of FlattenedTreeCollection
        /// </summary>
        public FlattenedTreeCollection()
        {
        }

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item">The item to add</param>
        public new void Add(IHierarchicalItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            RegisterItem(item);
            base.Add(item);
            
            // If the item is expanded, add all its visible children
            if (item.IsExpanded)
            {
                AddExpandedChildren(item);
            }

            // Subscribe to item expansion changes
            SubscribeToItemEvents(item);
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>True if the item was found and removed, false otherwise</returns>
        public new bool Remove(IHierarchicalItem item)
        {
            if (item == null || !Contains(item))
            {
                return false;
            }

            // First remove all children if the item is expanded
            if (item.IsExpanded)
            {
                RemoveExpandedChildren(item);
            }

            // Unsubscribe from item events
            UnsubscribeFromItemEvents(item);
            
            // Unregister the item
            UnregisterItem(item);
            
            // Remove the item itself
            return base.Remove(item);
        }

        /// <summary>
        /// Clears all items from the collection
        /// </summary>
        public new void Clear()
        {
            // Unsubscribe from all items
            foreach (var item in this)
            {
                UnsubscribeFromItemEvents(item);
            }
            
            // Clear the registry
            _itemRegistry.Clear();
            
            // Clear the collection
            base.Clear();
        }

        /// <summary>
        /// Rebuild the flat collection from a set of root items
        /// </summary>
        /// <param name="rootItems">The root items to use</param>
        public void RebuildFrom(IEnumerable<IHierarchicalItem> rootItems)
        {
            // Clear the collection first
            Clear();

            // Add all root items
            if (rootItems == null)
            {
                return;
            }

            _internalUpdate = true;
            foreach (var item in rootItems)
            {
                Add(item);
            }
            _internalUpdate = false;
        }

        /// <summary>
        /// Called when an item is expanded or collapsed
        /// </summary>
        /// <param name="sender">The item that was expanded or collapsed</param>
        private void OnItemExpandedChanged(object sender, EventArgs e)
        {
            var item = (IHierarchicalItem)sender;
            Debug.WriteLine($">>> FlattenedTreeCollection.OnItemExpandedChanged: Item '{item}' IsExpanded = {item.IsExpanded}");

            if (item.IsExpanded)
            {
                // Item was expanded - add its children
                Debug.WriteLine($"    -> Calling AddExpandedChildren for '{item}'");
                AddExpandedChildren(item);
            }
            else
            {
                // Item was collapsed - remove its children
                Debug.WriteLine($"    -> Calling RemoveExpandedChildren for '{item}'");
                RemoveExpandedChildren(item);
            }
        }

        /// <summary>
        /// Add all the visible children of an expanded item
        /// </summary>
        /// <param name="item">The expanded item</param>
        private void AddExpandedChildren(IHierarchicalItem item)
        {
            Debug.WriteLine($"    +++ FlattenedTreeCollection.AddExpandedChildren: Adding children for '{item}'");

            // Skip if we don't have the item in our collection
            if (!_itemRegistry.ContainsKey(item))
            {
                Debug.WriteLine($"        Item '{item}' not in registry. Skipping.");
                return;
            }

            var itemIndex = IndexOf(item);
            if (itemIndex < 0)
            {
                Debug.WriteLine($"        Item '{item}' not found in collection (Index={itemIndex}). Skipping.");
                return;
            }
            var insertionIndex = itemIndex + 1;
            Debug.WriteLine($"        Parent Item '{item}' found at index {itemIndex}. Initial insertion index: {insertionIndex}");

            _internalUpdate = true;

            // Add all immediate children at the insertion point
            int childCount = 0;
            foreach (var child in item.Children)
            {
                childCount++;
                // Register and subscribe to the child
                RegisterItem(child);
                SubscribeToItemEvents(child);

                // Insert the child
                Debug.WriteLine($"        Inserting child '{child}' at index {insertionIndex}");
                InsertItem(insertionIndex++, child);

                // If the child is also expanded, add its children recursively
                if (child.IsExpanded)
                {
                    Debug.WriteLine($"        Child '{child}' is also expanded. Recursively adding its children.");
                    AddExpandedChildren(child);

                    // Recalculate the insertion index AFTER recursive call
                    int currentChildIndex = IndexOf(child);
                    if (currentChildIndex < 0) // Child might not have been added if it had no children itself yet
                    {
                         Debug.WriteLine($"        WARNING: Child '{child}' not found after recursive AddExpandedChildren. This might be okay if it had no expanded children.");
                         // Attempt to find the last added item related to the original parent to continue insertion
                         // This is complex, let's rely on Reset notification for now.
                    }
                    else
                    {
                        insertionIndex = currentChildIndex + 1; // Next item goes after the child and its expanded descendants
                        Debug.WriteLine($"            Child '{child}' and its descendants added. New insertion index: {insertionIndex}");
                    }
                }
            }
            Debug.WriteLine($"    +++ Finished adding {childCount} direct children for '{item}'.");

            _internalUpdate = false;
            // Notify the UI that the collection has changed significantly
            Debug.WriteLine($"    +++ Raising CollectionChanged Reset after adding children for '{item}'.");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Remove all the children of a collapsed item
        /// </summary>
        /// <param name="item">The collapsed item</param>
        private void RemoveExpandedChildren(IHierarchicalItem item)
        {
            Debug.WriteLine($"    --- FlattenedTreeCollection.RemoveExpandedChildren: Removing children for '{item}'");

            // Skip if we don't have the item in our collection
            if (!_itemRegistry.ContainsKey(item))
            {
                 Debug.WriteLine($"        Item '{item}' not in registry. Skipping.");
                return;
            }

            _internalUpdate = true;

            // Get all visible children (direct and indirect)
            var visibleChildren = GetAllVisibleChildren(item).ToList();
            Debug.WriteLine($"        Found {visibleChildren.Count} visible children to remove.");

            // Remove all visible children
            int removedCount = 0;
            foreach (var child in visibleChildren)
            {
                Debug.WriteLine($"        Removing child '{child}'");
                // Unsubscribe from the child
                UnsubscribeFromItemEvents(child);

                // Unregister the child
                UnregisterItem(child);

                // Remove the child
                if (Remove(child))
                {
                    removedCount++;
                }
                else
                {
                    Debug.WriteLine($"        WARNING: Failed to remove child '{child}' from base collection.");
                }
            }
             Debug.WriteLine($"    --- Finished removing {removedCount} children for '{item}'.");

            _internalUpdate = false;
            // Notify the UI that the collection has changed significantly
            Debug.WriteLine($"    --- Raising CollectionChanged Reset after removing children for '{item}'.");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Get all visible children of an item (direct and indirect)
        /// </summary>
        /// <param name="item">The parent item</param>
        /// <returns>All visible children, including nested children if their parents are expanded</returns>
        private IEnumerable<IHierarchicalItem> GetAllVisibleChildren(IHierarchicalItem item)
        {
            var visibleChildren = new List<IHierarchicalItem>();
            
            foreach (var child in item.Children)
            {
                // Add the immediate child
                visibleChildren.Add(child);
                
                // If the child is expanded, add its children recursively
                if (child.IsExpanded)
                {
                    visibleChildren.AddRange(GetAllVisibleChildren(child));
                }
            }
            
            return visibleChildren;
        }

        /// <summary>
        /// Register an item in the internal registry
        /// </summary>
        /// <param name="item">The item to register</param>
        private void RegisterItem(IHierarchicalItem item)
        {
            _itemRegistry[item] = true;
        }

        /// <summary>
        /// Unregister an item from the internal registry
        /// </summary>
        /// <param name="item">The item to unregister</param>
        private void UnregisterItem(IHierarchicalItem item)
        {
            _itemRegistry.Remove(item);
        }

        /// <summary>
        /// Subscribe to the PropertyChanged event of an item to detect expansion changes
        /// </summary>
        /// <param name="item">The item to subscribe to</param>
        private void SubscribeToItemEvents(IHierarchicalItem item)
        {
            // Subscribe to the item's property changes (for IsExpanded)
            item.PropertyChanged += Item_PropertyChanged;
        }

        /// <summary>
        /// Unsubscribe from the PropertyChanged event of an item
        /// </summary>
        /// <param name="item">The item to unsubscribe from</param>
        private void UnsubscribeFromItemEvents(IHierarchicalItem item)
        {
            // Unsubscribe from the item's property changes
            item.PropertyChanged -= Item_PropertyChanged;
        }

        /// <summary>
        /// Called when a property on an item changes
        /// </summary>
        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Handle the IsExpanded property change
            if (e.PropertyName == nameof(IHierarchicalItem.IsExpanded))
            {
                OnItemExpandedChanged(sender, EventArgs.Empty);
            }
        }

        // Override InsertItem and RemoveItem to bypass OnCollectionChanged when _internalUpdate is true
        protected override void InsertItem(int index, IHierarchicalItem item)
        {
            if (!_internalUpdate)
            {
                 Debug.WriteLine($">>> FlattenedTreeCollection.InsertItem: Index={index}, Item='{item}', InternalUpdate={_internalUpdate}");
                 base.InsertItem(index, item);
            }
            else
            {
                Debug.WriteLine($"    FlattenedTreeCollection.InsertItem (Suppressed): Index={index}, Item='{item}'");
                CheckReentrancy();
                Items.Insert(index, item);
                // Do not call base.InsertItem to prevent notification during internal update
                // We'll notify with Reset later.
            }
        }

        protected override void RemoveItem(int index)
        {
             if (!_internalUpdate)
            {
                IHierarchicalItem item = this[index];
                Debug.WriteLine($">>> FlattenedTreeCollection.RemoveItem: Index={index}, Item='{item}', InternalUpdate={_internalUpdate}");
                base.RemoveItem(index);
            }
            else
            {
                IHierarchicalItem item = this[index];
                Debug.WriteLine($"    FlattenedTreeCollection.RemoveItem (Suppressed): Index={index}, Item='{item}'");
                CheckReentrancy();
                Items.RemoveAt(index);
                // Do not call base.RemoveItem to prevent notification during internal update
                // We'll notify with Reset later.
            }
        }

        protected override void ClearItems()
        {
             if (!_internalUpdate)
            {
                Debug.WriteLine($">>> FlattenedTreeCollection.ClearItems: InternalUpdate={_internalUpdate}");
                base.ClearItems();
            }
            else
            {
                Debug.WriteLine($"    FlattenedTreeCollection.ClearItems (Suppressed)");
                CheckReentrancy();
                Items.Clear();
                // Do not call base.ClearItems to prevent notification during internal update
                // We'll notify with Reset later.
            }
        }

        // OnCollectionChanged is now only called manually or when _internalUpdate is false
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Block reentrancy before raising the event
            using (BlockReentrancy())
            {
                 Debug.WriteLine($"### FlattenedTreeCollection.OnCollectionChanged: Raising event Action={e.Action}");
                 if (e.NewItems != null) Debug.WriteLine($"    NewItems: {string.Join(", ", e.NewItems.Cast<object>())}");
                 if (e.OldItems != null) Debug.WriteLine($"    OldItems: {string.Join(", ", e.OldItems.Cast<object>())}");
                 base.OnCollectionChanged(e);
            }
        }
    }
} 