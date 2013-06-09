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
    /// Interaction logic for VersionSelectWindow.xaml
    /// </summary>
    public partial class VersionSelectWindow : Window
    {
        public VersionSelectWindow(CodeFile file)
        {
            File = file;
            InitializeComponent();
        }

        public CodeFile File { get; private set; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //nameTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            versionView.DataContext = File.Versions;
        }

        private void versionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pair = versionView.SelectedItem as KeyValuePair<DateTime, string>?;
            if (pair.HasValue)
            {
                //richTextBox1.Document = null;
                string file = pair.Value.Value;
                FlowDocument document = new FlowDocument() { PageWidth = 1000 };
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(file));
                document.Blocks.Add(paragraph);
                richTextBox1.Document = document;
            }
            else
            {
                richTextBox1.Document = new FlowDocument();
            }

            CompareButton.IsEnabled = (versionView.SelectedItems.Count == 2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<DateTime, string> pair in versionView.SelectedItems)
            {
                File.RemoveVersion(pair.Key);
            }

            /*while (versionView.SelectedItems.Count > 0)
            {
                var pair = versionView.SelectedItem as KeyValuePair<DateTime, string>?;
                File.RemoveVersion(pair.Value.Key);
            }*/

            // Обновить список, иначе никак похоже :(
            versionView.DataContext = null;
            versionView.DataContext = File.Versions;
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            var pair1 = versionView.SelectedItems[0] as KeyValuePair<DateTime, string>?;
            var pair2 = versionView.SelectedItems[1] as KeyValuePair<DateTime, string>?;

            VersionCompareWindow window = new VersionCompareWindow { Owner = this };
            
            window.key1.Text = pair1.Value.Key.ToString();
            window.text1.Text = pair1.Value.Value;

            window.key2.Text = pair2.Value.Key.ToString();
            window.text2.Text = pair2.Value.Value;

            window.ShowDialog();
        }

        private void Item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
            }
        }

        private void Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }
    }

    class IndexToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int) value;
            return index != -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class Select2ToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList selected = value as IList;
            return selected.Count == 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class HeaderDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as DateTime?).Value.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
