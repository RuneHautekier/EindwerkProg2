namespace MongoDB
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var worldDAO = new WorldDAO("mongodb://localhost:27017");
            var worldManager = new WorldManager(worldDAO);

            await worldManager.GenerateAndStoreWorlds();
        }
    }
}
