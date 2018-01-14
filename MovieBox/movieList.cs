using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieBox.NeoModels;

namespace MovieBox
{
    public class movieList
    {
        #region Atributes

        private List<Movie> listMovie;

        #endregion


        #region Properties

        public List<Movie> listMovieValues
        {
            get
            {
                return listMovie;
            }
            set
            {
                listMovie = value;
            }
        }

        #endregion


        #region Constructors

        public movieList()
        {
            listMovie = new List<Movie>();
        }

        #endregion


        public bool addMovie(Movie toAdd)
        {
            if (existsInList(toAdd.Title))
                return false;

            listMovie.Add(toAdd);
            return true;
        }

        public bool modifyMovie(Movie toModify)
        {
            var tmp = getMovie(toModify.Title);

            if (tmp == null)
                return false;

            tmp = toModify;

            return true;
        }

        public bool deleteMovie(Movie toDelete)
        {
            if (!existsInList(toDelete))
                return false;

            listMovie.Remove(toDelete);
            return true;
        }

        public bool deleteMovie(String title)
        {
            Movie tmpMovie = null;

            foreach (var v in listMovie)
            {
                if (v.Title == title)
                {
                    tmpMovie = v;
                    break;
                }
            }

            if (tmpMovie != null)
            {
                listMovie.Remove(tmpMovie);
                return true;
            }

            return false;
        }

        public bool deleteMovie(int id)
        {
            foreach (Movie obj in listMovie)
                if (obj.Id == id)
                {
                    listMovie.Remove(obj);
                    return true;
                }

            return false;
        }

        public bool existsInList(Movie movie)
        {
            foreach (var v in listMovie)
            {
                if (v.Id == movie.Id)
                    return true;
            }

            return false;
        }

        public bool existsInList(String title)
        {
            foreach (var v in listMovie)
            {
                if (v.Title == title)
                    return true;
            }

            return false;
        }

        public Movie getMovie(String title)
        {
            foreach (var v in listMovie)
            {
                if (v.Title == title)
                    return v;
            }

            return null;
        }

        private static movieList _instance = null;

        public static movieList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new movieList();

                return _instance;
            }
        }

        public static void GetAllMovieTitles(ObservableCollection<Movie> list)
        {
            list.Clear();
            foreach (Movie movie in _instance.listMovieValues)
                list.Add(movie);
        }

        public static Movie GetMovieByName(string title)
        {
            foreach (Movie movie in movieList.Instance.listMovieValues)
                if (movie.Title == title)
                    return movie;
            return null;
        }
    }
}
