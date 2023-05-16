using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nightmares.Code.UI
{
    public class CenterScroll : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private Transform content;
        [SerializeField] private float speed = 1;
        [SerializeField] private float startDelay = 1f;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float minPixDist = 10;
        
        private Transform[] _elements;
        private float _xMiddle;
        
        private void Awake()
        {
            _elements = new Transform[content.childCount];
            for (int i = 0; i < content.childCount; i++)
            {
                _elements[i] = content.GetChild(i); 
            }

            _xMiddle = Screen.width / 2f;
        }

        private void Start()
        {
            StartCoroutine(InitialCentering());
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopRecentering();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StartRecentering();
        }

        public void ScrollBy(int entriesToScroll)
        {
            var closest = FindElementClosestToCenter();
            ScrollTo(closest + entriesToScroll);
        }
        
        public void ScrollTo(int elementIndex)
        {
            if(elementIndex < 0 || elementIndex >= _elements.Length) return;
            StopAllCoroutines();
            StartCoroutine(RecenterOn(elementIndex));
        }

        private IEnumerator InitialCentering()
        {
            scrollRect.inertia = false;
            var dist = GetElementsDistancesFromCenter()[0];
            while (Mathf.Abs(dist) > .1f)
            {
                content.transform.position -= new Vector3(dist,0,0);
                yield return null;
                dist = GetElementsDistancesFromCenter()[0];
            }
            scrollRect.inertia = true;
        }

        private void StopRecentering()
        {
            StopAllCoroutines();
        }

        private void StartRecentering()
        {
            StopRecentering();
            StartCoroutine(Recenter());
        }

        public int FindElementClosestToCenter()
        {
            var distancesFromCenterScreen = GetElementsDistancesFromCenter();

            var minDist = float.PositiveInfinity;
            var index = -1;

            for (int i = 0; i < distancesFromCenterScreen.Length; i++)
            {
                var abs = Mathf.Abs(distancesFromCenterScreen[i]);
                if (abs < minDist)
                {
                    minDist = abs;
                    index = i;
                }
            }

            return index;
        }

        private IEnumerator Recenter()
        {
            yield return new WaitForSeconds(startDelay);
            var iElement = FindElementClosestToCenter();
            yield return RecenterOn(iElement);
        }

        private IEnumerator RecenterOn(int elementIndex)
        {
            var dist = GetElementsDistancesFromCenter()[elementIndex];
            while (Mathf.Abs(dist) > .1f)
            {
                var delta = dist * Time.deltaTime * speed;
                if (Mathf.Abs(dist) < minPixDist)
                {
                    delta = dist;
                    scrollRect.inertia = false;
                }

                content.transform.position -= new Vector3(delta,0,0);
                yield return null;
                dist = GetElementsDistancesFromCenter()[elementIndex];
            }

            scrollRect.inertia = true;
        }

        private float[] GetElementsDistancesFromCenter()
        {
            return _elements.Select(e => e.position.x - _xMiddle).ToArray();
        }
    }
}
