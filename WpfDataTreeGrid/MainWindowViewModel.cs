using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WpfDataTreeGrid.Models;
using WpfDataTreeGrid.ViewModels;

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
    }
}
