using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween library added for the pop-in effect

public class RewardDisplayView : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private RewardSlotUI slotPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private InventoryService _inventory;
    [SerializeField] private ZoneConfigSO _zoneConfig;

    [Header("Juice Settings (Items)")]
    [SerializeField] private float _popDuration = 0.4f; // Duration of the pop-in animation for slots
    [SerializeField] private float _popDelay = 0.1f;    // Delay between each reward card's appearance
    [SerializeField] private Ease _popEase = Ease.OutBack; // The bouncy easing type for pop-in

    [Header("Juice Settings (Panel)")]
    [SerializeField] private float _panelPopDuration = 0.4f; // Panel opening duration
    [SerializeField] private Ease _panelPopEase = Ease.OutBack; // Panel opening ease

    private readonly List<RewardSlotUI> _activeSlots = new();

    private void OnEnable()
    {
        // Connected to the signal system
        GameSignals.OnRewardCollected += HandleRewardCollected;
        GameSignals.OnGameRestarted += ClearDisplay;
        GameSignals.OnBombExploded += () => SetPanelActive(true);
        GameSignals.OnRevived += HandleRevive;

        // Zone advancement is still listened (for display)
        ZoneManager.OnZoneAdvanced += HandleZoneVisibility;
        GameStateManager.OnStateChanged += HandleStateVisibility;
    }

    private void OnDisable()
    {
        GameSignals.OnRewardCollected -= HandleRewardCollected;
        GameSignals.OnGameRestarted -= ClearDisplay;
        GameSignals.OnRevived -= HandleRevive;
        ZoneManager.OnZoneAdvanced -= HandleZoneVisibility;
        GameStateManager.OnStateChanged -= HandleStateVisibility;
    }

    // Pulls the curent Inventory from InventoryService when the new reward occures
    private void HandleRewardCollected(RewardData data) =>
       RefreshDisplay(_inventory.GetAll());

    public void RefreshDisplay(List<RewardData> currentLoot) // Refreshes the display when a new reward is obtained
    {
        ClearDisplay();

        for (int i = 0; i < currentLoot.Count; i++)
        {
            var data = currentLoot[i];
            RewardSlotUI newSlot = Instantiate(slotPrefab, container);
            newSlot.Setup(data);
            _activeSlots.Add(newSlot);

            // Pop-in animation for each card 
            newSlot.transform.localScale = Vector3.zero;

            float delay = i * _popDelay;
            newSlot.transform.DOScale(Vector3.one, _popDuration)
                .SetDelay(delay)
                .SetEase(_popEase);
        }
    }

    public void ClearDisplay() // Clears the display when the game restarts or refreshes
    {
        foreach (var slot in _activeSlots)
        {
            if (slot != null)
            {
                // Safely kill active tweens before destroying the object to prevent memory leaks
                slot.transform.DOKill();
                Destroy(slot.gameObject);
            }
        }
        _activeSlots.Clear();
    }

    private void HandleRevive()
    {
        SetPanelActive(false);
    }

    private void HandleZoneVisibility(int currentZone) // Safe Zone or Super Zone control
    {
        bool isSafeZone = (currentZone % _zoneConfig.safeZoneInterval_value == 0) || (currentZone % _zoneConfig.superZoneInterval_value == 0);
        SetPanelActive(isSafeZone);
    }

    // Panel controls according to the GameState changes
    private void HandleStateVisibility(GameState newState)
    {
        switch (newState)
        {
            case GameState.Spinning:
                SetPanelActive(false); // Close smoothly while the wheel is spinning
                break;
            case GameState.BombResult:
            case GameState.LeavePrompt:
                SetPanelActive(true); // Open smoothly when the bomb explodes or the "Leaving" option enabled
                break;
        }
    }

    //  Animates the panel to scale up smoothly when opened, and shrink down when closed
    private void SetPanelActive(bool isActive)
    {
        if (displayPanel == null) return;

        displayPanel.transform.DOKill();

        if (isActive)
        {
            if (!displayPanel.activeSelf)
            {
                // Start from zero if it was completely closed
                displayPanel.transform.localScale = Vector3.zero;
                displayPanel.SetActive(true);
            }
            // Scale up to normal size
            displayPanel.transform.DOScale(Vector3.one, _panelPopDuration).SetEase(_panelPopEase);
        }
        else
        {
            if (displayPanel.activeSelf)
            {
                // Shrink down and then disable the game object
                displayPanel.transform.DOScale(Vector3.zero, _panelPopDuration * 0.8f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => displayPanel.SetActive(false));
            }
        }
    }
}