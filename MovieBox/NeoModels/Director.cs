using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi.MovieDb.Movies;

namespace MovieBox.NeoModels
{
    public class Director
    {
        public int PersonId { get; set; }

        public string CreditId { get; set; }

        public string Department { get; set; }

        public string Job { get; set; }

        public string Name { get; set; }

        public string ProfilePath { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }

        public Director(DM.MovieApi.MovieDb.Movies.MovieCrewMember director)
        {
            PersonId = director.PersonId;
            CreditId = director.CreditId;
            Department = director.Department;
            Job = director.Job;
            Name = director.Name;
            ProfilePath = director.ProfilePath;
        }


        public Director()
        {

        }
    }
}
