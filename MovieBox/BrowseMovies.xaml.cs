using DM.MovieApi;
using DM.MovieApi.ApiResponse;
using DM.MovieApi.MovieDb.Movies;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MovieBox.NeoModels;
using MovieBox.NeoController;

namespace MovieBox
{
    public sealed partial class BrowseMovies : ContentDialog
    {

        private ObservableCollection<MovieBox.NeoModels.Movie> moviesPath;
        private ObservableCollection<string> files;
        private MovieBox.NeoModels.Movie addMovie;

        /// <summary>
        /// API communication attributes
        /// </summary>
        private IApiMovieRequest movieAPI;

        public BrowseMovies()
        {
            this.InitializeComponent();
            MovieDbFactory.RegisterSettings("c837ca9610248f9584be59c8a1f2d44b", "http://api.themoviedb.org/3/");
            movieAPI = MovieDbFactory.Create<IApiMovieRequest>().Value;
            moviesPath = new ObservableCollection<MovieBox.NeoModels.Movie>();
            files = new ObservableCollection<string>();

            this.initializeFiles();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NeoSingleton._connect();
            await NeoSingleton._addMovieAsync(addMovie);
            MovieBox.NeoModels.Movie movie = (MovieBox.NeoModels.Movie)MoviesList.SelectedItem;
            movieList.Instance.addMovie(addMovie);
            moviesPath.Remove(movie);
        }

        private async void MetadataButton_Click(object sender, RoutedEventArgs e)
        {
            String separator = ",";

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
                return;

            if (moviesPath.Count == 0)
                return;

            ProgressMeter.Visibility = Visibility.Visible;
            ProgressMeter.IsActive = true;

            String movieTitle = txtTitle.Text;
            try
            {
                addMovie = await ApiMovieAsync(movieTitle);
                addMovie.Path = ((MovieBox.NeoModels.Movie)MoviesList.SelectedItem).Path;
                lblAlert.Text = "";
            }
            catch (Exception)
            {
                lblAlert.Text = "Metadata not found, check your title!";
                ProgressMeter.Visibility = Visibility.Collapsed;
                ProgressMeter.IsActive = false;
                return;
            }

            AddButton.IsEnabled = true;

            txtTitle.Text = addMovie.Title;
            txtYear.Text = addMovie.Year.ToString();
            txtGenres.Text = String.Join(separator, addMovie.Genres);
            txtDirectors.Text = String.Join(separator, addMovie.Directors);
            txtPlot.Text = addMovie.Overview;

            ProgressMeter.Visibility = Visibility.Collapsed;
            ProgressMeter.IsActive = false;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
                    
        }

        private void MoviesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddButton.IsEnabled = false;
            MovieBox.NeoModels.Movie selected = (MovieBox.NeoModels.Movie)MoviesList.SelectedItem;
            txtYear.Text = "";
            txtDirectors.Text = "";
            txtGenres.Text = "";
            txtPlot.Text = "";

            if (selected == null && moviesPath.Count == 0)
            {
                txtTitle.Text = "";
                MetadataButton.IsEnabled = false;
                AddButton.IsEnabled = false;
                lblAlert.Text = "All movies have been imported!";
                BackBorder.Opacity = 0;
                return;
            }
            else if (selected == null)
                return;

            if (selected.Title == null)
                return;
            txtTitle.Text = selected.Title;
        }

        private void initializeMovies()
        {
            foreach(String obj in files) {
                String movieTitle = Path.GetFileName(obj);
                movieTitle = movieTitle.Remove(movieTitle.Length - 4);
                MovieBox.NeoModels.Movie movie = new MovieBox.NeoModels.Movie();
                movie.Title = movieTitle;
                movie.Path = obj;
                moviesPath.Add(movie);
            }
            
        }

        private async void initializeFiles()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder directorty = await folderPicker.PickSingleFolderAsync();
            if (directorty != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", directorty);
                bool waited;

                waited = await GetFiles(directorty);
            }

            initializeMovies();
        }

        private async Task<bool> GetFiles(StorageFolder folder)
        {
            StorageFolder fold = folder;
            var items = await fold.GetItemsAsync();

            foreach (var item in items)
            {
                if (item.GetType() == typeof(StorageFile) && (item.Path.ToString().EndsWith("mkv") || item.Path.ToString().EndsWith("mp4") || item.Path.ToString().EndsWith("m4v") || item.Path.ToString().EndsWith("avi")))
                    files.Add(item.Path.ToString());
            }
            return true;
        }

        private async Task<MovieBox.NeoModels.Movie> ApiMovieAsync(string searchQuery)
        {
            ApiSearchResponse<MovieInfo> response = await movieAPI.SearchByTitleAsync(searchQuery);

            MovieInfo info = response.Results[0];
            ApiQueryResponse<DM.MovieApi.MovieDb.Movies.Movie> movieinfo = await movieAPI.FindByIdAsync(info.Id);
            ApiQueryResponse<DM.MovieApi.MovieDb.Movies.MovieCredit> credits = await movieAPI.GetCreditsAsync(info.Id);

            MovieBox.NeoModels.Movie movie = new MovieBox.NeoModels.Movie(movieinfo.Item, credits.Item);

            movie.Title = movie.Title.Replace(':', '-');

            String poster = "http://image.tmdb.org/t/p/w342/" + movie.Poster;

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            string appPath = folder.Path;
            appPath += "\\";
            appPath += movie.Title;
            appPath += ".jpg";

            await downloadImage(new Uri(poster), appPath);
            movie.Poster = appPath;

            return movie;
        }

        public async Task downloadImage(Uri uri, string destination)
        {
            try
            {
                HttpClient client = new HttpClient();
                byte[] buffer = await client.GetByteArrayAsync(uri);
                using (FileStream file = new FileStream(destination, FileMode.Create, System.IO.FileAccess.Write))
                    file.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load the image: {0}", ex.Message);
            }
        }
   }
}