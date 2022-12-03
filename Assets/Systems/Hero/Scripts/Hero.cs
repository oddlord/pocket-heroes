using System;
using UnityEngine;

namespace PocketHeroes
{
    [Serializable]
    public class Hero
    {
        private const int _EXPERIENCE_PER_LEVEL = 5;
        private const float _ATTRIBUTES_GROWTH_FACTOR_PER_LEVEL = 0.1f;

        [SerializeField] private string _name;
        public string Name { get => _name; private set { _name = value; } }

        [SerializeField] private int _health;
        public int Health { get => _health; private set { _health = value; } }

        [SerializeField] private int _attackPower;
        public int AttackPower { get => _attackPower; private set { _attackPower = value; } }

        [SerializeField] private int _experience;
        public int Experience { get => _experience; private set { _experience = value; } }

        [SerializeField] private int _level;
        public int Level { get => _level; private set { _level = value; } }

        public Action<Hero> OnChange;

        public Hero(string name, int health, int attackPower, int experience, int level)
        {
            _name = name;
            _health = health;
            _attackPower = attackPower;
            _experience = experience;
            _level = level;
        }

        public void GainExperience(int experiencePoints = 1)
        {
            _experience += experiencePoints;
            while (CanLevelUp()) LevelUp();
            OnChange?.Invoke(this);
        }

        private bool CanLevelUp() => _experience >= _EXPERIENCE_PER_LEVEL;

        private void LevelUp()
        {
            _health += (int)Math.Round(_health * _ATTRIBUTES_GROWTH_FACTOR_PER_LEVEL);
            _attackPower += (int)Math.Round(_attackPower * _ATTRIBUTES_GROWTH_FACTOR_PER_LEVEL);
            _experience -= _EXPERIENCE_PER_LEVEL;
            _level++;
        }

        public override string ToString() => JsonUtility.ToJson(this, true);
    }
}
