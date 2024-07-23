namespace Homework46.State
{
    public class SleepingState : IState
    {
        public string Name => "slept";

        public void Feed(Cat cat)
        {
            if (cat.Mood < -15)
            {
                cat.Note = $"Dead {cat.Name} can not eat!";
            }
            else
            {
                cat.Note = $"{cat.Name} is sleeping!";
            }
        }

        public void Play(Cat cat)
        {
            if (cat.Mood < -15)
            {
                cat.Note = $"{cat.Name} will never wake up...";
                cat.State = new SleepingState();
            }
            else
            {
                cat.Mood -= 5;
                if (cat.Mood < -15)
                {
                    cat.Note = $"{cat.Name} will never wake up...";
                    cat.State = new SleepingState();
                }
                else
                {
                    cat.Note = $"{cat.Name} waked up...";
                    cat.State = new PlayedState();
                }
            }
        }

        public void PutToSleep(Cat cat)
        {
            if (cat.Mood < -15)
            {
                cat.Note = $"H-m... Rest in peace.";
            }
            else
            {
                cat.Note = $"{cat.Name} is sleeping already!";
            }
        }
    }
}