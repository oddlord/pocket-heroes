using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PocketHeroes
{
    // TODO make a Unit superclass so that the MonsterUnit can extend it too
    // TODO make it take a Controller (input / AI)
    public class HeroUnit : MonoBehaviour
    {
        [Serializable]
        private struct _Config
        {
            public HealthBar HealthBar;
            public TextMeshPro Name;
            public CharacterTooltip Tooltip;
        }

        private const float _LONG_PRESS_DURATION = 3;
        private const float _ATTACK_MOVE_TIME = 0.5f;

        [Header("Config")]
        [SerializeField] private _Config _config;

        // TODO refactor so that this functionality is shared between Units and HeroCards
        public Action<HeroUnit> OnPress;

        public Hero Hero;
        public int CurrentHealth;

        private float _lastPressDownTime;

        public void Initialize(Hero hero)
        {
            Hero = hero;
            _lastPressDownTime = 0;

            _config.Name.text = hero.Name;
            SetCurrentHealth(hero.Health);
        }

        public void Attack(MonsterUnit enemyUnit, Action onAttackFinished)
        {
            StartCoroutine(AttackCoroutine(enemyUnit, onAttackFinished));
        }

        public void GetDamaged(int damageAmount)
        {
            SetCurrentHealth(CurrentHealth - damageAmount, true);
        }

        public bool IsDead => CurrentHealth == 0;

        private void SetCurrentHealth(int health, bool animate = false)
        {
            CurrentHealth = Math.Max(0, health);
            _config.HealthBar.SetFill(CurrentHealth, Hero.Health, !animate);
        }

        private IEnumerator AttackCoroutine(MonsterUnit enemyUnit, Action onAttackFinished)
        {
            Vector3 initialPosition = transform.position;

            float t = 0;
            while (t <= 1)
            {
                t += Time.deltaTime / _ATTACK_MOVE_TIME;
                // TODO use some nicer easing here rather than a linear one
                Vector3 position = Vector3.Lerp(initialPosition, enemyUnit.transform.position, t);
                transform.position = position;
                yield return null;
            }

            enemyUnit.GetDamaged(Hero.AttackPower);

            Vector3 attackPosition = transform.position;
            t = 0;
            while (t <= 1)
            {
                t += Time.deltaTime / _ATTACK_MOVE_TIME;
                Vector3 position = Vector3.Lerp(attackPosition, initialPosition, t);
                transform.position = position;
                yield return null;
            }

            onAttackFinished();
        }

        public void OnMouseDown()
        {
            _lastPressDownTime = Time.time;
        }

        public void OnMouseUp()
        {
            if (_lastPressDownTime == 0) return;

            if (Time.time - _lastPressDownTime < _LONG_PRESS_DURATION) OnPress?.Invoke(this);
            else _config.Tooltip.Initialize(TooltipUtils.GetHeroUnitTooltipRows(this));
        }
    }
}