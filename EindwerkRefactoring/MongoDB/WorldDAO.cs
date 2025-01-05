using MongoDB.Driver;
using MongoDB.MongoDB_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB
{
    public class WorldDAO
    {

        private IMongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<MongoDBWorld> worldCollection;
        private string connectionString;

        public WorldDAO(string connectionString)
        {
            this.connectionString = connectionString;
            client = new MongoClient(connectionString);
            database = client.GetDatabase("Worlds");
            worldCollection = database.GetCollection<MongoDBWorld>("World");
        }

        public async Task SaveWorld(MongoDBWorld world)
        {
            await worldCollection.InsertOneAsync(world);
        }
    }
}
