using System;
using Nightmares.Code.Control;
using UnityEngine;
using UnityEngine.UI;

namespace Nightmares.Code.UI
{
    public class HealthBarImages : MonoBehaviour
    {
        [SerializeField] private Image[] images;

        private void Awake()
        {
            Player.OnPlayerHealthChange += UpdBar;
        }

        private void OnDestroy()
        {
            Player.OnPlayerHealthChange -= UpdBar;
        }

        private void UpdBar(int newHealth)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < Math.Min(images.Length, newHealth); i++)
            {
                images[i].gameObject.SetActive(true);
            }
        }
    }
}
