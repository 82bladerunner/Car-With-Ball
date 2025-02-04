using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterfaceAmsterdam
{
    public class TabGroup : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color tabIdle;
        [SerializeField] private Color tabHover;
        [SerializeField] private Color tabActive;
        [SerializeField] private Color tabTextIdle;
        [SerializeField] private Color tabTextHover;
        [SerializeField] private Color tabTextActive;

        [Header("References")]
        [SerializeField] private List<TabButton> tabButtons;

        private TabButton _selectedTab;

        private void Awake()
        {
            foreach (var tabButton in tabButtons)
            {
                tabButton.OnTabButtonEnter += OnTabEnter;
                tabButton.OnTabButtonSelected += OnTabSelected;
                tabButton.OnTabButtonExit += OnTabExit;
                if(tabButton.isDefaultPanel) OnTabSelected(tabButton);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var idx = tabButtons.FindIndex(x => x == _selectedTab);
                tabButtons[Mathf.Clamp(idx - 1, 0, tabButtons.Count - 1)].OnPointerClick(new PointerEventData(EventSystem.current));
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                var idx = tabButtons.FindIndex(x => x == _selectedTab);
                tabButtons[Mathf.Clamp(idx + 1, 0, tabButtons.Count - 1)].OnPointerClick(new PointerEventData(EventSystem.current));
            }
        }

        private void OnDestroy()
        {
            foreach (var tabButton in tabButtons)
            {
                tabButton.OnTabButtonEnter -= OnTabEnter;
                tabButton.OnTabButtonSelected -= OnTabSelected;
                tabButton.OnTabButtonExit -= OnTabExit;
            }
        }

        private void OnTabEnter(TabButton tabButton)
        {
            if (_selectedTab == tabButton) return;
            ResetTabs();
            tabButton.background.color = tabHover;
            tabButton.text.color = tabTextHover;
        }

        private void OnTabSelected(TabButton tabButton)
        {
            _selectedTab = tabButton;
            ResetTabs();
            tabButton.background.color = tabActive;
            tabButton.text.color = tabTextActive;
            tabButton.panelToOpen.SetActive(true);
        }

        private void OnTabExit(TabButton tabButton)
        {
            ResetTabs();
        }

        private void ResetTabs()
        {
            foreach (var tabTabButton in tabButtons)
            {
                if(tabTabButton == _selectedTab) continue;
                tabTabButton.background.color = tabIdle;
                tabTabButton.text.color = tabTextIdle;
                tabTabButton.panelToOpen.SetActive(false);
            }
        }

        [ContextMenu("Get Tab Buttons References")]
        private void GetTabButtonReferences()
        {
            tabButtons = new List<TabButton>(GetComponentsInChildren<TabButton>());
        }
    }
}