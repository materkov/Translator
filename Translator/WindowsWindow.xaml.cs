using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Translator
{
    /// <summary>
    /// Interaction logic for WindowsWindow.xaml
    /// </summary>
    public partial class WindowsWindow : Window
    {
        private CodeEditorView _codeEditor;

        public WindowsWindow(IEnumerable<CodeFile> files, CodeEditorView codeEditor)
        {
            _codeEditor = codeEditor;
            Files = files;

            InitializeComponent();
        }

        public IEnumerable<CodeFile> Files { get; set; }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем все выделенные
            while (listView.SelectedItems.Count > 0)
                _codeEditor.RemoveFromTabs(listView.SelectedItem as CodeFile);
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            DoActivate();
        }

        private void Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoActivate();
        }

        private void DoActivate()
        {
            CodeFile file = listView.SelectedItem as CodeFile;
            if (file != null)
            {
                _codeEditor.OpenTab(file, true);
                DialogResult = true;
            }
        }

        private void CloseAllButton_Click(object sender, RoutedEventArgs e)
        {
            _codeEditor.CloseAllTabs();
        }

        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActivateButton.IsEnabled = (listView.SelectedItems.Count == 1);
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            // Установить фокус на открытый в данный момент элемент в listView
            listView.UpdateLayout();
            ListViewItem item = listView.ItemContainerGenerator.ContainerFromItem(_codeEditor.CurrentCodeFile) as ListViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                item.Focus();
            }
        }

        private void Item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                DoActivate();
            }
        }
    }
}
