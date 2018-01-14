using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi.MovieDb.Movies;

namespace MovieBox.NeoModels
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsAdultThemed { get; set; }

        public string BackdropPath { get; set; }

        public List<Genre> Genres { get; set; }
        public List<Director> Directors { get; set; }
        public List<Writer> Writers { get; set; }
        public List<Actor> Actors { get; set; }

        public string OriginalTitle { get; set; }

        public int Runtime { get; set; }

        public string Overview { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int Year { get; set; }

        public string Poster { get; set; }

        public string Path { get; set; }

        public double Popularity { get; set; }

        public double VoteAverage { get; set; }

        public int VoteCount { get; set; }

        public Movie()
        {
            Genres = new List<Genre>();
            Directors = new List<Director>();
            Writers = new List<Writer>();
            Actors = new List<Actor>();
        }

        public Movie(DM.MovieApi.MovieDb.Movies.Movie movie, DM.MovieApi.MovieDb.Movies.MovieCredit credit)
        {
            Id = movie.Id;
            Title = movie.Title;
            IsAdultThemed = movie.IsAdultThemed;
            BackdropPath = movie.BackdropPath;
            OriginalTitle = movie.OriginalTitle;
            Runtime = movie.Runtime;
            Overview = movie.Overview;
            ReleaseDate = movie.ReleaseDate;
            Year = ReleaseDate.Year;
            Poster = movie.PosterPath;
            Path = "";
            Popularity = movie.Popularity;
            VoteAverage = movie.VoteAverage;
            VoteCount = movie.VoteCount;

            Genres = new List<Genre>();
            foreach (DM.MovieApi.MovieDb.Genres.Genre element in movie.Genres)
            {
                Genres.Add(new Genre(element));
            }

            Directors = new List<Director>();
            foreach (DM.MovieApi.MovieDb.Movies.MovieCrewMember element in credit.CrewMembers)
            {
                if (element.Job == "Director")
                    Directors.Add(new Director(element));
            }

            Writers = new List<Writer>();
            foreach (DM.MovieApi.MovieDb.Movies.MovieCrewMember element in credit.CrewMembers)
            {
                if (element.Department == "Writing")
                    Writers.Add(new Writer(element));
            }

            Actors = new List<Actor>();
            long iter = 0;
            foreach (DM.MovieApi.MovieDb.Movies.MovieCastMember element in credit.CastMembers)
            {
                if (iter >= 20)
                    break;
                Actors.Add(new Actor(element));
                iter++;
            }
        }

        public Movie(Movie movie)
        {
            Id = movie.Id;
            Title = movie.Title;
            IsAdultThemed = movie.IsAdultThemed;
            BackdropPath = movie.BackdropPath;
            OriginalTitle = movie.OriginalTitle;
            Runtime = movie.Runtime;
            Overview = movie.Overview;
            ReleaseDate = movie.ReleaseDate;
            Year = ReleaseDate.Year;
            Poster = movie.Poster;
            Path = movie.Path;
            Popularity = movie.Popularity;
            VoteAverage = movie.VoteAverage;
            VoteCount = movie.VoteCount;

            Genres = new List<Genre>();
            foreach (Genre element in movie.Genres)
            {
                Genres.Add(element);
            }

            Directors = new List<Director>();
            foreach (Director element in movie.Directors)
            {
                Directors.Add(element);
            }

            Writers = new List<Writer>();
            foreach (Writer element in movie.Writers)
            {
                Writers.Add(element);
            }

            Actors = new List<Actor>();
            foreach (Actor element in movie.Actors)
            {
                Actors.Add(element);
            }
        }

        public Movie(NeoMovie neomovie)
        {
            Id = neomovie.Id;
            Title = neomovie.Title;
            IsAdultThemed = neomovie.IsAdultThemed;
            BackdropPath = neomovie.BackdropPath;
            OriginalTitle = neomovie.OriginalTitle;
            Runtime = neomovie.Runtime;
            Overview = neomovie.Overview;
            ReleaseDate = (DateTimeOffset.FromUnixTimeSeconds(neomovie.ReleaseDate)).UtcDateTime;
            Year = ReleaseDate.Year;
            Poster = neomovie.Poster;
            Path = neomovie.Path;
            Popularity = neomovie.Popularity;
            VoteAverage = neomovie.VoteAverage;
            VoteCount = neomovie.VoteCount;

            Genres = new List<Genre>();
            Directors = new List<Director>();
            Writers = new List<Writer>();
            Actors = new List<Actor>();
        }

        public override string ToString()
            => $"{Title} ({ReleaseDate:yyyy-MM-dd}) [{Id}]";
    }
}
