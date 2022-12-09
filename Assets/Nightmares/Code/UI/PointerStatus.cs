using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nightmares.Code.UI
{
    public class PointerStatus : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool Held { get; private set; }
        public bool Down { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            Held = true;
            Down = true;
            
            StartCoroutine(RevertDown());
            IEnumerator RevertDown()
            {
                yield return null;
                Down = false;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Held = false;
        }
    }
}
