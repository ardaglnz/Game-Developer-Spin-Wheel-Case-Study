using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween library

public class BombPopupView : MonoBehaviour // View method that prepares the Bomb Panel
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform panelRect; 
    [SerializeField] private Image bombShineEffect; 
    [SerializeField] private Image bombIconImage;
    [SerializeField] private Button btnReviveGold;
    [SerializeField] private Button btnReviveAd;
    [SerializeField] private Button btnGiveUp;

    [Header("Settings")]
    [SerializeField] private TransactionService _transactionService;
    [SerializeField] private EconomyService _economyService;
    [SerializeField] private int reviveCost = 25; // Revive cost (in golds)

    [Header("Juice Settings (Panel & Buttons)")]
    [SerializeField] private float _panelPopDuration = 0.4f;
    [SerializeField] private Ease _panelPopEase = Ease.OutBack;
    [SerializeField] private float _btnPulseScale = 1.05f;
    [SerializeField] private float _btnPulseDuration = 0.6f;
    [SerializeField] private float _bombPulseScale = 1.15f; 
    [SerializeField] private float _bombPulseDuration = 0.5f;

    [Header("Juice Settings (Shine)")]
    [SerializeField] private float _shineTargetScale = 1.5f;
    [SerializeField] private float _shineScaleDuration = 0.5f;
    [SerializeField] private float _shineRotationDuration = 6f;
    [SerializeField] private float _shineHideDuration = 0.3f;

    private void Awake()
    {
        // Immediately disable at startup without animation
        panel.SetActive(false);
        if (panelRect != null) panelRect.localScale = Vector3.zero;

        if (bombShineEffect != null)
        {
            bombShineEffect.gameObject.SetActive(false);
            bombShineEffect.transform.localScale = Vector3.zero;
        }

        btnReviveGold.onClick.AddListener(OnReviveGoldClicked);
        btnReviveAd.onClick.AddListener(OnReviveAdClicked);
        btnGiveUp.onClick.AddListener(OnGiveUpClicked);
    }

    private void OnEnable()
    {
        GameSignals.OnBombExploded += Show;
        GameSignals.OnRevived += Hide;
        GameSignals.OnGameRestarted += Hide;
    }

    private void OnDisable()
    {
        GameSignals.OnBombExploded -= Show;
        GameSignals.OnRevived -= Hide;
        GameSignals.OnGameRestarted -= Hide;
    }

    public void Show()
    {
        panel.SetActive(true);

        // Panel pop-in animation
        if (panelRect != null)
        {
            panelRect.DOKill();
            panelRect.localScale = Vector3.zero;
            panelRect.DOScale(Vector3.one, _panelPopDuration).SetEase(_panelPopEase);
        }

        // Button breathing animations
        PlayButtonPulse(btnReviveGold);
        PlayButtonPulse(btnReviveAd);

        // Bomb icon breathing animation
        if (bombIconImage != null)
        {
            bombIconImage.transform.DOKill();
            bombIconImage.transform.localScale = Vector3.one;
            bombIconImage.transform.DOScale(Vector3.one * _bombPulseScale, _bombPulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        // Bomb shine effect
        if (bombShineEffect != null)
        {
            bombShineEffect.DOKill();
            bombShineEffect.transform.DOKill();
            bombShineEffect.gameObject.SetActive(true);
            bombShineEffect.transform.localScale = Vector3.zero;

            bombShineEffect.transform.DOScale(Vector3.one * _shineTargetScale, _shineScaleDuration).SetEase(Ease.OutBack);
            bombShineEffect.transform.DORotate(new Vector3(0, 0, -360f), _shineRotationDuration, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        }
    }

    public void Hide()
    {
        // Stop button animations
        btnReviveGold.transform.DOKill();
        btnReviveAd.transform.DOKill();
        if (bombIconImage != null) bombIconImage.transform.DOKill();

        if (bombShineEffect != null)
        {
            bombShineEffect.transform.DOKill();
            bombShineEffect.transform.DOScale(Vector3.zero, _shineHideDuration).SetEase(Ease.InBack).OnComplete(() => {
                bombShineEffect.gameObject.SetActive(false);
            });
        }

        // Panel shrink-out animation
        if (panelRect != null && panel.activeSelf)
        {
            panelRect.DOKill();
            panelRect.DOScale(Vector3.zero, _panelPopDuration * 0.8f).SetEase(Ease.InBack).OnComplete(() => {
                panel.SetActive(false);
            });
        }
        else
        {
            panel.SetActive(false);
        }
    }

    private void PlayButtonPulse(Button btn)
    {
        if (btn == null) return;

        btn.transform.DOKill();
        btn.transform.localScale = Vector3.one;
        btn.transform.DOScale(Vector3.one * _btnPulseScale, _btnPulseDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnReviveGoldClicked()
    {
        if (_economyService.TrySpendGold(reviveCost))
        {
            Hide();
            GameSignals.OnRevived?.Invoke();
        }
        else
        {
            Debug.LogWarning("Insufficent Gold!");
        }
    }

    private void OnReviveAdClicked()
    {
        GameSignals.OnRevived?.Invoke();
    }

    private void OnGiveUpClicked()
    {
        _transactionService.AbandonLoot();
        Hide();
    }

    private void OnDestroy()
    {
        btnReviveGold.onClick.RemoveAllListeners();
        btnReviveAd.onClick.RemoveAllListeners();
        btnGiveUp.onClick.RemoveAllListeners();

        // Safely kill tweens to prevent memory leaks
        if (panelRect != null) panelRect.DOKill();
        if (btnReviveGold != null) btnReviveGold.transform.DOKill();
        if (btnReviveAd != null) btnReviveAd.transform.DOKill();
        if (bombIconImage != null) bombIconImage.transform.DOKill();
        if (bombShineEffect != null) bombShineEffect.transform.DOKill();
    }
}