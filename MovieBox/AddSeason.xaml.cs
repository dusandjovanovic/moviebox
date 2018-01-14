using DM.MovieApi;
using DM.MovieApi.ApiResponse;
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
using MovieBox.NeoModels;
using MovieBox.NeoController;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MovieBox
{
    public sealed partial class AddSeason : ContentDialog
    {
        private NeoModels.Season addSeason { get; set; }
        private NeoModels.TVShow show { get; set; }
        private bool already = false;

        /// <summary>
        /// API communication attributes
        /// </summary>
        private IApiTVShowRequest seasonAPI;
       
        public AddSeason()
        {
            this.InitializeComponent();
            MovieDbFactory.RegisterSettings("c837ca9610248f9584be59c8a1f2d44b", "http://api.themoviedb.org/3/");
            seasonAPI = MovieDbFactory.Create<IApiTVShowRequest>().Value;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!btnPath.IsEnabled)
                return;

            NeoSingleton._connect();
            if (already)
            {
                NeoSingleton._addSeason(addSeason, show);
                seasonList.Instance.addSeason(addSeason, show);
            }
            else
            {
                NeoSingleton._addTvShow(show);
                seasonList.Instance.addTVShow(show);
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private async void btnPath_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                txtPath.Text = folder.Path;
                show.Path = folder.Path;
            }
        }

        private async void btnMedatada_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSeries.Text))
                return;

            String series = txtSeries.Text;
            int season = cmbSeason.SelectedIndex + 1;

            ProgressMeter.Visibility = Visibility.Visible;
            ProgressMeter.IsActive = true;

            try
            {
                show = await apiSeason(series, season);
                lblAlert.Text = "";
            }
            catch (Exception)
            {
                lblAlert.Text = "Metadata not found, check your title!";
                ProgressMeter.Visibility = Visibility.Collapsed;
                ProgressMeter.IsActive = false;
                return;
            }

            txtSeries.Text = show.Name;
            txtYear.Text = show.FirstAirDate.Year.ToString();
            txtRuntime.Text = show.EpisodeRunTime.ToString();
            txtGenres.Text = String.Join(",", show.Genres);
            txtWriters.Text = String.Join(",", show.CreatedBy);
            txtNeworks.Text = String.Join(",", show.Networks);
            txtPlot.Text = show.Overview;

            Uri imageUri = new Uri(show.PosterPath, UriKind.Absolute);
            BitmapImage imageBitmap = new BitmapImage(imageUri);
            imgPoster.Source = imageBitmap;

            btnPath.IsEnabled = true;

            ProgressMeter.Visibility = Visibility.Collapsed;
            ProgressMeter.IsActive = false;
        }

        private async Task<NeoModels.TVShow> apiSeason(string searchQuery, int season)
        {
            ApiSearchResponse<TVShowInfo> response = await seasonAPI.SearchByNameAsync(searchQuery);

            TVShowInfo info = response.Results[0];
            ApiQueryResponse<DM.MovieApi.MovieDb.TV.TVShow> metadata = await seasonAPI.FindByIdAsync(info.Id);


            NeoModels.TVShow tvshow = new NeoModels.TVShow(metadata.Item, season);

            String plot = tvshow.Overview;
            String poster = "http://image.tmdb.org/t/p/w342/" + tvshow.Seasons[0].Path;
            string titleWhole = tvshow.Name + " Season " + season;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            string appPath = folder.Path;
            appPath += "\\";
            appPath += titleWhole;
            appPath += ".jpg";

            await downloadImage(new Uri(poster), appPath);

            tvshow.Seasons[0].Path = appPath;
            tvshow.PosterPath = appPath;

            if (seasonList.Instance.existsInList(tvshow))
            {
                already = true;
                addSeason = tvshow.Seasons[0];
            }

            return tvshow;
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