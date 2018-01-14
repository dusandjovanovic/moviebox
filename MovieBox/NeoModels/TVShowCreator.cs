using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi.MovieDb.TV;

namespace MovieBox.NeoModels
{
    public class TVShowCreator
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ProfilePath { get; set; }

        public TVShowCreator(int id, string name, string profilePath)
        {
            Id = id;
            Name = name;
            ProfilePath = profilePath;
        }

        public TVShowCreator(DM.MovieApi.MovieDb.TV.TVShowCreator creator)
        {
            Id = creator.Id;
            Name = creator.Name;
            ProfilePath = creator.ProfilePath;
        }

        public TVShowCreator()
        {

        }

        public bool Equals(TVShowCreator x, TVShowCreator y)
            => x.Id == y.Id && x.Name == y.Name;

        public int GetHashCode(TVShowCreator obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + obj.Id.GetHashCode();
                hash = hash * 23 + obj.Name.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var showCreator = obj as TVShowCreator;
            if (showCreator == null)
            {
                return false;
            }

            return Equals(this, showCreator);
        }

        public override int GetHashCode()
            => GetHashCode(this);

        public override string ToString()
=> $"{Name} ({Id})";
    }
}
