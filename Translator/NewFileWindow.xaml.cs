using System;
using System.Collections.Generic;
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
    /// Interaction logic for NewFileWindow.xaml
    /// </summary>
    public partial class NewFileWindow : Window
    {
        public NewFileWindow()
        {
            WorkDirs = new List<string>();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public List<string> WorkDirs { get; private set; }

        private void window1_Loaded(object sender, RoutedEventArgs e)
        {
            if (CodeProjectManager.Instance.Current != null)
                foreach (string dir in CodeProjectManager.Instance.Current.WorkDirs)
                {
                    // TODO Сокращение
                    WorkDirs.Add(dir);
                }

            LocalString v = App.Current.Resources["LocalString"] as LocalString;
            
            WorkDirs.Add(v.Dict["AddNewFileWindow_AddWorkDir"]);
        }
    }
}
