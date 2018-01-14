using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi.MovieDb.TV;

namespace MovieBox.NeoModels
{
    public class Season
    {
        public int Id { get; set; }

        public long AirDate { get; set; }

        public int EpisodeCount { get; set; }

        public string Path { get; set; }

        public int SeasonNumber { get; set; }

        public Season(int id, DateTime airDate, int episodeCount, string posterPath, int seasonNumber)
        {
            Id = id;
            AirDate = ((DateTimeOffset)airDate).ToUnixTimeSeconds();
            EpisodeCount = episodeCount;
            Path = posterPath;
            SeasonNumber = seasonNumber;
        }

        public Season(DM.MovieApi.MovieDb.TV.Season season)
        {
            Id = season.Id;
            AirDate = ((DateTimeOffset)season.AirDate).ToUnixTimeSeconds();
            EpisodeCount = season.EpisodeCount;
            Path = season.PosterPath;
            SeasonNumber = season.SeasonNumber;
        }

        public Season()
        {

        }

        public override string ToString()
            => $"({SeasonNumber} - {AirDate:yyyy-MM-dd})";
    }
}
