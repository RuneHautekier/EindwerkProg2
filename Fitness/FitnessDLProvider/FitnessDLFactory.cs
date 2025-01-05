using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessDLProvider
{
    public class FitnessDLFactory
    {
        public static FitnessRepo GiveRepos(string connectionString, string repositoryType)
        {
            return new FitnessRepo(connectionString, repositoryType);
        }
    }
}
