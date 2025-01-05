namespace MongoDB
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            WorldDAO worldDAO = new WorldDAO("mongodb://localhost:27017");
            WorldManager worldManager = new WorldManager(worldDAO);

            await worldManager.GenerateAndStoreWorlds();
        }
    }
}
