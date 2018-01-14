using DM.MovieApi;
using DM.MovieApi.MovieDb.Movies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Windows.Storage;
using DM.MovieApi.ApiResponse;
using MovieBox.NeoController;

namespace MovieBox
{
    public sealed partial class PreferencesPage : Page
    {
        public PreferencesPage()
        {
            this.InitializeComponent();
            NumberOfMovies.Text = "You have " + NeoSingleton._numberOfMovies().ToString() + " movies and " + NeoSingleton._numberOfSeasons().ToString() + " TVShow seasons in your collection.";
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            return;
        }
    }
}
