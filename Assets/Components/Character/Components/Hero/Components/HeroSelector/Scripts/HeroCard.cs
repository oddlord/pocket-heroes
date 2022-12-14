using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PocketHeroes
{
    public class HeroCard : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, ITooltipSubject
    {
        [Serializable]
        private struct _Config
        {
            public TextMeshProUGUI Name;
            public TextMeshProUGUI Level;
            public Image Background;
            public Tooltip Tooltip;
        }

        private const float _LONG_PRESS_DURATION = 3;

        private static readonly Color _UNSELECTED_COLOR = Color.white;
        private static readonly Color _SELECTED_COLOR = Color.yellow;

        [Header("Config")]
        [SerializeField] private _Config _config;

        public Action<Hero> OnPress;

        [HideInInspector] public Hero Hero;

        private float _lastPressDownTime;
        private Coroutine _longPressCoroutine;
        private bool _selected;

        public void Initialize(Hero hero, bool selected = false)
        {
            Hero = hero;
            _lastPressDownTime = 0;
            SetSelected(selected);

            _config.Name.text = hero.Name;
            // TODO replace with localized string with parameter
            _config.Level.text = $"lv. {hero.Level}";
            _config.Tooltip.Initialize(this);
        }

        public void ToggleSelected()
        {
            SetSelected(!_selected);
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
            SetBackground();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _lastPressDownTime = Time.time;

            if (_longPressCoroutine != null) StopCoroutine(_longPressCoroutine);
            _longPressCoroutine = StartCoroutine(LongPressCoroutine());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_lastPressDownTime == 0) return;

            if (_longPressCoroutine != null)
            {
                StopCoroutine(_longPressCoroutine);
                _longPressCoroutine = null;
                OnPress?.Invoke(Hero);
            }
        }

        public string[] GetTooltipRows()
        {
            return new string[]{
                $"Name: {Hero.Name}",
                $"Health: {Hero.Health}",
                $"Attack Power: {Hero.AttackPower}",
                $"Level: {Hero.Level}",
                $"Experience: {Hero.Experience}",
            };
        }

        private IEnumerator LongPressCoroutine()
        {
            yield return new WaitForSeconds(_LONG_PRESS_DURATION);
            _config.Tooltip.Show();
            _longPressCoroutine = null;
        }

        private void SetBackground()
        {
            _config.Background.color = _selected ? _SELECTED_COLOR : _UNSELECTED_COLOR;
        }
    }
}