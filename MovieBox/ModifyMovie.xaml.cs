using DM.MovieApi;
using DM.MovieApi.ApiResponse;
using DM.MovieApi.MovieDb.Movies;
using DM.MovieApi.MovieDb.TV;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MovieBox.NeoController;

namespace MovieBox
{
    public sealed partial class ModifyMovie : ContentDialog
    {
        private MovieBox.NeoModels.Movie addMovie { get; set; }

        /// <summary>
        /// API communication attributes
        /// </summary>
        private IApiMovieRequest movieAPI;

        public ModifyMovie(MovieBox.NeoModels.Movie movie)
        {
            this.InitializeComponent();
            MovieDbFactory.RegisterSettings("c837ca9610248f9584be59c8a1f2d44b", "http://api.themoviedb.org/3/");
            movieAPI = MovieDbFactory.Create<IApiMovieRequest>().Value;

            addMovie = new MovieBox.NeoModels.Movie(movie);
            txtTitle.Text = movie.Title;
            txtYear.Text = movie.Year.ToString();
            txtRuntime.Text = movie.Runtime.ToString();
            txtGenres.Text = String.Join(",", movie.Genres);
            txtDirectors.Text = String.Join(",", movie.Directors);
            txtWriters.Text = String.Join(",", movie.Writers);
            txtActors.Text = String.Join(",", movie.Actors);
            txtPlot.Text = movie.Overview;
            if (movie.Path != null)
            {
                txtPath.Text = movie.Path;
                addMovie.Path = movie.Path;
            }

            Uri imageUri = new Uri(addMovie.Poster, UriKind.Absolute);
            BitmapImage imageBitmap = new BitmapImage(imageUri);
            imgPoster.Source = imageBitmap;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            movieList.Instance.modifyMovie(addMovie);
            NeoSingleton._connect();
            NeoSingleton._modifyMovie(addMovie);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private async void btnPath_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp4");
            picker.FileTypeFilter.Add(".m4v");
            picker.FileTypeFilter.Add(".mkv");
            picker.FileTypeFilter.Add(".avi");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                txtPath.Text = file.Path;
                addMovie.Path = file.Path;
            }
        }

        private async void btnMedatada_Click(object sender, RoutedEventArgs e)
        {
            String separator = ",";

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
                return;

            String movieTitle = txtTitle.Text;

            ProgressMeter.Visibility = Visibility.Visible;
            ProgressMeter.IsActive = true;

            try
            {
                addMovie = await ApiMovieAsync(movieTitle);
                addMovie.Path = txtPath.Text;
                lblAlert.Text = "";
            }
            catch (Exception)
            {
                lblAlert.Text = "Metadata not found, check your title!";
                ProgressMeter.Visibility = Visibility.Collapsed;
                ProgressMeter.IsActive = false;
                return;
            }

            txtTitle.Text = addMovie.Title;
            txtYear.Text = addMovie.Year.ToString();
            txtRuntime.Text = addMovie.Runtime.ToString();
            txtGenres.Text = String.Join(separator, addMovie.Genres);
            txtDirectors.Text = String.Join(separator, addMovie.Directors);
            txtWriters.Text = String.Join(separator, addMovie.Writers);
            txtActors.Text = String.Join(separator, addMovie.Actors);
            txtPlot.Text = addMovie.Overview;

            Uri imageUri = new Uri(addMovie.Poster, UriKind.Absolute);
            BitmapImage imageBitmap = new BitmapImage(imageUri);
            imgPoster.Source = imageBitmap;

            btnPath.IsEnabled = true;

            ProgressMeter.Visibility = Visibility.Collapsed;
            ProgressMeter.IsActive = false;
        }

        private async Task<NeoModels.Movie> ApiMovieAsync(string searchQuery)
        {
            ApiSearchResponse<MovieInfo> response = await movieAPI.SearchByTitleAsync(searchQuery);

            MovieInfo info = response.Results[0];
            ApiQueryResponse<DM.MovieApi.MovieDb.Movies.Movie> movieinfo = await movieAPI.FindByIdAsync(info.Id);
            ApiQueryResponse<DM.MovieApi.MovieDb.Movies.MovieCredit> credits = await movieAPI.GetCreditsAsync(info.Id);

            NeoModels.Movie movie = new NeoModels.Movie(movieinfo.Item, credits.Item);

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
