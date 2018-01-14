using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi.MovieDb.Movies;

namespace MovieBox.NeoModels
{
    public class Actor
    {
        public int PersonId { get; set; }

        public int CastId { get; set; }

        public string CreditId { get; set; }

        public string Character { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public string ProfilePath { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }

        public Actor(DM.MovieApi.MovieDb.Movies.MovieCastMember actor)
        {
            PersonId = actor.PersonId;
            CastId = actor.CastId;
            CreditId = actor.CreditId;
            Character = actor.Character;
            Name = actor.Name;
            Order = actor.Order;
            ProfilePath = actor.ProfilePath;
        }


        public Actor()
        {

        }
    }
}
