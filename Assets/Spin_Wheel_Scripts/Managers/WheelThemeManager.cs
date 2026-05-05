using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween library added for title pulse effect

public class WheelThemeManager : MonoBehaviour
{
    // Manages wheel visuals, texts, and text colors according to the zones

    [System.Serializable]
    public struct WheelTheme
    {
        public string title;
        public Color themeColor;
        public Sprite baseSprite;
        public Sprite indicatorSprite;
    }

    [SerializeField] private WheelTheme _bronzeTheme;
    [SerializeField] private WheelTheme _silverTheme;
    [SerializeField] private WheelTheme _goldenTheme;

    [Header("UI References")]
    [SerializeField] private Image _imgWheelBase;
    [SerializeField] private Image _imgIndicator;
    [SerializeField] private TextMeshProUGUI _txtWheelTitle;

    [Header("Juice Settings")]
    [SerializeField] private float _titlePulseScale = 1.08f; 
    [SerializeField] private float _titlePulseDuration = 0.6f; 

    private void OnDisable()
    {
        if (_txtWheelTitle != null) _txtWheelTitle.transform.DOKill();
    }

    // Theme getter for WheelUIBuilder
    public WheelTheme GetTheme(int zone, int safeInterval, int superInterval)
    {
        if (zone % superInterval == 0) return _goldenTheme;
        if (zone % safeInterval == 0) return _silverTheme;

        WheelTheme bronze = _bronzeTheme;
        bronze.title = $"BRONZE SPIN - ZONE {zone}";
        return bronze;
    }

    // Theme setter for WheelUIBuilder
    public void ApplyTheme(WheelTheme theme)
    {
        _txtWheelTitle.text = theme.title;
        _txtWheelTitle.color = theme.themeColor;
        _imgWheelBase.sprite = theme.baseSprite;
        _imgIndicator.sprite = theme.indicatorSprite;

        // Breathing animation for Special Zones (Silver and Golden)
        bool isSpecial = (theme.title == _silverTheme.title || theme.title == _goldenTheme.title);

        _txtWheelTitle.transform.DOKill();
        _txtWheelTitle.transform.localScale = Vector3.one; 

        if (isSpecial)
        {
            // Starts the scaling animation to highlight the special zone
            _txtWheelTitle.transform.DOScale(Vector3.one * _titlePulseScale, _titlePulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}