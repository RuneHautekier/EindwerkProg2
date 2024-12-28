using System;
using FitnessBL.Exceptions;

namespace FitnessBL.Model
{
    public abstract class TrainingSession
    {
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (value > DateTime.Now)
                {
                    throw new TrainingSessionException(
                        "De Date van de TrainingSession kan niet in de toekomst liggen."
                    );
                }
                date = value;
            }
        }

        private int duration;
        public int Duration
        {
            get { return duration; }
            set
            {
                if (value <= 0)
                {
                    throw new TrainingSessionException(
                        "De Duration van de TrainingSession moet groter zijn dan 0."
                    );
                }
                duration = value;
            }
        }

        private Member member;
        public Member Member
        {
            get { return member; }
            set
            {
                if (value == null)
                {
                    throw new TrainingSessionException("Member mag niet null zijn.");
                }
                member = value;
            }
        }

        // Constructor
        public TrainingSession(DateTime date, int duration, Member member)
        {
            Date = date;
            Duration = duration;
            Member = member;
        }
    }
}
