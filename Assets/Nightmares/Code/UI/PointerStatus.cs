using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nightmares.Code.UI
{
    public class PointerStatus : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action onPointerDown;
        
        public bool Held { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            Held = true;
            onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Held = false;
        }
    }
}
