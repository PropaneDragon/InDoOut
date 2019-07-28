using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Desktop.UI.Controls.Search
{
    public partial class SearchBar : UserControl, ISearch
    {
        private string _slogan = "Search";

        public event EventHandler<SearchArgs> SearchRequested;

        public SearchBar()
        {
            InitializeComponent();

            _slogan = TextBox_Query.Text;
        }

        private void PerformSearch()
        {
            PerformSearch(TextBox_Query.Text);
        }

        private void PerformSearch(string query)
        {
            if (query != null)
            {
                SearchRequested?.Invoke(this, new SearchArgs(query));
            }
        }

        private void TextBox_Query_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox_Query.Text = "";
        }

        private void TextBox_Query_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox_Query.Text))
            {
                TextBox_Query.Text = _slogan;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void TextBox_Query_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
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
    }
}
