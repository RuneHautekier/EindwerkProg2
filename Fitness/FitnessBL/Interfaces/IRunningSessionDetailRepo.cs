using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessBL.Model;

namespace FitnessBL.Interfaces
{
    public interface IRunningSessionDetailRepo
    {
        IEnumerable<Runningsession_detail> GetRunningSessionDetails();
        IEnumerable<Runningsession_detail> GetRunningSessionDetailsId(int id);
        List<Runningsession_detail> AddRunningSessionDetails(List<Runningsession_detail> rsds);
    }
}
