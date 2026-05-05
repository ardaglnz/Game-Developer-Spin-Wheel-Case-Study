using UnityEngine;
using DG.Tweening; // DOTween library
using System;
using UnityEngine.UI;

public class WheelAnimationView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform _wheelRect;
    [SerializeField] private RectTransform _indicatorPivot; 
    [SerializeField] private Image _shineEffect; 
    [SerializeField] private Image _idleShineEffect; 

    [Header("Wheel Spin Settings")]
    [SerializeField] private int _extraSpins = 5; 
    [SerializeField] private float _spinDuration = 3f;
    [SerializeField] private Ease _spinEase = Ease.OutQuart; 

    [Header("Indicator Tick Settings")]
    [SerializeField] private float _tickPunchAngle = -25f; 
    [SerializeField] private float _tickPunchDuration = 0.15f; 
    [SerializeField] private float _finalPunchAngle = -15f; 
    [SerializeField] private float _finalPunchDuration = 0.2f; 

    [Header("Reward Shine Effect Settings")]
    [SerializeField] private float _rewardShineTargetScale = 1.5f; 
    [SerializeField] private float _rewardShineScaleDuration = 0.5f; 
    [SerializeField] private float _rewardShineRotationDuration = 6f; 
    [SerializeField] private float _rewardShineHideDuration = 0.3f; 

    [Header("Idle Shine Settings")]
    [SerializeField] private float _idleShineTargetAlpha = 0.6f; 
    [SerializeField] private float _idleShineFadeDuration = 1.2f;


    private const float SLICE_ANGLE = 45f;
    private int _lastPassedSlice = 0;

    private void Awake() // Hides and shrinks the shine effects at the beginning
    {
        if (_shineEffect != null)
        {
            _shineEffect.gameObject.SetActive(false);
            _shineEffect.transform.localScale = Vector3.zero;
        }

        if (_idleShineEffect != null)
        {
            _idleShineEffect.gameObject.SetActive(false);
            _idleShineEffect.transform.localScale = Vector3.zero;
        }
    }

    private void Start()
    {
        PlayIdleShine();
    }

    private void OnEnable()
    {
        // Listening the spin request event
        WheelSpinEvent.OnSpinRequested += HandleSpinRequested;
        GameSignals.OnRewardCollected += PlayShineEffect;
        GameSignals.OnSpinRequested += HideShineEffect;
        GameSignals.OnGameRestarted += PlayIdleShine; 
    }

    private void OnDisable()
    {
        WheelSpinEvent.OnSpinRequested -= HandleSpinRequested;
        GameSignals.OnRewardCollected -= PlayShineEffect;
        GameSignals.OnSpinRequested -= HideShineEffect;
        GameSignals.OnGameRestarted -= PlayIdleShine;
    }

    private void HandleSpinRequested(WheelType type, int targetIndex, Action onComplete)
    {
        // Hides the idle shine effect forever (until restart) on the very first spin
        HideIdleShine();

        // Resetting the wheel's angle for avoiding mathematical deflections
        _wheelRect.localEulerAngles = Vector3.zero;
        _lastPassedSlice = 0; // Resetting the tick tracker for the new spin

        // Kills any lingering indicator animations and forces it to default rotation
        if (_indicatorPivot != null)
        {
            _indicatorPivot.DOKill();
            _indicatorPivot.localEulerAngles = Vector3.zero;
        }

        // Calculating the clockwise angle (negative) that reaches the target slice to the top (0 point)
        float targetAngle = (targetIndex == 0) ? 0 : -(360f - (targetIndex * SLICE_ANGLE));

        // Total rotation calculation based on inspector variables
        float totalRotation = -(360f * _extraSpins) + targetAngle;

        _wheelRect.DORotate(new Vector3(0, 0, totalRotation), _spinDuration, RotateMode.FastBeyond360)
            .SetEase(_spinEase)
            .OnUpdate(TickIndicator) 
            .OnComplete(() => {

                if (_indicatorPivot != null)
                {
                    _indicatorPivot.DOKill();
                    _indicatorPivot.localEulerAngles = Vector3.zero;
                    _indicatorPivot.DOPunchRotation(new Vector3(0, 0, _finalPunchAngle), _finalPunchDuration, 1, 0.5f);
                }

                Debug.Log("<color=orange>[STEP 1]</color> DOTween bitti, callback fýrlatýlýyor.");
                onComplete?.Invoke();
            });
    }

    private void TickIndicator() // Creates a ticking effect on the indicator while the wheel is spinning
    {
        if (_indicatorPivot == null) return;

        float currentAngle = Mathf.Abs(_wheelRect.localEulerAngles.z);
        int currentSlice = Mathf.FloorToInt(currentAngle / SLICE_ANGLE);

        if (currentSlice != _lastPassedSlice)
        {
            _lastPassedSlice = currentSlice;

            _indicatorPivot.DOKill();
            _indicatorPivot.localEulerAngles = Vector3.zero;
            _indicatorPivot.DOPunchRotation(new Vector3(0, 0, -_tickPunchAngle), _tickPunchDuration, 1, 0.5f);
        }
    }


    private void PlayIdleShine() // Displays the breathing shine effect before the first spin
    {
        if (_idleShineEffect == null) return;

        if (_wheelRect != null) _wheelRect.localEulerAngles = Vector3.zero;
        if (_indicatorPivot != null) _indicatorPivot.localEulerAngles = Vector3.zero;

        _idleShineEffect.DOKill();
        _idleShineEffect.transform.DOKill();

        _idleShineEffect.gameObject.SetActive(true);
        _idleShineEffect.transform.localScale = Vector3.zero;

        Color c = _idleShineEffect.color;
        c.a = 0f;
        _idleShineEffect.color = c;

        _idleShineEffect.transform.DOScale(Vector3.one * _rewardShineTargetScale, _rewardShineScaleDuration).SetEase(Ease.OutBack);

        _idleShineEffect.DOFade(_idleShineTargetAlpha, _idleShineFadeDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void HideIdleShine() // Shrinks and hides the idle shine effect when spun
    {
        if (_idleShineEffect != null && _idleShineEffect.gameObject.activeSelf)
        {
            _idleShineEffect.DOKill();
            _idleShineEffect.transform.DOKill();

            _idleShineEffect.DOFade(0f, _rewardShineHideDuration);
            _idleShineEffect.transform.DOScale(Vector3.zero, _rewardShineHideDuration).SetEase(Ease.InBack).OnComplete(() => {
                _idleShineEffect.gameObject.SetActive(false);
            });
        }
    }

    private void PlayShineEffect(RewardData data) // Displays and animates the shine effect when a reward is collected
    {
        if (_shineEffect == null) return;

        _shineEffect.gameObject.SetActive(true);
        _shineEffect.transform.localScale = Vector3.zero;

        _shineEffect.transform.DOScale(Vector3.one * _rewardShineTargetScale, _rewardShineScaleDuration).SetEase(Ease.OutBack);
        _shineEffect.transform.DORotate(new Vector3(0, 0, -360f), _rewardShineRotationDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private void HideShineEffect() // Shrinks and hides the shine effect when a new spin starts
    {
        if (_shineEffect == null) return;

        _shineEffect.transform.DOKill();
        _shineEffect.transform.DOScale(Vector3.zero, _rewardShineHideDuration).SetEase(Ease.InBack).OnComplete(() => {
            _shineEffect.gameObject.SetActive(false);
        });
    }
}