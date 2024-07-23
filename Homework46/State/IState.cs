namespace Homework46.State
{
    public interface IState
    {
        public string Name { get; }

        public void Feed(Cat cat);
        public void Play(Cat cat);
        public void PutToSleep(Cat cat);
        
    }
}