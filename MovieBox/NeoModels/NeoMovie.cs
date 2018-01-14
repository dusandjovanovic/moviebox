using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBox.NeoModels
{
    public class NeoMovie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsAdultThemed { get; set; }

        public string BackdropPath { get; set; }

        public string OriginalTitle { get; set; }

        public int Runtime { get; set; }

        public string Overview { get; set; }

        public long ReleaseDate { get; set; }

        public int Year { get; set; }

        public string Poster { get; set; }

        public string Path { get; set; }

        public double Popularity { get; set; }

        public double VoteAverage { get; set; }

        public int VoteCount { get; set; }

        public NeoMovie(Movie movie)
        {
            Id = movie.Id;
            Title = movie.Title;
            IsAdultThemed = movie.IsAdultThemed;
            BackdropPath = movie.BackdropPath;
            OriginalTitle = movie.OriginalTitle;
            Runtime = movie.Runtime;
            Overview = movie.Overview;
            ReleaseDate = ((DateTimeOffset)movie.ReleaseDate).ToUnixTimeSeconds();
            Year = movie.Year;
            Poster = movie.Poster;
            Path = movie.Path;
            Popularity = movie.Popularity;
            VoteAverage = movie.VoteAverage;
            VoteCount = movie.VoteCount;
        }

        public NeoMovie()
        {

        }
    }
}
