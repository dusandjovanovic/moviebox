using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieBox.NeoModels;

namespace MovieBox
{
    public class seasonList
    {
        #region Atributes

        private List<TVShow> listSeason;

        #endregion


        #region Properties

        public List<TVShow> listSeasonValues
        {
            get
            {
                return listSeason;
            }
        }

        #endregion


        #region Constructors

        public seasonList()
        {
            listSeason = new List<TVShow>();
        }

        #endregion


        public bool addTVShow(TVShow toAdd)
        {
            if (existsInList(toAdd.Id))
                return false;

            listSeason.Add(toAdd);
            return true;
        }

        public bool addSeason(Season toAdd, TVShow source)
        {
            if (existsInList(source))
            {
                foreach (Season obj in getShow(source.Id).Seasons)
                    if (obj.Id == toAdd.Id)
                        return false;
                getShow(source.Id).Seasons.Add(toAdd);
                return false;
            }

            addTVShow(source);
            return true;
        }

        public bool modifyShow(TVShow toModify)
        {
            var tmp = getShow(toModify.Id);

            if (tmp == null)
                return false;

            tmp = toModify;

            return true;
        }

        public bool deleteShow(TVShow toDelete)
        {
            if (!existsInList(toDelete))
                return false;

            deleteShow(toDelete.Id);
            return true;
        }

        public bool deleteSeason(TVShow from, Season toDelete)
        {
            if (!existsInList(from))
                return false;

            getShow(from.Id).Seasons.Remove(toDelete);
            return true;
        }

        public bool deleteShow(int id)
        {
            TVShow tmp = null;

            foreach (var v in listSeason)
            {
                if (v.Id == id)
                {
                    tmp = v;
                    break;
                }
            }

            if (tmp != null)
            {
                listSeason.Remove(tmp);
                return true;
            }

            return false;
        }

        public bool existsInList(TVShow show)
        {
            foreach (var v in listSeason)
            {
                if (v.Id == show.Id)
                    return true;
            }

            return false;
        }

        public bool existsInList(int id)
        {
            foreach (var v in listSeason)
            {
                if (v.Id == id)
                    return true;
            }

            return false;
        }

        public TVShow getShow(int id)
        {
            foreach (var v in listSeason)
            {
                if (v.Id == id)
                    return v;
            }

            return null;
        }

        private static seasonList _instance = null;

        public static seasonList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new seasonList();

                return _instance;
            }
        }
    }
}
