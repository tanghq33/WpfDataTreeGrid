# WpfDataTreeGrid Sample

This project demonstrates a custom `TreeDataGrid` control for WPF that displays hierarchical data in a grid format with indentation for child items.

## Features

*   Displays hierarchical data similar to a TreeView but within a DataGrid.
*   Automatic indentation for child items based on their level in the hierarchy.
*   Customizable indentation size.
*   Uses a helper `TreeGridViewModel<T>` to manage the hierarchical data and provide a flattened list suitable for the `DataGrid`.

## How to Use `TreeDataGrid`

1.  **Define the Data Model:**
    Your data items must implement the `IHierarchicalItem` interface. This interface requires properties to manage the hierarchy:
    ```csharp
    // From WpfDataTreeGrid/Models/IHierarchicalItem.cs (Implicitly used)
    public interface IHierarchicalItem
    {
        int Level { get; } // Depth in the tree
        bool IsExpanded { get; set; } // Controls node expansion state
        IHierarchicalItem Parent { get; } // Reference to the parent item
        ObservableCollection<IHierarchicalItem> Children { get; } // Child items
        bool HasChildren { get; } // Convenience property
        // Add your specific data properties here (e.g., PanelBarcode, BoardBarcode)
    }
    ```
    The `IndexItem` class in the sample likely implements this interface.

2.  **Use `TreeGridViewModel<T>`:**
    This view model helper manages the hierarchical data collection and provides a flattened list (`FlattenedItems`) needed by the `DataGrid`. It handles adding items, managing parent/child relationships, and expanding/collapsing nodes.

    ```csharp
    // In your ViewModel (e.g., MainWindowViewModel.cs)
    using WpfDataTreeGrid.ViewModels;
    using WpfDataTreeGrid.Models; // Assuming IndexItem is here
    using CommunityToolkit.Mvvm.ComponentModel;
    using System.Collections.ObjectModel;

    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private TreeGridViewModel<IndexItem> _treeGridViewModel;

        public MainWindowViewModel()
        {
            _treeGridViewModel = new TreeGridViewModel<IndexItem>();
            // ... populate the model
            CreateSampleData();
        }

        private void CreateSampleData()
        {
            var panelIndex1 = new IndexItem { PanelBarcode = "Panel1" };
            var boardIndex1 = new IndexItem { BoardBarcode = "Board1" };
            var boardIndex2 = new IndexItem { BoardBarcode = "Board2" };

            panelIndex1.AddChild(boardIndex1); // AddChild should update Level and Parent internally
            panelIndex1.AddChild(boardIndex2);

            TreeGridViewModel.AddItem(panelIndex1); // Add top-level item

            var panelIndex2 = new IndexItem { PanelBarcode = "Panel2" };
            TreeGridViewModel.AddItem(panelIndex2);
        }
    }
    ```

3.  **Declare `TreeDataGrid` in XAML:**
    Use the `TreeDataGrid` control in your XAML and bind its `ItemsSource` to the `FlattenedItems` property of your `TreeGridViewModel` instance.

    ```xml
    <Window ...
        xmlns:controls="clr-namespace:WpfDataTreeGrid.Controls"
        ...>
        <Window.DataContext>
            <!-- Set your ViewModel instance -->
        </Window.DataContext>

        <Grid>
            <controls:TreeDataGrid ItemsSource="{Binding TreeGridViewModel.FlattenedItems}"
                                   IndentSize="20"> <!-- Optional: Customize indent size -->
                <controls:TreeDataGrid.Columns>
                    <!-- Expander/Indent Column (Crucial!) -->
                    <DataGridTemplateColumn Width="Auto"
                                            CellStyle="{StaticResource ExpanderCellStyle}"
                                            CellTemplate="{StaticResource ExpanderColumnTemplate}"
                                            HeaderStyle="{StaticResource ExpanderHeaderStyle}" />

                    <!-- Your Data Columns -->
                    <DataGridTemplateColumn Header="Panel Barcode" Width="*" IsReadOnly="True">
                         <DataGridTemplateColumn.CellTemplate>
                            <DataTemplate>
                                <TextBlock Text="{Binding PanelBarcode}" VerticalAlignment="Center" HorizontalAlignment="Center"/>
                            </DataTemplate>
                        </DataGridTemplateColumn.CellTemplate>
                    </DataGridTemplateColumn>

                    <DataGridTemplateColumn Header="Board Barcode" Width="*" IsReadOnly="True">
                         <DataGridTemplateColumn.CellTemplate>
                            <DataTemplate>
                                <TextBlock Text="{Binding BoardBarcode}" VerticalAlignment="Center" HorizontalAlignment="Center"/>
                            </DataTemplate>
                        </DataGridTemplateColumn.CellTemplate>
                    </DataGridTemplateColumn>
                    <!-- Add other columns as needed -->

                </controls:TreeDataGrid.Columns>
            </controls:TreeDataGrid>
        </Grid>
    </Window>
    ```

4.  **Define Expander Column Template:**
    The `TreeDataGrid` relies on a specific template for the *first* column to handle indentation and the expand/collapse button. This template **must** contain a `FrameworkElement` named `PART_FirstCell`. The indentation margin is applied to this element. You also need styles for the cell and header.

    *   Merge the `TreeGridTemplates.xaml` resource dictionary (or create your own based on it).
    *   Reference the `ExpanderColumnTemplate`, `ExpanderCellStyle`, and `ExpanderHeaderStyle` static resources in your first `DataGridTemplateColumn`.

    ```xml
    <!-- In your Window/App Resources -->
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="/WpfDataTreeGrid;component/Controls/TreeGridTemplates.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>

    <!-- In TreeDataGrid Column Definitions -->
    <DataGridTemplateColumn Width="Auto" <!-- Or fixed width -->
                            CellStyle="{StaticResource ExpanderCellStyle}"
                            CellTemplate="{StaticResource ExpanderColumnTemplate}"
                            HeaderStyle="{StaticResource ExpanderHeaderStyle}" />
    ```
    *(See `Controls/TreeGridTemplates.xaml` for the actual template definitions, including the Expander button and the `PART_FirstCell` element, likely a Grid or Border)*.

5.  **Define Data Columns:**
    Add standard `DataGridTextColumn` or `DataGridTemplateColumn` definitions for the rest of your data properties, binding them as usual.

## Key Components

*   **`TreeDataGrid.cs`**: The custom `DataGrid` control implementing indentation logic via the `LoadingRow` event and the `IndentSize` property. It requires `PART_FirstCell` in the first column's template.
*   **`IHierarchicalItem.cs`**: Interface that data items must implement.
*   **`TreeGridViewModel.cs`**: Manages the tree data and provides the flattened `ItemsSource`.
*   **`TreeGridTemplates.xaml`**: Contains necessary `Style`s and `DataTemplate`s for the expander column, including the crucial `PART_FirstCell`.
*   **`MainWindow.xaml` / `MainWindowViewModel.cs`**: Example usage.

## Dependencies

*   .NET SDK (Version depends on project file)
*   WPF
*   CommunityToolkit.Mvvm (Used in the sample ViewModel) 