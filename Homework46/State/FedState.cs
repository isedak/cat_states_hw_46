using System;

namespace Homework46.State
{
    public class FedState : IState
    {
        private readonly Random _random = new Random();
        public string Name => "fed";

        public void Feed(Cat cat)
        {
            switch (cat.Satiety)
            {
                case < 100:
                    cat.Satiety += 15;
                    cat.Mood += 5;
                    cat.Note = $"Feeding caused +15 satiety...";
                    if (cat.Satiety > 100)
                    {
                        cat.Mood -= 30;
                        cat.Note = $"Feeding caused +15 satiety...<br>" +
                                   $"{cat.Name}'s stomach is full!";
                    }

                    break;
                case >= 100:
                    cat.Note = $"{cat.Name} doesn't want to eat!";
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
                cat.State = new PlayedState();
            }
            else
            {
                cat.Mood += 15;
                cat.Satiety -= 10;
                cat.Note = "Playing caused +15 mood...";
                cat.State = new PlayedState();
            }
        }

        public void PutToSleep(Cat cat)
        {
            cat.Note = $"{cat.Name} went to sleep...";
            cat.State = new SleepingState();
        }
    }
}