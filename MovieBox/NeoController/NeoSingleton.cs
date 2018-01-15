using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieBox.NeoModels;
using Neo4jClient.Cypher;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

namespace MovieBox.NeoController
{
    public static class NeoSingleton
    {
        private static GraphClient _instance = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "admin");

        public static void _connect()
        {
            {
                if (_instance == null)
                    _instance = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "admin");

                try
                {
                    _instance.Connect();
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }

        public static void _addMovie(Movie movie)
        {
            NeoMovie newMovie = new NeoMovie(movie);

            _instance.Cypher
                .Merge("(movie:Movie { Id: {id} })")
                .OnCreate()
                .Set("movie = {newMovie}")
                .WithParams(new
                {
                    id = movie.Id,
                    newMovie
                })
                .ExecuteWithoutResults();


            foreach (Actor newActor in movie.Actors)
            {
                _instance.Cypher
                     .Merge("(actor:Actor { PersonId: {personId} })")
                     .OnCreate()
                     .Set("actor = {newActor}")
                     .WithParams(new
                     {
                         personId = newActor.PersonId,
                         newActor
                     })
                 .ExecuteWithoutResults();

                _instance.Cypher
                    .Match("(foundMovie:Movie)", "(foundActor:Actor)")
                    .Where((NeoMovie foundMovie) => foundMovie.Id == movie.Id)
                    .AndWhere((Actor foundActor) => foundActor.PersonId == newActor.PersonId)
                    .CreateUnique("(foundActor)-[:ACTS_IN]->(foundMovie)")
                    .ExecuteWithoutResults();
            }

            foreach (Genre newGenre in movie.Genres)
            {
                _instance.Cypher
                     .Merge("(genre:Genre { Id: {genreId} })")
                     .OnCreate()
                     .Set("genre = {newGenre}")
                     .WithParams(new
                     {
                         genreId = newGenre.Id,
                         newGenre
                     })
                 .ExecuteWithoutResults();

                _instance.Cypher
                    .Match("(foundMovie:Movie)", "(foundGenre:Genre)")
                    .Where((NeoMovie foundMovie) => foundMovie.Id == movie.Id)
                    .AndWhere((Genre foundGenre) => foundGenre.Id == newGenre.Id)
                    .CreateUnique("(foundMovie)-[:IS_GENRE]->(foundGenre)")
                    .ExecuteWithoutResults();
            }

            foreach (Director newDirector in movie.Directors)
            {
                _instance.Cypher
                     .Merge("(director:Director { PersonId: {personId} })")
                     .OnCreate()
                     .Set("director = {newDirector}")
                     .WithParams(new
                     {
                         personId = newDirector.PersonId,
                         newDirector
                     })
                 .ExecuteWithoutResults();

                _instance.Cypher
                    .Match("(foundMovie:Movie)", "(foundDirector:Director)")
                    .Where((NeoMovie foundMovie) => foundMovie.Id == movie.Id)
                    .AndWhere((Actor foundDirector) => foundDirector.PersonId == newDirector.PersonId)
                    .CreateUnique("(foundDirector)-[:DIRECTED]->(foundMovie)")
                    .ExecuteWithoutResults();
            }

            foreach (Writer newWriter in movie.Writers)
            {
                _instance.Cypher
                     .Merge("(writer:Writer { PersonId: {personId} })")
                     .OnCreate()
                     .Set("writer = {newWriter}")
                     .WithParams(new
                     {
                         personId = newWriter.PersonId,
                         newWriter
                     })
                 .ExecuteWithoutResults();

                _instance.Cypher
                    .Match("(foundMovie:Movie)", "(foundWriter:Writer)")
                    .Where((NeoMovie foundMovie) => foundMovie.Id == movie.Id)
                    .AndWhere((Writer foundWriter) => foundWriter.PersonId == newWriter.PersonId)
                    .CreateUnique("(foundWriter)-[:WROTE]->(foundMovie)")
                    .ExecuteWithoutResults();
            }
        }

        public static void _removeMovie(Movie todelete)
        {
            _instance.Cypher
                .OptionalMatch("(movie:Movie)-[r]->()")
                .Where((NeoMovie movie) => movie.Id == todelete.Id)
                .DetachDelete("r, movie")
                .ExecuteWithoutResults();

            _instance.Cypher
                .OptionalMatch("(movie:Movie)<-[r]-()")
                .Where((NeoMovie movie) => movie.Id == todelete.Id)
                .DetachDelete("r, movie")
                .ExecuteWithoutResults();

            _instance.Cypher
                .Match("(orphanNode)")
                .Where("not (orphanNode)-[]-()")
                .Delete("orphanNode")
                .ExecuteWithoutResults();

            _instance.Cypher
               .Match("(movie:Movie)")
               .Where((NeoMovie movie) => movie.Id == todelete.Id)
               .Delete("movie")
               .ExecuteWithoutResults();
        }

        public static void _modifyMovie(Movie tomodify)
        {
            NeoMovie newMovie = new NeoMovie(tomodify);

            _instance.Cypher
                .Match("(movie:Movie)")
                .Where((NeoMovie movie) => movie.Id == tomodify.Id)
                .Set("movie = {newMetadata}")
                .WithParam("newMetadata", newMovie)
                .ExecuteWithoutResults();
        }

        public static List<Actor> _allActors()
        {
            var returned = _instance.Cypher
                .Match("(actor:Actor)-[ACTS_IN]->(movie:Movie)")
                .ReturnDistinct(actor => actor.As<Actor>()).Results;

            return returned.ToList();
        }

        public static List<Genre> _allGenres()
        {
            var returned = _instance.Cypher
                .Match("(genre:Genre)<-[IS_GENRE]-(movie:Movie)")
                .ReturnDistinct(genre => genre.As<Genre>()).Results;

            return returned.ToList();
        }

        public static List<Director> _allDirectors()
        {
            var returned = _instance.Cypher
                .Match("(director:Director)-[DIRECTED]->(movie:Movie)")
                .ReturnDistinct(director => director.As<Director>()).Results;

            return returned.ToList();
        }

        public static List<Writer> _allWriters()
        {
            var returned = _instance.Cypher
                .Match("(writer:Writer)-[WROTE]->(movie:Movie)")
                .ReturnDistinct(writer => writer.As<Writer>()).Results;

            return returned.ToList();
        }

        public static ObservableCollection<Movie> _readByMultiple(int runtimeLow, int runtimeHigh, int yearLow, int yearHigh, IEnumerable<object> actors, IEnumerable<object> directors, IEnumerable<object> genres)
        {

            string[] actorsList = actors.Select(p => p.ToString()).ToArray();
            string[] genresList = genres.Select(p => p.ToString()).ToArray();
            string[] directorsList = directors.Select(p => p.ToString()).ToArray();

            var returned = _instance.Cypher
               .Match("(movie:Movie)<-[ACTS_IN]-(actor:Actor)")
               .Match("(movie:Movie)<-[DIRECTED]-(director:Director)")
               .Match("(movie:Movie)-[IS_GENRE]->(genre:Genre)")
               .Where((NeoMovie movie) => movie.Runtime >= runtimeLow)
               .AndWhere((NeoMovie movie) => movie.Runtime <= runtimeHigh)
               .AndWhere((NeoMovie movie) => movie.Year >= yearLow)
               .AndWhere((NeoMovie movie) => movie.Year <= yearHigh)
               .AndWhere("actor.Name in {actorsList}")
               .AndWhere("genre.Name in {genresList}")
               .AndWhere("director.Name in {directorsList}")
               .WithParams(new
               {
                   actorsList,
                   genresList,
                   directorsList
               })
               .ReturnDistinct(movie => movie.As<NeoMovie>())
               .Results;

            ObservableCollection<Movie> movieList = new ObservableCollection<Movie>();

            foreach (NeoMovie obj in returned)
            {
                movieList.Add(_readMovie(obj));
            }

            return movieList;
        }

        public static ObservableCollection<Movie> _readByMeta(object actorName, object directorName, object genreName)
        {
            string actorQ;
            if (actorName == null)
                actorQ = ".*";
            else
                actorQ = actorName.ToString();

            string directorQ;
            if (directorName == null)
                directorQ = ".*";
            else
                directorQ = directorName.ToString();

            string genreQ;
            if (genreName == null)
                genreQ = ".*";
            else
                genreQ = genreName.ToString();

            var returned = _instance.Cypher
                .Match("(movie:Movie)<-[ACTS_IN]-(actor:Actor)")
                .Match("(movie:Movie)<-[DIRECTED]-(director:Director)")
                .Match("(movie:Movie)-[IS_GENRE]->(genre:Genre)")
                .Where("actor.Name =~ {actorName}")
                .AndWhere("director.Name =~ {directorName}")
                .AndWhere("genre.Name =~ {genreName}")
                .WithParams(new{
                    actorName = actorQ,
                    directorName = directorQ,
                    genreName = genreQ
                })
                .ReturnDistinct(movie => movie.As<NeoMovie>())
                .Results;

            ObservableCollection<Movie> movieList = new ObservableCollection<Movie>();

            foreach (NeoMovie obj in returned)
            {
                movieList.Add(_readMovie(obj));
            }

            return movieList;
        }

        public static List<Movie> _readAll()
        {
            var returned = _instance.Cypher
                .Match("(movie:Movie)")
                .Return(movie => movie.As<NeoMovie>())
                .Results;

            List<Movie> movieList = new List<Movie>();

            foreach (NeoMovie obj in returned)
            {
                movieList.Add(_readMovie(obj));
            }

            return movieList;
        }

        private static Movie _readMovie(NeoMovie neo)
        {
            Movie add = new Movie(neo);

            var query = _instance.Cypher
                    .OptionalMatch("(actor:Actor)-[ACTS_IN]->(movie:Movie)")
                    .Where((NeoMovie movie) => movie.Id == add.Id)
                    .Return(actor => actor.As<Actor>())
                    .Results;

            foreach (Actor actor in query)
            {
                add.Actors.Add(actor);
            }

            var query2 = _instance.Cypher
                .OptionalMatch("(director:Director)-[DIRECTED]->(movie:Movie)")
                .Where((NeoMovie movie) => movie.Id == add.Id)
                .Return(director => director.As<Director>())
                .Results;

            foreach (Director director in query2)
            {
                add.Directors.Add(director);
            }

            var query3 = _instance.Cypher
                .OptionalMatch("(writer:Writer)-[WROTE]->(movie:Movie)")
                .Where((NeoMovie movie) => movie.Id == add.Id)
                .Return(writer => writer.As<Writer>())
                .Results;

            foreach (Writer writer in query3)
            {
                add.Writers.Add(writer);
            }

            var query4 = _instance.Cypher
                .OptionalMatch("(movie:Movie)-[IS_GENRE]->(genre:Genre)")
                .Where((NeoMovie movie) => movie.Id == add.Id)
                .Return(genre => genre.As<Genre>())
                .Results;

            foreach (Genre genre in query4)
            {
                add.Genres.Add(genre);
            }

            return add;
        }

        public static int _numberOfMovies()
        {
            var query = _instance.Cypher
                    .OptionalMatch("(movie:Movie)")
                    .Return(movie => movie.As<NeoMovie>())
                    .Results;

            return query.Count();
        }

        public static int _numberOfSeasons()
        {
            var query = _instance.Cypher
                    .OptionalMatch("(season:Season)")
                    .Return(season => season.As<Season>())
                    .Results;

            return query.Count();
        }

        private static TVShow _readShow(NeoTvShow neo)
        {
            TVShow add = new TVShow(neo);

            var query = _instance.Cypher
                    .OptionalMatch("(season:Season)<-[HAS]-(tvshow:TVShow)")
                    .Where((NeoTvShow tvshow) => tvshow.Id == add.Id)
                    .Return(season => season.As<Season>())
                    .Results;

            foreach (Season season in query)
            {
                add.Seasons.Add(season);
            }

            var query2 = _instance.Cypher
                .OptionalMatch("(creator:TVShowCreator)-[CREATED]->(tvshow:TVShow)")
                .Where((NeoTvShow tvshow) => tvshow.Id == add.Id)
                .Return(creator => creator.As<TVShowCreator>())
                .Results;

            foreach (TVShowCreator creator in query2)
            {
                add.CreatedBy.Add(creator);
            }

            var query3 = _instance.Cypher
                .OptionalMatch("(network:Network)<-[BELONGS]-(tvshow:TVShow)")
                .Where((NeoTvShow tvshow) => tvshow.Id == add.Id)
                .Return(network => network.As<Network>())
                .Results;

            foreach (Network network in query3)
            {
                add.Networks.Add(network);
            }

            var query4 = _instance.Cypher
                .OptionalMatch("(tvshow:TVShow)-[IS_GENRE]->(genre:Genre)")
                .Where((NeoTvShow tvshow) => tvshow.Id == add.Id)
                .Return(genre => genre.As<Genre>())
                .Results;

            foreach (Genre genre in query4)
            {
                add.Genres.Add(genre);
            }

            return add;
        }

        public static List<TVShow> _readTVShows()
        {
            var returned = _instance.Cypher
                .Match("(tvshow:TVShow)")
                .Return(tvshow => tvshow.As<NeoTvShow>())
                .Results;

            List<TVShow> showList = new List<TVShow>();

            foreach (NeoTvShow obj in returned)
            {
                showList.Add(_readShow(obj));
            }

            return showList;
        }

        public static void _addTvShow(TVShow show)
        {
            NeoTvShow newTVShow = new NeoTvShow(show);

            _instance.Cypher
                .Merge("(show:TVShow { Id: {id} })")
                .OnCreate()
                .Set("show = {newTVShow}")
                .WithParams(new
                {
                    id = show.Id,
                    newTVShow
                })
                .ExecuteWithoutResults();

            foreach (Genre newGenre in show.Genres)
            {
                _instance.Cypher
                     .Merge("(genre:Genre { Id: {genreId} })")
                     .OnCreate()
                     .Set("genre = {newGenre}")
                     .WithParams(new
                     {
                         genreId = newGenre.Id,
                         newGenre
                     })
                 .ExecuteWithoutResults();

                _instance.Cypher
                    .Match("(foundShow:TVShow)", "(foundGenre:Genre)")
                    .Where((NeoTvShow foundShow) => foundShow.Id == show.Id)
                    .AndWhere((Genre foundGenre) => foundGenre.Id == newGenre.Id)
                    .CreateUnique("(foundShow)-[:IS_GENRE]->(foundGenre)")
                    .ExecuteWithoutResults();
            }

            foreach (Network newNetwork in show.Networks)
            {
                _instance.Cypher
                     .Merge("(network:Network { Id: {networkId} })")
                     .OnCreate()
                     .Set("network = {newNetwork}")
                     .WithParams(new
                     {
                         networkId = newNetwork.Id,
                         newNetwork
                     })
                 .ExecuteWithoutResults();

                _instance.Cypher
                    .Match("(foundShow:TVShow)", "(foundNetwork:Network)")
                    .Where((NeoTvShow foundShow) => foundShow.Id == show.Id)
                    .AndWhere((Network foundNetwork) => foundNetwork.Id == newNetwork.Id)
                    .CreateUnique("(foundShow)-[:BELONGS]->(foundNetwork)")
                    .ExecuteWithoutResults();
            }

            foreach (TVShowCreator newCreator in show.CreatedBy)
            {
                _instance.Cypher
                     .Merge("(creator:TVShowCreator { Id: {creatorId} })")
                     .OnCreate()
                     .Set("creator = {newCreator}")
                     .WithParams(new
                     {
                         creatorId = newCreator.Id,
                         newCreator
                     })
                 .ExecuteWithoutResults();

                _instance.Cypher
                    .Match("(foundShow:TVShow)", "(foundCreator:TVShowCreator)")
                    .Where((NeoTvShow foundShow) => foundShow.Id == show.Id)
                    .AndWhere((TVShowCreator foundCreator) => foundCreator.Id == newCreator.Id)
                    .CreateUnique("(foundShow)<-[:CREATED]-(foundCreator)")
                    .ExecuteWithoutResults();
            }

            foreach (Season newSeason in show.Seasons)
            {
                _instance.Cypher
                     .Merge("(season:Season { Id: {seasonId} })")
                     .OnCreate()
                     .Set("season = {newSeason}")
                     .WithParams(new
                     {
                         seasonId = newSeason.Id,
                         newSeason
                     })
                 .ExecuteWithoutResults();

                _instance.Cypher
                    .Match("(foundShow:TVShow)", "(foundSeason:Season)")
                    .Where((NeoTvShow foundShow) => foundShow.Id == show.Id)
                    .AndWhere((Season foundSeason) => foundSeason.Id == newSeason.Id)
                    .CreateUnique("(foundShow)-[:HAS]->(foundSeason)")
                    .ExecuteWithoutResults();
            }
        }

        public static void _addSeason(Season newSeason, TVShow show)
        {
            _instance.Cypher
                    .Merge("(season:Season { Id: {seasonId} })")
                    .OnCreate()
                    .Set("season = {newSeason}")
                    .WithParams(new
                    {
                        seasonId = newSeason.Id,
                        newSeason
                    })
                .ExecuteWithoutResults();

            _instance.Cypher
                .Match("(foundShow:TVShow)", "(foundSeason:Season)")
                .Where((NeoTvShow foundShow) => foundShow.Id == show.Id)
                .AndWhere((Season foundSeason) => foundSeason.Id == newSeason.Id)
                .CreateUnique("(foundShow)-[:HAS]->(foundSeason)")
                .ExecuteWithoutResults();
        }

        public static void _removeTVShow(TVShow todelete)
        {
            _instance.Cypher
                .OptionalMatch("(show:TVShow)-[r]->()")
                .Where((NeoTvShow show) => show.Id == todelete.Id)
                .DetachDelete("r, show")
                .ExecuteWithoutResults();

            _instance.Cypher
                .OptionalMatch("(show:TVShow)<-[r]-()")
                .Where((NeoTvShow show) => show.Id == todelete.Id)
                .DetachDelete("r, show")
                .ExecuteWithoutResults();

            _instance.Cypher
                .Match("(orphanNode)")
                .Where("not (orphanNode)-[]-()")
                .Delete("orphanNode")
                .ExecuteWithoutResults();

            _instance.Cypher
               .Match("(show:TVShow)")
               .Where((NeoTvShow show) => show.Id == todelete.Id)
               .Delete("show")
               .ExecuteWithoutResults();
        }

        public static void _removeSeason(Season todelete)
        {
            _instance.Cypher
                .OptionalMatch("(season:Season)<-[r]-()")
                .Where((Season season) => season.Id == todelete.Id)
                .DetachDelete("r, season")
                .ExecuteWithoutResults();
        }
    }
}
