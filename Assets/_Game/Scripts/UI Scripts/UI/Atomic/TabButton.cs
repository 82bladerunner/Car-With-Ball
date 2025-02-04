using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterfaceAmsterdam
{
    public class TabButton : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
    {
        public event Action<TabButton> OnTabButtonEnter;
        public event Action<TabButton> OnTabButtonSelected;
        public event Action<TabButton> OnTabButtonExit;

        [field: SerializeField] public Image background { get; private set; }
        [field: SerializeField] public TextMeshProUGUI text { get; private set; }
        [field: SerializeField] public GameObject panelToOpen { get; private set; }
        [field: SerializeField] public bool isDefaultPanel { get; private set; }


        public void OnPointerEnter(PointerEventData eventData) => OnTabButtonEnter?.Invoke(this);
        public void OnPointerClick(PointerEventData eventData) => OnTabButtonSelected?.Invoke(this);
        public void OnPointerExit(PointerEventData eventData) => OnTabButtonExit?.Invoke(this);

    }
}
