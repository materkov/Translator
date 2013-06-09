using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ContentsControl.xaml
    /// </summary>
    public partial class ContentsControl : UserControl, INotifyPropertyChanged
    {
        public ContentsControl()
        {
            InitializeComponent();
        }
        
        public void Init(CodeEditorView codeEditor)
        {
            _codeEditor = codeEditor;
            MainObject = treeView;
        }

        private FrameworkElement _lastFocused;
        public FrameworkElement MainObject
        {
            get { return _lastFocused; }
            set
            {
                _lastFocused = value;
                OnPropertyChanged("MainObject");
            }
        }

        private CodeEditorView _codeEditor;
        private ObservableCollection<Node> _structure = new ObservableCollection<Node>();

        public ObservableCollection<Node> Structure
        {
            get { return _structure; }
            set
            {
                _structure = value;
                OnPropertyChanged("Structure");
            }
        }

        private int _errors = 0;
        public int Errors
        {
            get { return _errors; }
            set
            {
                _errors = value;
                OnPropertyChanged("Errors");
            }
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (_codeEditor == null) return;    // ДЛЯ ДИЗАЙНЕРА, ИНАЧЕ ОН ПАДАЕТ

            RebuildContents();
            _codeEditor.CurrentFileChanged += (o, args) => RebuildContents();
        }

        private void RebuildContents()
        {
            if (_codeEditor == null) return;    // ДЛЯ ДИЗАЙНЕРА, ИНАЧЕ ОН ПАДАЕТ

            Structure.Clear();

            string text = (_codeEditor.CurrentCodeFile != null) ? _codeEditor.CurrentCodeFile.Text : "";
            char[] separators = { '\n', '\r' };
            string[] lines = text.Split(separators);

            Node parent = null;
            int counter = 0;

            foreach (string line in lines)
            {
                Match m = Regex.Match(line, @"^\s*func (?<la>[a-zA-Z0-9]*)\s*$");
                if (m.Success && m.Index == 0)
                {
                    //Group d = m.Groups[0];
                    int idx = line.IndexOf("func ") + 5;
                    int offset = counter + idx;

                    string name = "";
                    while (idx < line.Length && Char.IsLetterOrDigit(line[idx]))
                    {
                        name += line[idx];
                        idx++;
                    }

                    Node newNode = new Node() { Name = name, Parent = parent, Offset = offset };
                    if (parent != null)
                    {
                        if (parent.Childrens == null)
                            parent.Childrens = new List<Node>();

                        parent.Childrens.Add(newNode);
                    }
                    parent = newNode;
                }

                Match m2 = Regex.Match(line, @"^\s*endfunc\s*$");
                if (m2.Success && m2.Index == 0)
                {
                    if (parent == null)
                    {
                        Structure.Clear();
                        Errors = 1;
                        MainObject = treeView;
                        return;
                    }

                    if (parent.Parent == null)
                        Structure.Add(parent);

                    parent = parent.Parent;
                }

                counter += line.Length + 1;
            }

            if (parent != null)
            {
                Errors = 2;
                Structure.Clear();
                MainObject = treeView;
                return;
            }

            Errors = 0;

            if (treeView.Items.Count > 0)
            {
                // Про новом перестроении, если есть элемент, устанавливаем на первый
                TreeViewItem item = (TreeViewItem) (treeView.ItemContainerGenerator.ContainerFromIndex(0));
                MainObject = item;
            }
            else
                // Иначе просто фокусироваться будем на самом treeView
                MainObject = treeView;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Node n = (sender as TreeViewItem).Header as Node;
            if (n != null)
                _codeEditor.SetCaretTo(_codeEditor.CurrentCodeFile, n.Offset);

            e.Handled = true;
        }

        private void Item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Node n = (sender as TreeViewItem).Header as Node;
                if (n != null)
                    _codeEditor.SetCaretTo(_codeEditor.CurrentCodeFile, n.Offset);

                e.Handled = true;
            }
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            if (sender != null || !e.Handled)
            {
                MainObject = sender as FrameworkElement;
                e.Handled = true;   // Чтоб событие не поднималось по иерархии вверх
            }
        }

        private void FreeSpace_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!MainObject.IsFocused)
                MainObject.Focus();
        }
    }

    public class Node
    {
        public string Name { get; set; }
        public int Offset { get; set; }
        public List<Node> Childrens { get; set; }
        public Node Parent { get; set; }
    }
}
