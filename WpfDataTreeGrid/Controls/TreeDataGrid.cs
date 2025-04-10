using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WpfDataTreeGrid.Models;

namespace WpfDataTreeGrid.Controls
{
    public class TreeDataGrid : DataGrid
    {
        /// <summary>
        /// Property for controlling the indentation size for each level in the tree
        /// </summary>
        public static readonly DependencyProperty IndentSizeProperty =
            DependencyProperty.Register(
                "IndentSize",
                typeof(double),
                typeof(TreeDataGrid),
                new PropertyMetadata(16.0));

        /// <summary>
        /// Gets or sets the indentation size for each level in the tree
        /// </summary>
        public double IndentSize
        {
            get { return (double)GetValue(IndentSizeProperty); }
            set { SetValue(IndentSizeProperty, value); }
        }

        public TreeDataGrid()
        {
            // Handle row loading to apply indentation
            LoadingRow += TreeDataGrid_LoadingRow;
        }

        private void TreeDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Check if the item is a HierarchicalItem
            if (e.Row.DataContext is IHierarchicalItem item)
            {
                // Apply indentation based on the level
                ApplyIndentation(e.Row, item.Level);
            }
        }

        private void ApplyIndentation(DataGridRow row, int level)
        {
            // Apply indentation based on the level
            if (level > 0 && row.FindName("PART_FirstCell") is FrameworkElement firstCell)
            {
                // If a cell named "PART_FirstCell" is found, apply indentation
                firstCell.Margin = new Thickness(level * IndentSize, 0, 0, 0);
            }
        }
    }
} 