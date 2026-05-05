using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween library added for UI animations

public class MainHUDView : MonoBehaviour // View method that prepares the Main (Wheel) Panel
{
    [Header("Config")]
    [SerializeField] private ZoneConfigSO _zoneConfig;

    [Header("Buttons")]
    [SerializeField] private Button btnSpin;
    [SerializeField] private Button btnLeave;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI txtGoldAmount_value;

    [Header("Service Reference")]
    [SerializeField] private TransactionService _transactionService;

    [Header("Juice Settings")]
    [SerializeField] private float _spinBtnPulseScale = 1.05f; // Max scale for the breathing effect
    [SerializeField] private float _spinBtnPulseDuration = 0.8f; // Duration of one pulse

    private void Awake()
    {
        if (_transactionService == null)
            _transactionService = FindObjectOfType<TransactionService>();

        btnSpin.onClick.AddListener(() => GameSignals.OnSpinRequested?.Invoke());
        btnLeave.onClick.AddListener(ProcessExit);
    }

    private void OnEnable()
    {
        ZoneManager.OnZoneAdvanced += HandleZoneChange;
        GameSignals.OnBalanceChanged += UpdateGoldUI;
        GameSignals.OnGameRestarted += ResetUI;
        GameStateManager.OnStateChanged += HandleStateChanged;
        InitializeUI();
    }

    private void OnDisable()
    {
        ZoneManager.OnZoneAdvanced -= HandleZoneChange;
        GameSignals.OnBalanceChanged -= UpdateGoldUI;
        GameSignals.OnGameRestarted -= ResetUI;
        GameStateManager.OnStateChanged -= HandleStateChanged;
    }

    private void OnDestroy()
    {
        btnSpin.onClick.RemoveAllListeners();
        btnLeave.onClick.RemoveAllListeners();

        // Safely kill the tween when the object is destroyed to prevent memory leaks
        if (btnSpin != null) btnSpin.transform.DOKill();
    }

    // Locks the interactable buttons while the wheel is actively spinning or during other states
    private void HandleStateChanged(GameState newState)
    {
        bool isIdle = (newState == GameState.Idle);

        btnSpin.interactable = isIdle;
        btnLeave.interactable = isIdle;

        // Idle animation for the Spin button
        if (isIdle)
        {
            btnSpin.transform.DOKill();
            btnSpin.transform.localScale = Vector3.one; // Reset to default size before animating

            // Starts an infinite scaling animation
            btnSpin.transform.DOScale(Vector3.one * _spinBtnPulseScale, _spinBtnPulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine); // Smooth in and out for a natural breathing feel
        }
        else
        {
            // Kills the breathing animation immediately when the player interacts
            btnSpin.transform.DOKill();
            btnSpin.transform.localScale = Vector3.one;
        }
    }

    private void InitializeUI()
    {
        if (EconomyService.Instance != null)
            UpdateGoldUI(EconomyService.Instance.GetBalance());

        HandleStateChanged(GameState.Idle);
    }

    // Evaluates if the current zone allows the player to exit and keep their loot
    private void HandleZoneChange(int currentZone)
    {
        bool canLeave = currentZone % _zoneConfig.safeZoneInterval_value == 0
                     || currentZone % _zoneConfig.superZoneInterval_value == 0;
        ToggleLeaveButton(canLeave);
    }

    // Refreshes the gold display text according to the player's balance changes
    private void UpdateGoldUI(int currentBalance)
    {
        if (txtGoldAmount_value != null)
            txtGoldAmount_value.text = currentBalance.ToString();
    }

    private void ResetUI()
    {
        HandleZoneChange(1);
        InitializeUI();
    }

    public void ToggleLeaveButton(bool canLeave) =>
        btnLeave.gameObject.SetActive(canLeave);

    // Commits their loot to the permanent inventory and closes the application
    private void ProcessExit()
    {
        if (_transactionService != null)
            _transactionService.CommitLoot();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}