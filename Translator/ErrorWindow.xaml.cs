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
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string shortDesc = "", string fullDesc = "")
        {
            InitializeComponent();

            ErrorShortDesc.Text = shortDesc;
            ErrorFullDesc.Text = fullDesc;
        }

        public string ShortDesc
        {
            get { return ErrorShortDesc.Text; }
            set { ErrorShortDesc.Text = value; }
        }

        public string FullDesc
        {
            get { return ErrorFullDesc.Text; }
            set { ErrorFullDesc.Text = value; }
        }

        public Exception Exception { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Details_OnClick(object sender, RoutedEventArgs args)
        {
            StringBuilder b = new StringBuilder();
            Exception e = Exception;

            while (e != null)
            {
                b.AppendFormat("Class       : {0}", e.GetType().ToString());
                b.AppendLine();
                b.AppendFormat("Message     : {0}", e.Message);
                b.AppendLine();
                b.AppendFormat("Source      : {0}", e.Source);
                b.AppendLine();
                b.AppendFormat("TargetSite  : {0}", e.TargetSite.ToString());
                b.AppendLine();
                b.AppendLine("StackTrace  : ");
                b.AppendLine(e.StackTrace);
                b.AppendLine("---------------------------------------------------------------------------------------------");

                e = e.InnerException;
            }

            ErrorDetailsWindow details = new ErrorDetailsWindow() { Owner = this };
            details.Err.Text = b.ToString();
            details.ShowDialog();
        }
    }
}
