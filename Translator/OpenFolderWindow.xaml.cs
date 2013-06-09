using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace Translator
{
    /// <summary>
    /// Interaction logic for OpenFolderWindow.xaml
    /// </summary>
    public partial class OpenFolderWindow : Window
    {
        public OpenFolderWindow()
        {
            InitializeComponent();
        }

        private string InitPath = "";

        public OpenFolderWindow(string path)
            : this()
        {
            InitPath = path;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void treeView1_Loaded(object sender, RoutedEventArgs e)
        {
            // Create main expanded node of TreeView
            treeView1.Items.Add(TreeView_CreateComputerItem());

            if (InitPath != "")
            {
                string xpath = "";
                string[] fullPath = InitPath.Split(System.IO.Path.DirectorySeparatorChar);
                fullPath[0] = fullPath[0] + "\\";
                TreeViewItem root = treeView1.Items[0] as TreeViewItem;
                root.IsExpanded = true;
                foreach (string path in fullPath)
                {
                    xpath = System.IO.Path.Combine(xpath, path);

                    foreach (TreeViewItem item in root.Items)
                    {
                        if ((string)item.Tag == xpath)
                        {
                            root = item;
                            root.IsExpanded = true;
                            break;
                        }
                    }

                    if (xpath == InitPath)
                        break;
                }
            }
        }

        TreeViewItem TreeView_CreateComputerItem()
        {
            string header = LocalString.Get("OpenFolderWindow_RootNode");

            TreeViewItem computer = new TreeViewItem { Header = header, IsExpanded = true };
            foreach (var drive in DriveInfo.GetDrives())
            {
                TreeViewItem driveItem = new TreeViewItem();
                if (drive.IsReady)
                {
                    driveItem.Header = String.Format("{0} ({1}:)", drive.VolumeLabel, drive.Name[0]);
                    if (Directory.GetDirectories(drive.Name).Length > 0)
                        driveItem.Items.Add(null);
                }
                else
                {
                    driveItem.Header = String.Format("{0} ({1}:)", drive.DriveType, drive.Name[0]);
                }
                driveItem.Tag = drive.Name;
                driveItem.Expanded += TreeViewItem_Expanded;
                driveItem.MouseDoubleClick += TreeViewitem_DoubleClick;
                driveItem.KeyDown += TreeViewitem_KeyDown;
                driveItem.Selected += TreeViewItem_Selected;
                computer.Items.Add(driveItem);
            }
            return computer;
        }

        void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem rootItem = (TreeViewItem)sender;
            rootItem.IsSelected = true;
            Path = (string)rootItem.Tag;

            if (rootItem.Items.Count == 1 && rootItem.Items[0] == null)
            {
                rootItem.Items.Clear();

                string[] dirs;
                try
                {
                    dirs = Directory.GetDirectories((string)rootItem.Tag);
                }
                catch
                {
                    return;
                }

                foreach (var dir in dirs)
                {
                    TreeViewItem subItem = new TreeViewItem();
                    subItem.Header = new DirectoryInfo(dir).Name;
                    subItem.Tag = dir;
                    try
                    {
                        if (Directory.GetDirectories(dir).Length > 0)
                            subItem.Items.Add(null);
                    }
                    catch { }
                    subItem.Expanded += TreeViewItem_Expanded;
                    subItem.MouseDoubleClick += TreeViewitem_DoubleClick;
                    subItem.KeyDown += TreeViewitem_KeyDown;
                    subItem.Selected += TreeViewItem_Selected;
                    rootItem.Items.Add(subItem);
                }
            }

            e.Handled = true;
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs routedEventArgs)
        {
            Path = (string)(sender as TreeViewItem).Tag;
            routedEventArgs.Handled = true;
        }

        private void TreeViewitem_KeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            Path = (string)(sender as TreeViewItem).Tag;
            keyEventArgs.Handled = true;
            DialogResult = true;
        }

        public string Path = "";

        private void TreeViewitem_DoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if ((sender as TreeViewItem).Items.Count == 0)
            {
                Path = (string) (sender as TreeViewItem).Tag;
                DialogResult = true;
            }
        }
    }
}
