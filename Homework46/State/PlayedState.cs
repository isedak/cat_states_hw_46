using System;

namespace Homework46.State
{
    public class PlayedState : IState
    {
        private readonly Random _random = new Random();
        public string Name => "played";

        public void Feed(Cat cat)
        {
            switch (cat.Satiety)
            {
                case < 100:
                    cat.Satiety += 15;
                    cat.Mood += 5;
                    cat.Note = "Feeding caused +15 satiety...";
                    cat.State = new FedState();
                    if (cat.Satiety > 100)
                    {
                        cat.Mood -= 30;
                        cat.Note = $"Feeding caused +15 satiety...<br>" +
                                   $"{cat.Name}'s stomach is full!";
                        cat.State = new FedState();
                    }

                    break;
                case >= 100:
                    cat.Note = $"{Name} doesn't want to eat!";
                    break;
            }
        }

        public void Play(Cat cat)
        {
            int number = _random.Next(1, 4);
            if (number == 1)
            {
                cat.Mood = 0;
                cat.Satiety -= 10;
                cat.Note = $"Ups!";
            }
            else
            {
                cat.Mood += 15;
                cat.Satiety -= 10;
                cat.Note = $"Playing caused +15 mood...";
            }
        }

        public void PutToSleep(Cat cat)
        {
            cat.Note = $"{cat.Name} went to sleep...";
            cat.State = new SleepingState();
        }
    }
}