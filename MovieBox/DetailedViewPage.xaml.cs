using MovieBox.NeoModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MovieBox.NeoController;

namespace MovieBox
{
    public sealed partial class DetailedViewPage : Page
    {
        /// <summary>
        /// Observable Movie objects -> MasterDetialView
        /// </summary>
        private ObservableCollection<Movie> Movies { get; set; }
        private ObservableCollection<string> Genres { get; set; }
        private ObservableCollection<string> Directors { get; set; }
        private ObservableCollection<string> Actors { get; set; }

        public DetailedViewPage()
        {
            this.InitializeComponent();

            Movies = new ObservableCollection<Movie>();
            Genres = new ObservableCollection<string>();
            Directors = new ObservableCollection<string>();
            Actors = new ObservableCollection<string>();
            updateObservableMovies();
            updateObservableListings();
        }

        private void formMovies()
        {
            Movies.Clear();

            foreach (Movie movie in NeoSingleton._readByMultiple((int)(RuntimeRange.RangeMin), (int)(RuntimeRange.RangeMax), (int)YearRange.RangeMin, (int)YearRange.RangeMax, ActorsList.SelectedItems.ToArray(), DirectorsList.SelectedItems.ToArray(), GenresList.SelectedItems.ToArray()))
            {
                Movies.Add(movie);
            }
        }

        private void updateObservableMovies()
        {
            Movies.Clear();
            int minimumRuntime = int.MaxValue;
            int maximumRuntime = 0;
            int minimumYear = int.MaxValue;
            int maximumYear = 0;

            foreach (Movie movie in movieList.Instance.listMovieValues)
            {
                if (movie.Year > maximumYear)
                    maximumYear = movie.Year;
                if (movie.Year < minimumYear)
                    minimumYear = movie.Year;

                if (movie.Runtime > maximumRuntime)
                    maximumRuntime = movie.Runtime;
                if (movie.Runtime < minimumRuntime)
                    minimumRuntime = movie.Runtime;

                Movies.Add(movie);
            }

            RuntimeRange.Minimum = (double)minimumRuntime;
            RuntimeRange.Maximum = (double)maximumRuntime;
            YearRange.Minimum = (double)minimumYear;
            YearRange.Maximum = (double)maximumYear;
        }

        private void updateObservable()
        {
            Movies.Clear();
            foreach (Movie movie in movieList.Instance.listMovieValues)
            {
                Movies.Add(movie);
            }
        }

        private void updateObservableListings() /* call whenever adding/removing a movie from static list */
        {
            
            Genres.Clear();
            Directors.Clear();
            Actors.Clear();
            foreach (Movie movie in movieList.Instance.listMovieValues)
            {
                if (movie.Genres != null)
                {
                    foreach (Genre genre in movie.Genres)
                        if (!Genres.Contains(genre.Name))
                            Genres.Add(genre.Name);
                }
                if (movie.Directors != null)
                {
                    foreach (Director director in movie.Directors)
                        if (!Directors.Contains(director.Name))
                            Directors.Add(director.Name);
                }
                if (movie.Actors != null)
                {
                    foreach (Actor actor in movie.Actors)
                        if (!Actors.Contains(actor.Name))
                        {
                            Actors.Add(actor.Name);
                        }
                }
            }

            Actors = new ObservableCollection<string>(Actors.OrderBy(o => o).ToList());
            Directors = new ObservableCollection<string>(Directors.OrderBy(o => o).ToList());
            Genres = new ObservableCollection<string>(Genres.OrderBy(o => o).ToList());
            
        }

        private void FlyoutPresenter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            FlyoutPresenter obj = (FlyoutPresenter)sender;
            obj.Visibility = Visibility.Collapsed;
        }

        private void FlyoutPresenter_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            FlyoutPresenter obj = (FlyoutPresenter)sender;
            obj.Visibility = Visibility.Collapsed;
        }

        private void FlyoutPresenter_PointerEntered_2(object sender, PointerRoutedEventArgs e)
        {
            FlyoutPresenter obj = (FlyoutPresenter)sender;
            obj.Visibility = Visibility.Collapsed;
        }

        private void DirectorsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            formMovies();
        }

        private void ActorsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            formMovies();
        }

        private void GenresList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            formMovies();
        }

        private void YearRange_ValueChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RangeChangedEventArgs e)
        {
            formMovies();
        }
    }
}
