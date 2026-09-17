using System;

namespace SpaceInvaders.Gameplay.Scoring
{
    public sealed class ScoreService
    {
        public event Action<int> ScoreChanged;

        public int CurrentScore { get; private set; }

        public void AddScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentScore += amount;
            ScoreChanged?.Invoke(CurrentScore);
        }
    }
}