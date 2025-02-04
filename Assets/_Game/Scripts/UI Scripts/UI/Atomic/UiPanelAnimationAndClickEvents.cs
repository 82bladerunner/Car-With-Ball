using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterfaceAmsterdam
{
    public class UiPanelAnimationAndClickEvents : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
    {
        public event Action OnClick;
        [SerializeField] private Color _hoveredColour;
        [SerializeField] private Color _defaultColour;
        [SerializeField] private Image _backGround;
        [SerializeField] private float delayMultiplier;
        private RectTransform rectTransform => transform as RectTransform; 
        private void OnEnable()
        {
            var originalScale = rectTransform.sizeDelta.magnitude;
            rectTransform.localScale = Vector3.zero; 
            rectTransform.DOScale(Vector3.one, 0.1f).SetDelay(originalScale / delayMultiplier);
        }

        public void OnPointerEnter(PointerEventData eventData) => _backGround.color = _hoveredColour;
        public void OnPointerClick(PointerEventData eventData) => OnClick?.Invoke();
        public void OnPointerExit(PointerEventData eventData) => _backGround.color = _defaultColour;
    }
}

