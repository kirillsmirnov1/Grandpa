using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nightmares.Code.UI
{
    public class HealthSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI title;

        public void Init(string newTitle, float startValue)
        {
            title.text = newTitle;
            UpdateSlider(startValue);
        }

        public void UpdateSlider(float newValue)
        {
            slider.value = newValue;
        }
    }
}
