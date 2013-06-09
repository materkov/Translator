using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Translator
{
    class CollapsedTextBlock : TextBlock
    {
        public CollapsedTextBlock()
        {
            _typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        }

        public static readonly DependencyProperty RenderWidthProperty =
            DependencyProperty.Register("RenderWidth", typeof (double), typeof (CollapsedTextBlock), new PropertyMetadata(default(double), RenderWidthChangedCallback));

        private static void RenderWidthChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            CollapsedTextBlock obj = dependencyObject as CollapsedTextBlock;
            obj.Resize();
        }

        public double RenderWidth
        {
            get { return (double) GetValue(RenderWidthProperty); }
            set { SetValue(RenderWidthProperty, value); }
        }

        public static readonly DependencyProperty FullTextProperty =
            DependencyProperty.Register("FullText", typeof (string), typeof (CollapsedTextBlock), new PropertyMetadata(default(string), FullTextChangedCallback));

        private static void FullTextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            CollapsedTextBlock obj = dependencyObject as CollapsedTextBlock;
            obj.Resize();
        }

        public string FullText
        {
            get { return (string) GetValue(FullTextProperty); }
            set { SetValue(FullTextProperty, value); }
        }

        private readonly Typeface _typeface;

        private double GetWidth(string str)
        {
            var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, FontSize, Brushes.Black);
            return formattedText.Width;
        }

        private string TrimIteration(string str, int iterNum, out bool noWhere)
        {
            int idx1 = str.IndexOf('\\');
            int idx2 = (iterNum == 0) ? str.IndexOf('\\', idx1 + 1) : str.IndexOf('\\', idx1 + 5);
            if (idx2 == -1)
            {
                noWhere = true;
            }
            else
            {
                str = str.Insert(idx1 + 1, "...");
                str = str.Remove(idx1 + 4, idx2 - idx1 - 1);

                noWhere = false;
            }

            return str;
        }

        private void Resize()
        {
            string newStr = FullText;
            if (newStr == null)
                return;

            int iterNum = 0;
            bool noWhere = false;   // Сжимать уже некуда?

            while (!noWhere && (GetWidth(newStr) > RenderWidth-10))
            {
                newStr = TrimIteration(newStr, iterNum++, out noWhere);
            }

            Text = newStr;
        }
    }
}
