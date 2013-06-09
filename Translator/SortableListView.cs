using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;

namespace Translator
{
    public class SortableListView : ListView
    {
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public SortableListView()
        {
            this.AddHandler(
                GridViewColumnHeader.ClickEvent,
                new RoutedEventHandler(GridViewColumnHeaderClickedHandler));
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);

            if (dataView != null)
            {
                dataView.SortDescriptions.Clear();
                SortDescription sd = new SortDescription(sortBy, direction);
                dataView.SortDescriptions.Add(sd);
                dataView.Refresh();
            }
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked == null)
                return;

            string sortBy = GetSortPropertyName(headerClicked.Column);
            if (string.IsNullOrEmpty(sortBy))
                return;

            if (headerClicked != null &&
                headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                if (headerClicked != _lastHeaderClicked)
                {
                    if (_lastHeaderClicked != null)
                        _lastHeaderClicked.Column.HeaderTemplate = App.Current.Resources["HeaderDataTemplate_Normal"] as DataTemplate;

                    direction = ListSortDirection.Ascending;
                }
                else
                {
                    if (_lastDirection == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                    }
                }

                // see if we have an attached SortPropertyName value
                Sort(sortBy, direction);

                IsSorted = true;

                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;

                //headerClicked.Background = new SolidColorBrush(Colors.Tomato);
                if (direction == ListSortDirection.Ascending)
                    headerClicked.Column.HeaderTemplate = App.Current.Resources["HeaderDataTemplate_Asc"] as DataTemplate;
                else
                    headerClicked.Column.HeaderTemplate = App.Current.Resources["HeaderDataTemplate_Dsc"] as DataTemplate;
            }
        }

        public void ClearSorting()
        {
            if (_lastHeaderClicked != null)
            {
                _lastHeaderClicked.Column.HeaderTemplate = App.Current.Resources["HeaderDataTemplate_Normal"] as DataTemplate;
                _lastHeaderClicked = null;

                ICollectionView dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);

                if (dataView != null)
                {
                    dataView.SortDescriptions.Clear();
                    dataView.Refresh();
                }

                IsSorted = false;
            }
        }

        public static readonly DependencyProperty SortPropertyNameProperty =
            DependencyProperty.RegisterAttached("SortPropertyName", typeof(string), typeof(SortableListView));

        public static string GetSortPropertyName(GridViewColumn obj)
        {
            return (string)obj.GetValue(SortPropertyNameProperty);
        }

        public static void SetSortPropertyName(GridViewColumn obj, string value)
        {
            obj.SetValue(SortPropertyNameProperty, value);
        }

        public static readonly DependencyProperty IsSortedProperty =
            DependencyProperty.Register("IsSorted", typeof (bool), typeof (SortableListView), new PropertyMetadata(default(bool)));

        public bool IsSorted
        {
            get { return (bool) GetValue(IsSortedProperty); }
            set { SetValue(IsSortedProperty, value); }
        }
    }
}