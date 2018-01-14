using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBox.NeoModels
{
    public class TVShowView : TVShow
    {
        public long AirDate { get; set; }

        public int EpisodeCount { get; set; }

        public int SeasonNumber { get; set; }

        TVShowView()
        {

        }

        public TVShowView(TVShow tvshow, Season season)
        {
            Id = tvshow.Id;
            BackdropPath = tvshow.BackdropPath;
            FirstAirDate = tvshow.FirstAirDate;
            Homepage = tvshow.Homepage;
            InProduction = tvshow.InProduction;
            LastAirDate = tvshow.LastAirDate;
            Name = tvshow.Name + " Season " + season.SeasonNumber;
            NumberOfEpisodes = tvshow.NumberOfEpisodes;
            NumberOfEpisodes = tvshow.NumberOfSeasons;
            OriginalLanguage = tvshow.OriginalLanguage;
            OriginalName = tvshow.OriginalName;
            Overview = tvshow.Overview;
            Popularity = tvshow.Popularity;
            
            Path = tvshow.Path;

            Seasons = new List<Season>();
            Seasons.Add(season);
            AirDate = season.AirDate;
            EpisodeCount = season.EpisodeCount;
            SeasonNumber = season.SeasonNumber;
            PosterPath = season.Path;

            Genres = new List<Genre>();
            foreach (Genre element in tvshow.Genres)
            {
                Genres.Add(element);
            }

            CreatedBy = new List<TVShowCreator>();
            foreach (TVShowCreator element in tvshow.CreatedBy)
            {
                CreatedBy.Add(element);
            }

            Networks = new List<Network>();
            foreach (Network element in tvshow.Networks)
            {
                Networks.Add(element);
            }

            EpisodeRunTime = tvshow.EpisodeRunTime;
        }
    }
}
