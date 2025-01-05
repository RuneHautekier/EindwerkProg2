
using ConsoleAppSquareMaster;
using MongoDB;
using MongoDB.MongoDB_Model;

public class WorldManager
{
    private readonly WorldDAO worldDAO;
    private readonly World world;

    public WorldManager(WorldDAO worldDAO)
    {
        this.worldDAO = worldDAO;
        world = new World();
    }

    public async Task GenerateAndStoreWorlds()
    {
        for (int i = 1; i <= 10; i++)
        {
            int maxx = 100;
            int maxy = 100;
            bool[,] squares = (i % 2 == 0)
                ? world.BuildWorld1(maxy, maxx)
                : world.BuildWorld2(maxy, maxx, 0.7);

            double coverage = CalculateCoverage(squares);

            var worldModel = new MongoDBWorld
            {
                Name = $"World_{i}",
                Type = (i % 2 == 0) ? "Column based" : "Seed based",
                Dimensions = $"{maxx} x {maxy}",
                Coverage = coverage,
                Squares = squares
            };

            await worldDAO.SaveWorld(worldModel);
            Console.WriteLine($"World {i} saved with coverage {coverage:F2}%.");
        }
    }

    private double CalculateCoverage(bool[,] squares)
    {
        int trueCount = 0;
        int totalCount = squares.GetLength(0) * squares.GetLength(1);

        foreach (bool square in squares)
        {
            if (square) trueCount++;
        }

        return (double)trueCount / totalCount * 100;
    }
}
