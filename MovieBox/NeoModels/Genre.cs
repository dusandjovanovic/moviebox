using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi.MovieDb.Movies;

namespace MovieBox.NeoModels
{
    public class Genre
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public Genre(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Genre(DM.MovieApi.MovieDb.Genres.Genre genre)
        {
            Id = genre.Id;
            Name = genre.Name;
        }


        public Genre()
        {

        }

        public override bool Equals(object obj)
        {
            var genre = obj as Genre;
            if (genre == null)
            {
                return false;
            }

            return Equals(this, genre);
        }

        public bool Equals(Genre x, Genre y)
            => x.Id == y.Id && x.Name == y.Name;

        public override int GetHashCode()
            => GetHashCode(this);

        public int GetHashCode(Genre obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + obj.Id.GetHashCode();
                hash = hash * 23 + obj.Name.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
            => $"{Name}";
    }
}
