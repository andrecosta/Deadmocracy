namespace Deadmocracy
{
    public class Level
    {
        public static Level CurrentLevel { get; private set; }

        public int DistanceToObjective { get; set; }
        public bool IsCompleted { get; set; }

        public Level(int distanceToObjective)
        {
            CurrentLevel = this;
            DistanceToObjective = distanceToObjective;
            IsCompleted = false;
        }
    }
}
