using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WpfDataTreeGrid.Models
{
    /// <summary>
    /// Base implementation of IHierarchicalItem that can be used as a foundation for custom hierarchical data types
    /// </summary>
    public class HierarchicalItem : IHierarchicalItem
    {
        private bool _isExpanded;
        private int _level;
        private IHierarchicalItem _parent;
        private readonly ObservableCollection<IHierarchicalItem> _children = new ObservableCollection<IHierarchicalItem>();

        /// <summary>
        /// Event raised when a property on this object changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new instance of the HierarchicalItem class
        /// </summary>
        public HierarchicalItem()
        {
            _level = 0;
        }

        /// <summary>
        /// Gets whether this item has any children
        /// </summary>
        public bool HasChildren => _children.Count > 0;

        /// <summary>
        /// Gets or sets whether this item is expanded
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            { 
                if (_isExpanded != value)
                {
                    Debug.WriteLine($"* HierarchicalItem.IsExpanded.set: Item '{this}' changing from {_isExpanded} to {value}");
                    _isExpanded = value;
                    OnPropertyChanged();
                    
                    // Call the appropriate event when expanded/collapsed
                    if (_isExpanded)
                    {
                        Debug.WriteLine($"    -> Calling OnExpanded for '{this}'");
                        OnExpanded();
                    }
                    else
                    {
                        Debug.WriteLine($"    -> Calling OnCollapsed for '{this}'");
                        OnCollapsed();
                    }
                }
                else
                {
                    Debug.WriteLine($"* HierarchicalItem.IsExpanded.set: Item '{this}' value already {value}. No change.");
                }
            }
        }

        /// <summary>
        /// Gets the level of this item in the hierarchy (0 for root items)
        /// </summary>
        public int Level
        {
            get => _level;
            private set
            {
                if (_level != value)
                {
                    _level = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the collection of child items
        /// </summary>
        public IEnumerable<IHierarchicalItem> Children => _children;

        /// <summary>
        /// Gets the parent item, or null if this is a root item
        /// </summary>
        public IHierarchicalItem Parent => _parent;

        /// <summary>
        /// Set the parent of this item
        /// </summary>
        /// <param name="parent">The parent item</param>
        public void SetParent(IHierarchicalItem parent)
        {
            _parent = parent;
            UpdateLevel();
        }

        /// <summary>
        /// Add a child item to this item
        /// </summary>
        /// <param name="child">The child item to add</param>
        public void AddChild(IHierarchicalItem child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child), "The child item cannot be null.");
            }

            _children.Add(child);
            child.SetParent(this);
            OnPropertyChanged(nameof(HasChildren));
            OnChildAdded(child);
        }

        /// <summary>
        /// Remove a child item from this item
        /// </summary>
        /// <param name="child">The child item to remove</param>
        /// <returns>True if the item was removed, false otherwise</returns>
        public bool RemoveChild(IHierarchicalItem child)
        {
            if (child == null)
            {
                return false;
            }

            bool removed = _children.Remove(child);
            if (removed)
            {
                child.SetParent(null);
                OnPropertyChanged(nameof(HasChildren));
                OnChildRemoved(child);
            }
            return removed;
        }

        /// <summary>
        /// Clear all children from this item
        /// </summary>
        public void ClearChildren()
        {
            foreach (var child in _children)
            {
                child.SetParent(null);
            }
            _children.Clear();
            OnPropertyChanged(nameof(HasChildren));
            OnChildrenCleared();
        }

        /// <summary>
        /// Called when the item is expanded
        /// </summary>
        protected virtual void OnExpanded()
        {
            // Can be overridden in derived classes
        }

        /// <summary>
        /// Called when the item is collapsed
        /// </summary>
        protected virtual void OnCollapsed()
        {
            // Can be overridden in derived classes
        }

        /// <summary>
        /// Called when a child is added to this item
        /// </summary>
        /// <param name="child">The child that was added</param>
        protected virtual void OnChildAdded(IHierarchicalItem child)
        {
            // Can be overridden in derived classes
        }

        /// <summary>
        /// Called when a child is removed from this item
        /// </summary>
        /// <param name="child">The child that was removed</param>
        protected virtual void OnChildRemoved(IHierarchicalItem child)
        {
            // Can be overridden in derived classes
        }

        /// <summary>
        /// Called when all children are cleared from this item
        /// </summary>
        protected virtual void OnChildrenCleared()
        {
            // Can be overridden in derived classes
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.WriteLine($"** HierarchicalItem.OnPropertyChanged: Item '{this}' Property='{propertyName}'");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Update the level of this item and all of its children
        /// </summary>
        private void UpdateLevel()
        {
            Level = (_parent != null) ? _parent.Level + 1 : 0;

            foreach (var child in _children)
            {
                child.SetParent(this); // This will trigger UpdateLevel on the child
            }
        }
    }
} 