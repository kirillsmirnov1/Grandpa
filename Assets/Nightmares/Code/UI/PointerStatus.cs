using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nightmares.Code.UI
{
    public class PointerStatus : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool Held { get; private set; }

        public event Action onPointerDown;

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke();
            Held = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Held = false;
        }
    }
}
