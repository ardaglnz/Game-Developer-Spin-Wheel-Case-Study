using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardSlotUI : MonoBehaviour // Setup for implementing ui elements for each reward
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TextMeshProUGUI txtAmount_value;
    [SerializeField] private TextMeshProUGUI txtName_value;

    public void Setup(RewardData data)
    {
        if (imgIcon != null) imgIcon.sprite = data.Icon;
        if (txtAmount_value != null) txtAmount_value.text = $"x{data.Amount}";
        if (txtName_value != null) txtName_value.text = data.RewardId.Replace("_", " ").ToUpper();
    }
}