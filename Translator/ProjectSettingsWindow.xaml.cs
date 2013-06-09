using System;
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
    /// Interaction logic for ProjectSettingsWindow.xaml
    /// </summary>
    public partial class ProjectSettingsWindow : Window
    {
        public ProjectSettingsWindow()
        {
            Current = CodeProjectManager.Instance.Current;

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Update binding values
            textBox1.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textBox2.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            DialogResult = true;
        }

        public CodeProject Current { get; private set; }

        private void userControl1_Loaded(object sender, RoutedEventArgs e)
        {
            folderManagerControl.Init(Current.WorkDirs, this);
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox2.GetBindingExpression(TextBox.TextProperty).ValidateWithoutUpdate();
        }
    }

    public class InvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((bool) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
