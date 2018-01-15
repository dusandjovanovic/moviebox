using MovieBox.NeoModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MovieBox.NeoController;

namespace MovieBox
{
    public sealed partial class MoviesPage : Page
    {
        /// <summary>
        /// Observable Movie objects -> AdaptiveGridView
        /// </summary>
        private ObservableCollection<Movie> Movies { get; set; }

        /// <summary>
        /// Observable listings for genres, directors and actors
        /// </summary>
        private ObservableCollection<string> Genres { get; set; }
        private ObservableCollection<string> Directors { get; set; }
        private ObservableCollection<string> Actors { get; set; }

        public MoviesPage()
        {
            this.InitializeComponent();
            Movies = new ObservableCollection<Movie>();
            Genres = new ObservableCollection<string>();
            Directors = new ObservableCollection<string>();
            Actors = new ObservableCollection<string>();

            updateObservableMovies();
            updateObservableListings();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Movie selectedMovie = (Movie)e.ClickedItem;
            TitleTextblock.Text = selectedMovie.Title;
            YearTextblock.Text = selectedMovie.Year.ToString();
            PlotTextblock.Text = selectedMovie.Overview;
            GenresTextblock.Text = String.Join(",", selectedMovie.Genres);
            DirectorsTextblock.Text = String.Join(",", selectedMovie.Directors);
            ActorsTextblock.Text = String.Join(",", selectedMovie.Actors);

            DirectorsTextblockHolder.Text = "Directors:";
            ActorsTextblockHolder.Text = "Actors:";
            if(selectedMovie.Path != null)
                PathTextblock.Text = selectedMovie.Path;

            PosterImage.Source = new BitmapImage(new Uri(PosterImage.BaseUri, selectedMovie.Poster));
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(PlayMoviePresenter.Visibility == Visibility.Visible)
            {
                Movie selected = (Movie)MovieGridView.SelectedItem;
                if (selected == null)
                    return;
                else
                {
                    App.mediaPath = selected.Path;
                    Frame.Navigate(typeof(Playing));
                }
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Movie> Backup = new ObservableCollection<Movie>(Movies.OrderBy(o => o.Title).ToList());
            Movies.Clear();
            foreach (Movie obj in Backup)
                Movies.Add(obj);
        }

        private void PosterImage_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Movie selected = (Movie)MovieGridView.SelectedItem;
            if (selected == null)
                return;
            if(!String.IsNullOrEmpty(selected.Path))
                PlayMoviePresenter.Visibility = Visibility.Visible;
        }

        private void PlayMoviePresenter_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            PlayMoviePresenter.Visibility = Visibility.Collapsed;
        }

        private async void AddMovieButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new AddMovie();
            await dialog.ShowAsync();

            GenresComboBox.SelectedIndex = -1;
            DirectorsComboBox.SelectedIndex = -1;
            ActorsComboBox.SelectedIndex = -1;

            updateObservable();
            updateObservableListings();
        }

        private async void ModifyMovieButton_Click(object sender, RoutedEventArgs e)
        {
            Movie selected = (Movie)MovieGridView.SelectedItem;
            if (selected == null)
                return;

            ContentDialog dialog = new ModifyMovie(selected);
            await dialog.ShowAsync();

            GenresComboBox.SelectedIndex = -1;
            DirectorsComboBox.SelectedIndex = -1;
            ActorsComboBox.SelectedIndex = -1;

            updateObservable();
            updateObservableListings();
        }

        private async void RemoveMovieButton_Click(object sender, RoutedEventArgs e)
        {
            if (MovieGridView.SelectedItem == null)
                return;

            Movie selected = (Movie)MovieGridView.SelectedItem;

            var messageDialog = new MessageDialog("Are you sure you want to remove " + selected.Title + " from your collection?");
            messageDialog.Commands.Add(new UICommand("Yes, Remove", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;

            await messageDialog.ShowAsync();
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            Movie selected = (Movie)MovieGridView.SelectedItem;

            if (command.Label == "Cancel")
                return;
            else
            {
                movieList.Instance.deleteMovie(selected.Id);
                NeoSingleton._connect();
                NeoSingleton._removeMovie(selected);
            }

            GenresComboBox.SelectedIndex = -1;
            DirectorsComboBox.SelectedIndex = -1;
            ActorsComboBox.SelectedIndex = -1;

            updateObservable();
            updateObservableListings();

            TitleTextblock.Text = "";
            YearTextblock.Text = "";
            PlotTextblock.Text = "";
            GenresTextblock.Text = "";
            DirectorsTextblock.Text = "";
            ActorsTextblock.Text = "";

            DirectorsTextblockHolder.Text = "";
            ActorsTextblockHolder.Text = "";
            PathTextblock.Text = "";

            PosterImage.Source = null;
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new BrowseMovies();
            await dialog.ShowAsync();

            GenresComboBox.SelectedIndex = -1;
            DirectorsComboBox.SelectedIndex = -1;
            ActorsComboBox.SelectedIndex = -1;

            updateObservable();
            updateObservableListings();
        }

        private void RemoveSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            GenresComboBox.SelectedIndex = -1;
            DirectorsComboBox.SelectedIndex = -1;
            ActorsComboBox.SelectedIndex = -1;
            updateObservable();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(GenresComboBox.SelectedIndex == -1 && DirectorsComboBox.SelectedIndex == -1 && ActorsComboBox.SelectedIndex == -1)
            {
                updateObservableMovies();
                return;
            }

            updateObservable();
        }

        /// <summary>
        /// Helper methods
        /// </summary>

        private void updateObservableMovies()
        {
            Movies.Clear();

            foreach (Movie movie in movieList.Instance.listMovieValues)
                Movies.Add(movie);
        }

        private void updateObservable() {
            Movies.Clear();

            foreach (Movie obj in NeoSingleton._readByMeta(ActorsComboBox.SelectedItem, DirectorsComboBox.SelectedItem, GenresComboBox.SelectedItem))
                Movies.Add(obj);

        }

        private void updateObservableListings() /* call whenever adding/removing a movie from static list */
        {
            Genres.Clear();
            Directors.Clear();
            Actors.Clear();

            foreach (Genre genre in NeoSingleton._allGenres())
                if (!Genres.Contains(genre.Name))
                    Genres.Add(genre.Name);

            foreach (Director director in NeoSingleton._allDirectors())
                if (!Directors.Contains(director.Name))
                    Directors.Add(director.Name);

            foreach (Actor actor in NeoSingleton._allActors())
                if (!Actors.Contains(actor.Name))
                    Actors.Add(actor.Name);
        }
    }
}
