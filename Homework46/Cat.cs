using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Homework46.State;

namespace Homework46
{
    public class Cat
    {
        private readonly Random _random = new Random();

        private readonly List<string> _avatars = new()
        {
            "img/cat-0.svg",
            "img/cat-1.svg",
            "img/cat-2.svg",
            "img/cat-3.svg",
            "img/cat-4.svg",
            "img/cat-0-1-2-s.svg",
            "img/cat-3-4-s.svg",
            "img/cat-5.svg"
        };

        public string Name { get; set; }
        public int Age { get; set; }
        public int Satiety { get; set; }
        public int Mood { get; set; }
        public string StateName { get; set; }
        [JsonIgnore] public IState State { get; set; }
        public string Note { get; set; }
        public string AvatarNote { get; set; }
        public static List<string> Avatars { get; set; }
        public string AvatarPath { get; set; }

        public Cat(string name, int satiety, int mood, IState state)
        {
            Name = name;
            Age = _random.Next(1, 9);
            Satiety = satiety;
            Mood = mood;
            State = state;
            StateName = State.Name;
            Avatars = _avatars;
            AvatarPath = ShowCatStateAvatarPath();
            Note = "...";
        }

        public Cat()
        {
            State = SetState(StateName);
        }

        public void Feed()
        {
            State.Feed(this);
            StateName = State.Name;
            AvatarPath = ShowCatStateAvatarPath();
        }

        public void Play()
        {
            State.Play(this);
            StateName = State.Name;
            AvatarPath = ShowCatStateAvatarPath();
        }

        public void PutToSleep()
        {
            State.PutToSleep(this);
            StateName = State.Name;
            AvatarPath = ShowCatStateAvatarPath();
        }

        public static IState SetState(string name)
        {
            return name switch
            {
                "played" => new PlayedState(),
                "fed" => new FedState(),
                _ => new SleepingState()
            };
        }

        public string ShowCatStateAvatarPath()
        {
            if (Mood < -5 && Satiety < -10 || Satiety < -15 || Mood < -15)
            {
                AvatarNote = $"{Name} is died...";
                return Avatars[7];
            }
            switch (StateName)
            {
                case "slept":
                    switch (Mood)
                    {
                        case < -15 :
                            AvatarNote = $"{Name} is died...";
                            return Avatars[7];
                        case >= -15 and <=49:
                            AvatarNote = $"{Name} is tormented by nightmares...";
                            return Avatars[5];
                        default:
                            AvatarNote = $"{Name} sleeps well.";
                            return Avatars[6];
                    }
                default:
                    switch (Mood)
                    {
                        case <=0:
                            AvatarNote = $"{Name} is furious!";
                            return Avatars[0];
                        case > 0 and <= 25:
                            AvatarNote = $"{Name} is in a bad mood.";
                            return Avatars[1];
                        case > 25 and <= 49:
                            AvatarNote = $"{Name} needs something...";
                            return Avatars[2];
                        case > 49 and <= 90:
                            AvatarNote = $"{Name} feels good!";
                            return Avatars[3];
                        case > 90:
                            AvatarNote = $"{Name} is happy!";
                            return Avatars[4];
                    }
            }
        }
    }
}