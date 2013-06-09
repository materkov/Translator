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
    /// Interaction logic for NewProject.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public NewProjectWindow()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            var window = new OpenFolderWindow(comboBox2.Text) { Owner = this };
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                comboBox2.Text = window.Path;
            }
        }

        private int NextProjectNumber;

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists(GetDirectory()))
            {
                MessageBoxResult result = MessageBox.Show(LocalString.Get("NewProjectWindow_DirExists"), 
                    LocalString.Get("NewProjectWindow_Attention"),
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    IDEState.Get().NextProjectNumber = NextProjectNumber;
                    DialogResult = true;
                }
                else
                {
                    // Ничего, возврат в предыдущую форму
                }
            }
            else
            {
                IDEState.Get().NextProjectNumber = NextProjectNumber;
                DialogResult = true;
            }
        }

        private const string DefaultProjectName = "MyProject";

        private void GenerateName()
        {
            string name = DefaultProjectName + (NextProjectNumber++);
            string dir;
            if (comboBox2.SelectedIndex == -1)
                dir = comboBox2.Text;
            else
                dir = comboBox2.SelectedValue as string;

            string path = System.IO.Path.Combine(dir, name);
            while (System.IO.Directory.Exists(path))
            {
                name = DefaultProjectName + (NextProjectNumber++);
                path = System.IO.Path.Combine(comboBox2.SelectedValue as string, name);
            }

            textBox1.Text = name;
        }

        private string GetDirectory()
        {
            string name = textBox1.Text;
            string dir;
            if (comboBox2.SelectedIndex == -1)
                dir = comboBox2.Text;
            else
                dir = comboBox2.SelectedValue as string;

            return System.IO.Path.Combine(dir, name);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NextProjectNumber = IDEState.Get().NextProjectNumber;
            GenerateName();
        }
    }
}
