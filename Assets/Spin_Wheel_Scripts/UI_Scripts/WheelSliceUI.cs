using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WheelSliceUI : MonoBehaviour
{
    // Vertigo kuralý: Deðiþen UI elementleri "_value" ile bitmeli[cite: 1]
    [SerializeField] private Image _imgIcon_value;
    [SerializeField] private TextMeshProUGUI _txtAmount_value;

    public void Setup(SliceData data, int currentZone)
    {
        if (data == null || data.RewardConfig == null) return;

        // Ýkonu ata
        _imgIcon_value.sprite = data.RewardConfig.icon_value;

        // Eðer dilim bir bomba ise altýndaki yazýyý boþ býrakýyoruz
        if (data.IsBomb)
        {
            _txtAmount_value.text = "";
        }
        else
        {
            int totalAmount = data.RewardConfig.baseValue_value * currentZone;

            // Referans görseldeki gibi binlik deðerleri "K" ile formatla
            if (totalAmount >= 1000)
            {
                float kAmount = totalAmount / 1000f;
                // "0.#" formatý sayesinde 1500 gelirse "1.5K", 1000 gelirse "1K" yazar
                _txtAmount_value.text = $"x{kAmount:0.#}K";
            }
            else
            {
                // Artýk miktar 1 bile olsa ekrana "x1" yazdýracak
                _txtAmount_value.text = $"x{totalAmount}";
            }
        }
    }
}