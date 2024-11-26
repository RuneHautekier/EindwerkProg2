using FitnessBL.Model;

namespace ConsoleAppFitness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Tijdslot> tijdsloten = new List<Tijdslot>();
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

                Tijdslot tijdslot = new Tijdslot(id, uur, uur + 1, dagDeel);
                tijdsloten.Add(tijdslot);
                id++;
            }


        }
    }
}
