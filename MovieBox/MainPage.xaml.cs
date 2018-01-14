using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MovieBox.NeoModels;

namespace MovieBox
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<Movie> Movies;

        public MainPage()
        {
            this.InitializeComponent();
            
            MenuItemsListbox.SelectedItem = ListboxMovies;
            BackButton.Visibility = Visibility.Collapsed;
            TitleTextblock.Text = "Movies";
            Movies = new ObservableCollection<Movie>();
            NavigationalFrame.Navigate(typeof(MoviesPage));
        }


        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            HamburgerSpliView.IsPaneOpen = !HamburgerSpliView.IsPaneOpen;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            goBack();
        }

        private void goBack()
        {
            NavigationalFrame.Navigate(typeof(MoviesPage));
            MenuItemsListbox.SelectedItem = ListboxMovies;
            BackButton.Visibility = Visibility.Collapsed;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListboxMovies.IsSelected)
            {
                BackButton.Visibility = Visibility.Collapsed;
                NavigationalFrame.Navigate(typeof(MoviesPage));
                TitleTextblock.Text = "Movies";
            }
            else if (ListboxSeries.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                NavigationalFrame.Navigate(typeof(SeriesPage));
                TitleTextblock.Text = "Series";
            }
            else if (ListboxExplore.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                NavigationalFrame.Navigate(typeof(ExplorePage));
                TitleTextblock.Text = "Explore";
            }
            else if (ListboxDetailedView.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                NavigationalFrame.Navigate(typeof(DetailedViewPage));
                TitleTextblock.Text = "Detailed view";
            }
            else if (ListboxPreferences.IsSelected)
            {
                BackButton.Visibility = Visibility.Visible;
                NavigationalFrame.Navigate(typeof(PreferencesPage));
                TitleTextblock.Text = "Preferences";
            }
            else
            {
                BackButton.Visibility = Visibility.Visible;
                NavigationalFrame.Navigate(typeof(Playing));
                TitleTextblock.Text = "Now Playing..";
            }
        }

        private void SearchAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (String.IsNullOrEmpty(sender.Text)) goBack();

            List<string> Suggestions = new List<string>();
            movieList.GetAllMovieTitles(Movies);

            Suggestions = Movies.Where(p => p.Title.StartsWith(sender.Text)).Select(p => p.Title).ToList();
            SearchAutoSuggestBox.ItemsSource = Suggestions;
        }

        private async void SearchAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Movie toDisplay = movieList.GetMovieByName(sender.Text);
            ContentDialog dialog = new SearchQuery(toDisplay);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                if (String.IsNullOrEmpty(App.mediaPath))
                    return;
                NavigationalFrame.Navigate(typeof(Playing));
                MenuItemsListbox.SelectedItem = ListboxPlaying;
            }
        }

        private void NavigationalFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if(NavigationalFrame.CurrentSourcePageType == typeof(MoviesPage))
            {
                BackButton.Visibility = Visibility.Collapsed;
                ListboxMovies.IsSelected = true;
                TitleTextblock.Text = "Movies";
            }
            else if (NavigationalFrame.CurrentSourcePageType == typeof(SeriesPage))
            {
                BackButton.Visibility = Visibility.Visible;
                ListboxSeries.IsSelected = true;
                TitleTextblock.Text = "Series";
            }
            else if (NavigationalFrame.CurrentSourcePageType == typeof(ExplorePage))
            {
                BackButton.Visibility = Visibility.Visible;
                ListboxExplore.IsSelected = true;
                TitleTextblock.Text = "Explore";
            }
            else if (NavigationalFrame.CurrentSourcePageType == typeof(DetailedViewPage))
            {
                BackButton.Visibility = Visibility.Visible;
                ListboxDetailedView.IsSelected = true;
                TitleTextblock.Text = "Detailed view";
            }
            else if (NavigationalFrame.CurrentSourcePageType == typeof(PreferencesPage))
            {
                BackButton.Visibility = Visibility.Visible;
                ListboxPreferences.IsSelected = true;
                TitleTextblock.Text = "Preferences";
            }
            else if (NavigationalFrame.CurrentSourcePageType == typeof(Playing))
            {
                BackButton.Visibility = Visibility.Visible;
                ListboxPlaying.IsSelected = true;
                TitleTextblock.Text = "Now Playing";
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
