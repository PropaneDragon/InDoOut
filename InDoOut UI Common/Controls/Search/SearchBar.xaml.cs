using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace InDoOut_UI_Common.Controls.Search
{
    public partial class SearchBar : UserControl, ISearch
    {
        private static readonly TimeSpan AUTO_SEARCH_INTERVAL = TimeSpan.FromMilliseconds(500);

        private readonly string _slogan = "Search";
        private readonly DispatcherTimer _searchTimer = new(DispatcherPriority.Normal);

        public event EventHandler<SearchArgs> SearchRequested;

        public bool DynamicallySearch { get; set; } = true;

        public SearchBar()
        {
            InitializeComponent();

            _slogan = Text_Slogan.Text;
            _searchTimer.Interval = AUTO_SEARCH_INTERVAL;
            _searchTimer.Tick += SearchTimer_Tick;
        }

        private void PerformSearch() => PerformSearch(TextBox_Query.Text);

        private void PerformSearch(string query)
        {
            if (query != null)
            {
                SearchRequested?.Invoke(this, new SearchArgs(query));
            }
        }

        private void UpdateSloganText() => Text_Slogan.Text = string.IsNullOrEmpty(TextBox_Query.Text) ? _slogan : "";

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();

            if (DynamicallySearch)
            {
                PerformSearch();
            }
        }

        private void TextBox_Query_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateSloganText();

            if (e.Key == Key.Enter)
            {
                PerformSearch();
            }
            else if (e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
            }
            else if (DynamicallySearch)
            {
                _searchTimer.Stop();
                _searchTimer.Start();
            }
        }

        private void TextBox_Query_LostFocus(object sender, System.Windows.RoutedEventArgs e) => UpdateSloganText();

        private void TextBox_Query_KeyDown(object sender, KeyEventArgs e) => UpdateSloganText();

        private void TextBox_Query_GotFocus(object sender, System.Windows.RoutedEventArgs e) => TextBox_Query.Text = "";

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e) => PerformSearch();

        private void UserControl_GotFocus(object sender, System.Windows.RoutedEventArgs e) => _ = TextBox_Query.Focus();
    }
}
