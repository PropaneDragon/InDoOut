using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_UI_Common.Controls.Search
{
    public partial class SearchBar : UserControl, ISearch
    {
        private readonly string _slogan = "Search";

        public event EventHandler<SearchArgs> SearchRequested;

        public bool DynamicallySearch { get; set; } = false;

        public SearchBar()
        {
            InitializeComponent();

            _slogan = Text_Slogan.Text;
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

        private void TextBox_Query_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateSloganText();

            if (DynamicallySearch)
            {
                PerformSearch();
            }

            if (e.Key == Key.Enter)
            {
                PerformSearch();
                Keyboard.ClearFocus();
            }
            else if (e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
            }
        }

        private void TextBox_Query_LostFocus(object sender, System.Windows.RoutedEventArgs e) => UpdateSloganText();

        private void TextBox_Query_KeyDown(object sender, KeyEventArgs e) => UpdateSloganText();

        private void TextBox_Query_GotFocus(object sender, System.Windows.RoutedEventArgs e) => TextBox_Query.Text = "";

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e) => PerformSearch();

        private void UserControl_GotFocus(object sender, System.Windows.RoutedEventArgs e) => _ = TextBox_Query.Focus();
    }
}
