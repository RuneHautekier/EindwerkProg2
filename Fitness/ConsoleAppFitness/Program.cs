using FitnessBL.Model;

namespace ConsoleAppFitness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Time_slot> tijdsloten = new List<Time_slot>();
            int id = 1;

            for (int uur = 8; uur < 22; uur++)
            {
                string dagDeel;

                if (uur < 12)
                    dagDeel = "Voormiddag";
                else if (uur < 18)
                    dagDeel = "Namiddag";
                else
                    dagDeel = "Avond";

                Time_slot tijdslot = new Time_slot(id, uur, uur + 1, dagDeel);
                tijdsloten.Add(tijdslot);
                id++;
            }

            foreach (var tijdslot in tijdsloten)
            {
                Console.WriteLine(
                    $"Id: {tijdslot.Time_slot_id}, Start: {tijdslot.Start_time}, End: {tijdslot.End_time}, Part of Day: {tijdslot.Part_of_day}"
                );
            }
        }
    }
}
