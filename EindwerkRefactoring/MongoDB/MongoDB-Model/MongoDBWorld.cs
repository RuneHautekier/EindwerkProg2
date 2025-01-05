using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.MongoDB_Model
{
    public class MongoDBWorld
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Dimensions { get; set; }
        public double Coverage { get; set; }
        public bool[,] Squares { get; set; }
    }
}
