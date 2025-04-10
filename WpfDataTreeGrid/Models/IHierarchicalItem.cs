using System.Collections.Generic;
using System.ComponentModel;

namespace WpfDataTreeGrid.Models
{
    /// <summary>
    /// Interface for items that can be displayed in a hierarchical tree structure
    /// </summary>
    public interface IHierarchicalItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets whether this item has any children
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Gets or sets whether this item is expanded
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets the level of this item in the hierarchy (0 for root items)
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Gets the collection of child items
        /// </summary>
        IEnumerable<IHierarchicalItem> Children { get; }

        /// <summary>
        /// Gets the parent item, or null if this is a root item
        /// </summary>
        IHierarchicalItem Parent { get; }

        /// <summary>
        /// Set the parent of this item
        /// </summary>
        /// <param name="parent">The parent item</param>
        void SetParent(IHierarchicalItem parent);

        /// <summary>
        /// Add a child item to this item
        /// </summary>
        /// <param name="child">The child item to add</param>
        void AddChild(IHierarchicalItem child);

        /// <summary>
        /// Remove a child item from this item
        /// </summary>
        /// <param name="child">The child item to remove</param>
        /// <returns>True if the item was removed, false otherwise</returns>
        bool RemoveChild(IHierarchicalItem child);

        /// <summary>
        /// Clear all children from this item
        /// </summary>
        void ClearChildren();
    }
} 