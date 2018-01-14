using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi.MovieDb.TV;

namespace MovieBox.NeoModels
{
    public class Network
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Network(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Network(DM.MovieApi.MovieDb.TV.Network network)
        {
            Id = network.Id;
            Name = network.Name;
        }

        public Network()
        {

        }

        public bool Equals(Network x, Network y)
            => x.Id == y.Id && x.Name == y.Name;

        public int GetHashCode(Network obj)
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
            var network = obj as Network;
            if (network == null)
            {
                return false;
            }

            return Equals(this, network);
        }

        public override int GetHashCode()
            => GetHashCode(this);

        public override string ToString()
            => $"{Name}";
    }
}
