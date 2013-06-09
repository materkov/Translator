using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Xceed.Wpf.Toolkit;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;
using ColorPicker = Xceed.Wpf.Toolkit.ColorPicker;

namespace Translator
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void UpdateBinding()
        {
            comboBox1.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();
            
            colorPicker1.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker11.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker2.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker22.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker3.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker33.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker4.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker44.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker5.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker55.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker6.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker66.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker7.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();

            colorPicker100.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker101.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
            colorPicker102.GetBindingExpression(ColorCanvas.SelectedColorProperty).UpdateSource();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            UpdateBinding();

            DialogResult = true;
        }

        public ObservableCollection<ColorItem> Colors { get; set; }

        private void window1_Loaded(object sender, RoutedEventArgs e)
        {
            var collection = (Application.Current.Resources["IDEState"] as IDEState).FavoriteDirs;
            folderManagerControl.Init(collection, this);
        }

        private void buttonToDefault_Click(object sender, RoutedEventArgs e)
        {
            var state = Application.Current.Resources["IDEState"] as IDEState;
            state.ToDefault();

            UpdateBinding();
        }
    }
}
