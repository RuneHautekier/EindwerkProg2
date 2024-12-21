using FitnessEF;

namespace ConsoleAppDatabaseEF
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                @"Data Source=RUNE\SQLEXPRESS;Initial Catalog=GymTest;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

            // Initialiseer de context met de gekozen connectionString
            FitnessContext ctx = new FitnessContext(connectionString);

            // Databasebeheer
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            Console.WriteLine("Database is opnieuw aangemaakt.");
        }
    }
}
