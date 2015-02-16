namespace Deadmocracy
{
    public class Score
    {
        public int TotalXP { get; set; }
        public float TotalTime { get; set; }
        public float TotalDistance { get; set; }
        public int TotalHpLost { get; set; }
        public int Kills { get; set; }
        public int TotalScore { get; set; }
        public string PlayerName { get; set; }

        public Score()
        {
            TotalXP = 0;
            TotalTime = 0;
            TotalDistance = 0;
            TotalHpLost = 0;
            Kills = 0;
        }

        public Score(string name, int score)
            : this()
        {
            PlayerName = name;
            TotalScore = score;
        }
    }
}
