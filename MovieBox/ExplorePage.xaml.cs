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
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MovieBox.NeoModels;
using MovieBox.NeoController;

namespace MovieBox
{
    public sealed partial class ExplorePage : Page
    {
        /// <summary>
        /// API Communication attribute
        /// </summary>
        private IApiMovieRequest movieAPI;
        private ObservableCollection<NeoModels.Movie> Suggested { get; set; }
        private List<bool> ShouldAdd;

        public ExplorePage()
        {
            this.InitializeComponent();

            ShouldAdd = new List<bool>();
            Suggested = new ObservableCollection<NeoModels.Movie>();
            MovieDbFactory.RegisterSettings("c837ca9610248f9584be59c8a1f2d44b", "http://api.themoviedb.org/3/");
            movieAPI = MovieDbFactory.Create<IApiMovieRequest>().Value;
            control.IsTapEnabled = false;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Intro.Visibility = Visibility.Collapsed;
            NeoModels.Movie async = await getSuggestionsAsync();
            control.IsTapEnabled = true;
        }

        private void AddToButton_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            foreach (NeoModels.Movie movie in Suggested)
            {
                if (ShouldAdd[index])
                {
                    movieList.Instance.addMovie(movie);
                    NeoSingleton._connect();
                    NeoSingleton._addMovie(movie);
                }
                index++;
            }
            Frame.Navigate(typeof(MoviesPage));
        }

        private void indicator_ItemClick(object sender, ItemClickEventArgs e)
        {
            int index = control.SelectedIndex;
            if (ShouldAdd[index])
                AddToCheckBox.IsChecked = true;
            else
                AddToCheckBox.IsChecked = false;

            NeoModels.Movie selectedMovie = (NeoModels.Movie)e.ClickedItem;
            TitleTextblock.Text = selectedMovie.Title;
            YearTextblock.Text = selectedMovie.Year.ToString();
            PlotTextblock.Text = selectedMovie.Overview;
            GenresTextblock.Text = String.Join(",", selectedMovie.Genres);
            DirectorsTextblock.Text = String.Join(",", selectedMovie.Directors);
            ActorsTextblock.Text = String.Join(",", selectedMovie.Actors);

            DirectorsTextblockHolder.Text = "Directors:";
            ActorsTextblockHolder.Text = "Actors:";
        }

        private void control_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int index = control.SelectedIndex;
            if (ShouldAdd[index])
                AddToCheckBox.IsChecked = true;
            else
                AddToCheckBox.IsChecked = false;

            NeoModels.Movie selectedMovie = (NeoModels.Movie)indicator.SelectedItem;
            if (selectedMovie == null)
                return;

            TitleTextblock.Text = selectedMovie.Title;
            YearTextblock.Text = selectedMovie.Year.ToString();
            PlotTextblock.Text = selectedMovie.Overview;
            GenresTextblock.Text = String.Join(",", selectedMovie.Genres);
            DirectorsTextblock.Text = String.Join(",", selectedMovie.Directors);
            ActorsTextblock.Text = String.Join(",", selectedMovie.Actors);

            DirectorsTextblockHolder.Text = "Directors:";
            ActorsTextblockHolder.Text = "Actors:";
        }

        private void AddToCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int index = control.SelectedIndex;
            ShouldAdd[index] = true;
        }

        private void AddToCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            int index = control.SelectedIndex;
            ShouldAdd[index] = false;
        }

        /// <summary>
        /// Helper methods
        /// </summary>
        /// <returns></returns>
        private async Task<NeoModels.Movie> getSuggestionsAsync()
        {
            Suggested.Clear();
            ShouldAdd.Clear();

            try
            {
                LoadingControl.Visibility = Visibility.Visible;
                LoadingControl.IsEnabled = true;
                LoadingControl.IsLoading = true;

                Random randomGenerator = new Random();
                int randomPage = randomGenerator.Next(1, 20);
                ApiSearchResponse<MovieInfo> response = await movieAPI.GetTopRatedAsync(randomPage);
                int iter = 0;

                foreach (MovieInfo info in response.Results)
                {
                    if (movieList.Instance.existsInList(info.Title))
                        continue;

                    NeoModels.Movie addMovie = await ApiMovieAsync(info.Title);

                    Suggested.Add(addMovie);
                    ShouldAdd.Add(false);

                    if (iter++ == 10)
                        break;
                }

                randomPage = randomGenerator.Next(1, 20);
                response = await movieAPI.GetPopularAsync(randomPage);
                iter = 0;

                foreach (MovieInfo info in response.Results)
                {
                    if (movieList.Instance.existsInList(info.Title))
                        continue;

                    NeoModels.Movie addMovie = await ApiMovieAsync(info.Title);

                    Suggested.Add(addMovie);
                    ShouldAdd.Add(false);

                    if (iter++ == 10)
                        break;
                }
            }
            catch
            {
                LoadingControl.IsLoading = false;
                AddToCheckBox.Visibility = Visibility.Visible;
                control.Visibility = Visibility.Visible;
                control.SelectedIndex = 9;
                return null;
            }

            LoadingControl.IsLoading = false;
            AddToCheckBox.Visibility = Visibility.Visible;
            control.Visibility = Visibility.Visible;
            control.SelectedIndex = 9;
            return null;
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
