using TMPro;
using UnityEngine;
using DG.Tweening; // DOTween library added for the pulsing effect

public class WheelProgressView : MonoBehaviour // Managing progress texts on the wheel UI, according to the current zone and multiplier
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _txtMultiplier_value;
    [SerializeField] private TextMeshProUGUI _txtProgressInfo_value;

    [Header("Animation Settings")]
    [SerializeField] private float _pulseScale = 1.1f; // Maximum scale size for the breathing effect
    [SerializeField] private float _pulseDuration = 0.6f; // How long one pulse (in or out) takes

    private void OnDisable()
    {
        // Safely kill animations to prevent memory leaks if the panel is disabled or destroyed
        if (_txtMultiplier_value != null) _txtMultiplier_value.transform.DOKill();
    }

    public void UpdateMultiplier(int zone)
    {
        bool isActive = zone > 1;
        _txtMultiplier_value.gameObject.SetActive(isActive);

        if (isActive)
        {
            _txtMultiplier_value.text = $"Up To x{zone} Rewards";

            // Continuous breathing/pulsing animation for the multiplier text
            _txtMultiplier_value.transform.DOKill(); // Prevents animation stacking if updated rapidly
            _txtMultiplier_value.transform.localScale = Vector3.one; // Reset to default size before animating

            // Starts an infinite ping-pong (Yoyo) scaling animation
            _txtMultiplier_value.transform.DOScale(Vector3.one * _pulseScale, _pulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine); // InOutSine gives a very smooth, natural breathing feel
        }
        else
        {
            // Stops the animation and resets scale if the text is hidden (e.g., Zone 1)
            _txtMultiplier_value.transform.DOKill();
            _txtMultiplier_value.transform.localScale = Vector3.one;
        }
    }

    public void UpdateProgress(int zone, int safeInterval, int superInterval, Color silverColor, Color goldenColor)
    {
        bool isSpecial = (zone % safeInterval == 0) || (zone % superInterval == 0);
        _txtProgressInfo_value.gameObject.SetActive(!isSpecial);

        if (!isSpecial)
        {
            int toSafe = safeInterval - (zone % safeInterval);
            int toSuper = superInterval - (zone % superInterval);

            bool showSuper = toSuper <= toSafe;
            _txtProgressInfo_value.text = showSuper ? $"Spin {toSuper} times to reach GOLDEN SPIN!" : $"Spin {toSafe} times to reach SILVER SPIN!";
            _txtProgressInfo_value.color = showSuper ? goldenColor : silverColor;
        }
    }
}