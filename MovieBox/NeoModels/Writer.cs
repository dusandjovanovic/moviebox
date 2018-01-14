using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBox.NeoModels
{
    public class Writer
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

        public Writer(DM.MovieApi.MovieDb.Movies.MovieCrewMember writer)
        {
            PersonId = writer.PersonId;
            CreditId = writer.CreditId;
            Department = writer.Department;
            Job = writer.Job;
            Name = writer.Name;
            ProfilePath = writer.ProfilePath;
        }

        public Writer()
        {

        }
    }
}
