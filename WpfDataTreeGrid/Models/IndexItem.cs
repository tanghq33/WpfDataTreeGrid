using System;

namespace WpfDataTreeGrid.Models
{
    /// <summary>
    /// Represents an index item in the tree grid
    /// </summary>
    public class IndexItem : HierarchicalItem
    {
        private string _panelBarcode;
        private string _boardBarcode;

        /// <summary>
        /// Gets or sets the panel barcode
        /// </summary>
        public string PanelBarcode
        {
            get => _panelBarcode;
            set
            {
                if (_panelBarcode != value)
                {
                    _panelBarcode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the board barcode
        /// </summary>
        public string BoardBarcode
        {
            get => _boardBarcode;
            set
            {
                if (_boardBarcode != value)
                {
                    _boardBarcode = value;
                    OnPropertyChanged();
                }
            }
        }
    }
} 