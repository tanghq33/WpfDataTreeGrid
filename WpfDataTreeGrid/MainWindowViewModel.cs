using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace WpfDataTreeGrid
{

    public partial class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<Index> Indexes { get; set; } = new();

        private TreeGridModel _treeGridModel = new TreeGridModel();

        [ObservableProperty]
        private TreeGridFlatModel _flatIndexes = new TreeGridFlatModel();
        public MainWindowViewModel()
        {
            var panelIndex1 = new Index
            {
                PanelBarcode = "Panel1",
            };

            var boardIndex1 = new Index
            {
                BoardBarcode = "Board1"
            };

            var boardIndex2 = new Index
            {
                BoardBarcode = "Board2"
            };

            panelIndex1.Children.Add(boardIndex1);
            panelIndex1.Children.Add(boardIndex2);

            _treeGridModel.Add(panelIndex1);

            var panelIndex2 = new Index
            {
                PanelBarcode = "Panel2",
            };

            _treeGridModel.Add(panelIndex2);

            FlatIndexes = _treeGridModel.FlatModel;
        }
    }
}
