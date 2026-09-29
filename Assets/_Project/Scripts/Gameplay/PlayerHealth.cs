using System;
using SpaceInvaders.Gameplay.Combat;
using UnityEngine;

namespace SpaceInvaders.Gameplay.Player
{
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        public event Action<int> HealthChanged;
        public event Action Damaged;
        public event Action Died;

        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }

        public void Initialize(int health)
        {
            MaxHealth = health;
            CurrentHealth = health;

            HealthChanged?.Invoke(CurrentHealth);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0 || CurrentHealth <= 0)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - damage);

            HealthChanged?.Invoke(CurrentHealth);
            Damaged?.Invoke();

            if (CurrentHealth == 0)
            {
                Died?.Invoke();
            }
        }
    }
}
