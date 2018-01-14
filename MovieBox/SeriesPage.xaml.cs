using MovieBox.NeoModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MovieBox.NeoController;

namespace MovieBox
{
    public sealed partial class SeriesPage : Page
    {
        /// <summary>
        /// Observable Season objects -> AdaptiveGridView
        /// </summary>
        private ObservableCollection<TVShowView> Series;

        public SeriesPage()
        {
            this.InitializeComponent();
            Series = new ObservableCollection<TVShowView>();
            updateObservableSeries();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            TVShowView selectedSeason = (TVShowView)e.ClickedItem;
            TitleTextblock.Text = selectedSeason.Name;
            YearTextblock.Text = selectedSeason.FirstAirDate.Year.ToString();
            PlotTextblock.Text = selectedSeason.Overview;
            GenresTextblock.Text = String.Join(",", selectedSeason.Genres);
            WritersTextblock.Text = String.Join(",", selectedSeason.CreatedBy);
            NetworksTextblock.Text = String.Join(",", selectedSeason.Networks);
            AirDateTextblock.Text = (DateTimeOffset.FromUnixTimeSeconds(selectedSeason.AirDate)).UtcDateTime.ToString();
            NumberOfEpisodesTextblock.Text = selectedSeason.EpisodeCount.ToString();
            PopularityTextblock.Text = selectedSeason.Popularity.ToString();

            WritersTextblockHolder.Text = "Writers:";
            NetworksTextblockHolder.Text = "Networks:";
            AirDateTextblockHolder.Text = "Air Date:";
            NumberOfEpisodesTextblockHolder.Text = "Number of episodes:";
            PopularityTextblockHolder.Text = "Popularity:";


            if (selectedSeason.Path != null)
                PathTextblock.Text = selectedSeason.Path;

            PosterImage.Source = new BitmapImage(new Uri(PosterImage.BaseUri, selectedSeason.PosterPath));
        }

        private async void AddSeasonButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new AddSeason();
            await dialog.ShowAsync();

            updateObservableSeries();
        }

        private async void RemoveSeasonButton_Click(object sender, RoutedEventArgs e)
        {
            if (SeasonGridView.SelectedItem == null)
                return;

            TVShowView selected = (TVShowView)SeasonGridView.SelectedItem;

            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("Are you sure you want to remove " + selected.Name + " from your collection?");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Yes, Remove", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CommandInvokedHandler)));

            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;

            await messageDialog.ShowAsync();
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            TVShowView selected = (TVShowView)SeasonGridView.SelectedItem;

            if (command.Label == "Cancel")
                return;
            else
            {
                int foundSeasons = 0;
                foreach (TVShowView obj in Series)
                    if (obj.Id == selected.Id)
                        foundSeasons++;

                if (foundSeasons == 1)
                {
                    seasonList.Instance.deleteShow(selected);
                    NeoSingleton._connect();
                    NeoSingleton._removeTVShow(selected);
                }
                else
                {
                    seasonList.Instance.deleteSeason(selected, selected.Seasons[0]);
                    NeoSingleton._connect();
                    NeoSingleton._removeSeason(selected.Seasons[0]);
                }
            }

            updateObservableSeries();

            TitleTextblock.Text = "";
            YearTextblock.Text = "";
            PlotTextblock.Text = "";
            GenresTextblock.Text = "";
            NetworksTextblock.Text = "";
            WritersTextblock.Text = "";
            AirDateTextblock.Text = "";
            NumberOfEpisodesTextblock.Text = "";
            PopularityTextblock.Text = "";


            WritersTextblockHolder.Text = "";
            PathTextblock.Text = "";
            NetworksTextblockHolder.Text = "";
            AirDateTextblockHolder.Text = "";
            NumberOfEpisodesTextblockHolder.Text = "";
            PopularityTextblockHolder.Text = "";

            PosterImage.Source = null;
        }

        /// <summary>
        /// Helper methods
        /// </summary>

        private void updateObservableSeries()
        {
            Series.Clear();

            foreach (TVShow show in seasonList.Instance.listSeasonValues)
            {
                foreach (Season season in show.Seasons)
                    Series.Add(new TVShowView(show, season));
            }
        }
    }
}
