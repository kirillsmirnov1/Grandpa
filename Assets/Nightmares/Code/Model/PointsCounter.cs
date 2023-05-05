using TMPro;
using UnityEngine;

namespace Nightmares.Code.Model
{
    public class PointsCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI pointsText;
        
        public int Points
        {
            get => _points;
            private set
            {
                if(value == _points) return;
                _points = value;
                pointsText.text = _points.ToString();
            }
        }

        private int _points = int.MinValue;

        private void Awake()
        {
            Points = 0;
        }
    }
}