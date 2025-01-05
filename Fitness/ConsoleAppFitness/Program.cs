using FitnessBL.Model;

namespace ConsoleAppFitness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dataLayer = ConfigurationManager.AppSettings["DataLayer"];
            string connectionString = ConfigurationManager
                .ConnectionStrings["EFconnection"]
                .ConnectionString;

            FitnessRepo repos = FitnessDLFactory.GeefRepositories(connectionString, dataLayer);
            FitnessContext ctx = new FitnessContext(connectionString);
        }
    }
}
