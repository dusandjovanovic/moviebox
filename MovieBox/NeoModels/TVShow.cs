using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi.MovieDb.TV;

namespace MovieBox.NeoModels
{
    public class TVShow
    {
        public int Id { get; set; }

        public string BackdropPath { get; set; }

        public DateTime FirstAirDate { get; set; }

        public List<Genre> Genres { get; set; }

        public List<TVShowCreator> CreatedBy { get; set; }

        public List<Season> Seasons { get; set; }

        public List<Network> Networks { get; set; }

        public int EpisodeRunTime { get; set; }

        public string Homepage { get; set; }

        public bool InProduction { get; set; }

        public DateTime LastAirDate { get; set; }

        public string Name { get; set; }

        public int NumberOfEpisodes { get; set; }

        public int NumberOfSeasons { get; set; }

        public string OriginalLanguage { get; set; }

        public string OriginalName { get; set; }

        public string Overview { get; set; }

        public double Popularity { get; set; }

        public string PosterPath { get; set; }

        public string Path { get; set; }

        public TVShow(DM.MovieApi.MovieDb.TV.TVShow tvshow, int season)
        {
            Id = tvshow.Id;
            BackdropPath = tvshow.BackdropPath;
            FirstAirDate = tvshow.FirstAirDate;
            Homepage = tvshow.Homepage;
            InProduction = tvshow.InProduction;
            LastAirDate = tvshow.LastAirDate;
            Name = tvshow.Name;
            NumberOfEpisodes = tvshow.NumberOfEpisodes;
            NumberOfEpisodes = tvshow.NumberOfSeasons;
            OriginalLanguage = tvshow.OriginalLanguage;
            OriginalName = tvshow.OriginalName;
            Overview = tvshow.Overview;
            Popularity = tvshow.Popularity;
            PosterPath = tvshow.PosterPath;
            Path = "";

            Genres = new List<Genre>();
            foreach (DM.MovieApi.MovieDb.Genres.Genre element in tvshow.Genres)
            {
                Genres.Add(new Genre(element));
            }

            CreatedBy = new List<TVShowCreator>();
            foreach (DM.MovieApi.MovieDb.TV.TVShowCreator element in tvshow.CreatedBy)
            {
                CreatedBy.Add(new TVShowCreator(element));
            }

            int iter = 0;
            Seasons = new List<Season>();
            foreach (DM.MovieApi.MovieDb.TV.Season element in tvshow.Seasons)
            {
                if(iter == season)
                    Seasons.Add(new Season(element));
                iter++;
            }

            Networks = new List<Network>();
            foreach (DM.MovieApi.MovieDb.TV.Network element in tvshow.Networks)
            {
                Networks.Add(new Network(element));
            }

            EpisodeRunTime = tvshow.EpisodeRunTime[0];
        }

        public TVShow(NeoTvShow tvshow)
        {
            Id = tvshow.Id;
            BackdropPath = tvshow.BackdropPath;
            FirstAirDate = (DateTimeOffset.FromUnixTimeSeconds(tvshow.FirstAirDate)).UtcDateTime;
            Homepage = tvshow.Homepage;
            InProduction = tvshow.InProduction;
            LastAirDate = (DateTimeOffset.FromUnixTimeSeconds(tvshow.LastAirDate)).UtcDateTime;
            Name = tvshow.Name;
            NumberOfEpisodes = tvshow.NumberOfEpisodes;
            NumberOfEpisodes = tvshow.NumberOfSeasons;
            OriginalLanguage = tvshow.OriginalLanguage;
            OriginalName = tvshow.OriginalName;
            Overview = tvshow.Overview;
            Popularity = tvshow.Popularity;
            PosterPath = tvshow.PosterPath;
            EpisodeRunTime = tvshow.EpisodeRunTime;
            Path = tvshow.Path;

            Genres = new List<Genre>();
            CreatedBy = new List<TVShowCreator>();
            Seasons = new List<Season>();
            Networks = new List<Network>();
        }

        public TVShow()
        {

        }

        public override string ToString()
            => $"{Name} ({FirstAirDate:yyyy-MM-dd}) [{Id}]";
    }
}
