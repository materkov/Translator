using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Translator
{
    class EMenuItem : MenuItem
    {
        protected override void OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == IsHighlightedProperty)
            {
                if ((bool)e.NewValue == true)
                    OnStartHighlight();
                else
                    OnEndHighlight();
            }

            base.OnPropertyChanged(e);
        }

        public static readonly DependencyProperty HoverProperty =
            DependencyProperty.Register("Hover", typeof (string), typeof (EMenuItem), new PropertyMetadata(default(string)));

        public string Hover
        {
            get { return (string) GetValue(HoverProperty); }
            set { SetValue(HoverProperty, value); }
        }

        private void OnStartHighlight()
        {
            Logger.ShowPopup(Hover);
        }

        private void OnEndHighlight()
        {
            Logger.HidePopup(Hover);
        }
    }
}
