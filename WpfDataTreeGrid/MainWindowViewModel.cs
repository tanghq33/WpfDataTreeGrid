using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WpfDataTreeGrid.Models;
using WpfDataTreeGrid.ViewModels;
using System.Windows.Input; // Added for ICommand
using CommunityToolkit.Mvvm.Input; // Added for RelayCommand
using System.Diagnostics; // Added for Debug.WriteLine
using WpfDataTreeGrid.Infrastructure; // Corrected namespace
using System.Linq; // Added for Linq query

namespace WpfDataTreeGrid
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private TreeGridViewModel<IndexItem> _treeGridViewModel;

        public MainWindowViewModel()
        {
            // Initialize the tree grid view model
            _treeGridViewModel = new TreeGridViewModel<IndexItem>();

            // Create sample data
            CreateSampleData();
        }

        private void CreateSampleData()
        {
            // Create sample panel and board items
            var panelIndex1 = new IndexItem
            {
                PanelBarcode = "Panel1"
            };

            var boardIndex1 = new IndexItem
            {
                BoardBarcode = "Board1"
            };

            var boardIndex2 = new IndexItem
            {
                BoardBarcode = "Board2"
            };

            // Add boards to panel
            panelIndex1.AddChild(boardIndex1);
            panelIndex1.AddChild(boardIndex2);

            // Add panel to tree grid
            TreeGridViewModel.AddItem(panelIndex1);

            // Create another panel
            var panelIndex2 = new IndexItem
            {
                PanelBarcode = "Panel2"
            };

            // Add second panel to tree grid
            TreeGridViewModel.AddItem(panelIndex2);
        }

        [RelayCommand]
        private void RowDoubleClick(object? item)
        {
            if (item is IHierarchicalItem hierarchicalItem)
            {
                // Check if the item exists in the root collection
                bool isRootItem = TreeGridViewModel.RootItems.Contains(hierarchicalItem);

                if (isRootItem)
                {
                    Debug.WriteLine($"Double-clicked on Root Item (Parent): {hierarchicalItem}");
                }
                else
                {
                    // To find the parent, we might need to traverse up, but for now just identify as child
                    Debug.WriteLine($"Double-clicked on Child Item: {hierarchicalItem}");
                }
                // Add your command logic here based on whether it's a root or child
            }
            else
            {
                Debug.WriteLine($"Double-clicked on unknown item type: {item?.GetType().Name ?? "null"}");
            }
        }
    }
}
