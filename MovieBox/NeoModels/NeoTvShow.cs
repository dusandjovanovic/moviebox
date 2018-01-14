using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBox.NeoModels
{
    public class NeoTvShow
    {
        public int Id { get; set; }

        public string BackdropPath { get; set; }

        public long FirstAirDate { get; set; }

        public int EpisodeRunTime { get; set; }

        public string Homepage { get; set; }

        public bool InProduction { get; set; }

        public long LastAirDate { get; set; }

        public string Name { get; set; }

        public int NumberOfEpisodes { get; set; }

        public int NumberOfSeasons { get; set; }

        public string OriginalLanguage { get; set; }

        public string OriginalName { get; set; }

        public string Overview { get; set; }

        public double Popularity { get; set; }

        public string PosterPath { get; set; }

        public string Path { get; set; }

        public NeoTvShow(TVShow tvshow)
        {
            Id = tvshow.Id;
            BackdropPath = tvshow.BackdropPath;
            FirstAirDate = ((DateTimeOffset)tvshow.FirstAirDate).ToUnixTimeSeconds();
            Homepage = tvshow.Homepage;
            InProduction = tvshow.InProduction;
            LastAirDate = ((DateTimeOffset)tvshow.LastAirDate).ToUnixTimeSeconds();
            Name = tvshow.Name;
            NumberOfEpisodes = tvshow.NumberOfEpisodes;
            NumberOfEpisodes = tvshow.NumberOfSeasons;
            OriginalLanguage = tvshow.OriginalLanguage;
            OriginalName = tvshow.OriginalName;
            Overview = tvshow.Overview;
            Popularity = tvshow.Popularity;
            PosterPath = tvshow.PosterPath;
            Path = tvshow.Path;
            EpisodeRunTime = tvshow.EpisodeRunTime;
        }

        public NeoTvShow()
        {

        }
    }
}
