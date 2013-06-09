using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for StartPageWindow.xaml
    /// </summary>
    public partial class StartPageWindow : Window
    {
        public StartPageWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void HelpGroupBoxLoaded(object sender, RoutedEventArgs e)
        {
            Dictionary<CompileError.Type, int> recent = IDEState.Get().RecentErrors;

            FlowDocument doc = new FlowDocument();
            int[] num = new int[10];
            recent.TryGetValue(CompileError.Type.Type0, out num[0]);
            recent.TryGetValue(CompileError.Type.Type1, out num[1]);
            recent.TryGetValue(CompileError.Type.Type2, out num[2]);
            recent.TryGetValue(CompileError.Type.Type3, out num[3]);
            recent.TryGetValue(CompileError.Type.Type4, out num[4]);
            recent.TryGetValue(CompileError.Type.Type5, out num[5]);
            recent.TryGetValue(CompileError.Type.Type6, out num[6]);
            recent.TryGetValue(CompileError.Type.Type7, out num[7]);
            recent.TryGetValue(CompileError.Type.Type8, out num[8]);
            recent.TryGetValue(CompileError.Type.Type9, out num[9]);

            int max_idx = 0, max = num[0];
            for (int i = 1; i < 10; i++)
            {
                if (num[i] > max)
                {
                    max_idx = i;
                    max = num[i];
                }
            }
            
            Paragraph par = new Paragraph();

            if (IDEState.Get().IsFirstTime)
            {
                par.Inlines.Add(LocalString.Get("StartPage_Msg_FirstTime"));
                doc.Blocks.Add(par);
            }
            else if (recent.Count == 0)
            {
                par.Inlines.Add(LocalString.Get("StartPage_Msg_NoErrors"));
                doc.Blocks.Add(par);
            }
            else
            {
                par.Inlines.Add(LocalString.Get("StartPage_Msg_MostError"));
                par.Inlines.Add(new Bold(new Run("№" + max_idx)));
                doc.Blocks.Add(par);

                string shortDesc = LocalString.Get("Err_" + max_idx.ToString() + "_short");
                string fullDesc = LocalString.Get("Err_" + max_idx.ToString() + "_full");
 
                par = new Paragraph();
                par.Inlines.Add(LocalString.Get("StartPage_Msg_Short"));
                par.Inlines.Add(new Bold(new Run(shortDesc)));
                doc.Blocks.Add(par);

                par = new Paragraph();
                par.Inlines.Add(LocalString.Get("StartPage_Msg_Full"));
                par.Inlines.Add(new Bold(new Run(fullDesc)));
                doc.Blocks.Add(par); 
            }

            richTextBox1.Document = doc;
        }

        private void LastProject_DoubleCLick(object sender, MouseButtonEventArgs e)
        {
            string path = (sender as ListViewItem).Content as string;
            CodeProjectManager.Instance.Load(path);

            DialogResult = true;
        }

        private void LastProject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string path = (sender as ListViewItem).Content as string;
                CodeProjectManager.Instance.Load(path);
                e.Handled = true;

                DialogResult = true;
            }
        }
    }
}
