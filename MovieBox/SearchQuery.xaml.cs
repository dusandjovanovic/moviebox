using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MovieBox.NeoModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MovieBox
{
    public sealed partial class SearchQuery : ContentDialog
    {
        private Movie toDisplay;

        public SearchQuery(Movie showMovie)
        {
            this.InitializeComponent();

            if (showMovie == null)
            {
                holderActors.Text = "There is no movie for given query";
                holderDirectors.Text = "";
                holderWriters.Text = "";
                return;
            }
                

            txtTitle.Text = showMovie.Title;
            txtYear.Text = showMovie.Year.ToString();
            txtGenres.Text = String.Join(",", showMovie.Genres);
            txtDirectors.Text = String.Join(",", showMovie.Directors);
            txtWriters.Text = String.Join(",", showMovie.Writers);
            txtActors.Text = String.Join(",", showMovie.Actors);
            txtPlot.Text = showMovie.Overview;
            if (showMovie.Path != null)
            {
                txtPath.Text = showMovie.Path;
            }

            Uri imageUri = new Uri(showMovie.Poster, UriKind.Absolute);
            BitmapImage imageBitmap = new BitmapImage(imageUri);
            imgPoster.Source = imageBitmap;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!String.IsNullOrEmpty(txtPath.Text))
                App.mediaPath = txtPath.Text;
            else
                App.mediaPath = "";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
