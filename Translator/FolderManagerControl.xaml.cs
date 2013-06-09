using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Translator
{
    /// <summary>
    /// Interaction logic for FolderManagerControl.xaml
    /// </summary>
    public partial class FolderManagerControl : UserControl
    {
        public FolderManagerControl()
        {
            InitializeComponent();
        }

        public void Init(ObservableCollection<string> bindingCollection, Window parent)
        {
            BindingCollection = bindingCollection;
            _parent = parent;

            listView1.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
        }

        public ObservableCollection<string> BindingCollection { get; private set; }
        private Window _parent;

        private void listView1_Loaded(object sender, RoutedEventArgs e)
        {
            if (listView1.Items.Count <= 1)
                DelButton.IsEnabled = false;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listView1.SelectedIndex;
            if (selectedIndex == -1)
                return;

            string selectedItem = (string)listView1.SelectedItem;

            int newIndex = selectedIndex - 1;
            if (newIndex < 0) newIndex = 0;


            BindingCollection.Remove(selectedItem);
            BindingCollection.Insert(newIndex, selectedItem);
            listView1.SelectedIndex = newIndex;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listView1.SelectedIndex;
            if (selectedIndex == -1)
                return;

            string selectedItem = (string)listView1.SelectedItem;

            int newIndex = selectedIndex + 1;
            if (newIndex >= listView1.Items.Count) newIndex = listView1.Items.Count - 1;

            BindingCollection.Remove(selectedItem);
            BindingCollection.Insert(newIndex, selectedItem);
            listView1.SelectedIndex = newIndex;
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            // Add new folder
            var window = new OpenFolderWindow() { Owner = _parent };
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                BindingCollection.Add(window.Path);
            }

            if (listView1.Items.Count <= 1)
                DelButton.IsEnabled = false;
            else
                DelButton.IsEnabled = true;
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listView1.SelectedIndex;
            if (selectedIndex == -1)
                return;

            if (listView1.Items.Count <= 1)
                return;

            string selectedItem = (string)listView1.SelectedItem;

            BindingCollection.Remove(selectedItem);

            if (selectedIndex >= listView1.Items.Count) selectedIndex = listView1.Items.Count - 1;
            listView1.SelectedIndex = selectedIndex;

            if (listView1.Items.Count <= 1)
                DelButton.IsEnabled = false;
            else
                DelButton.IsEnabled = true;
        }
    }
}
