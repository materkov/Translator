using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;

namespace Translator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            (Resources["IDEState"] as IDEState).LoadConfiguration();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Сначала все события, 
            base.OnExit(e);

            // И только в сааамом конце
            IDEState.Get().SaveConfiguration();
        }
    }

    class SelectedToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int idx = (int) value;
            return idx != -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}