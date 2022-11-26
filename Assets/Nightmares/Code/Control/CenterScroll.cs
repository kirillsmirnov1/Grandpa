using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class CenterScroll : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private float speed = 1;
        [SerializeField] private float minDistPerFrame = 1;

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
            StartCoroutine(RecenterOn(0));
        }

        public void OnPointerDown()
        {
            StopAllCoroutines();
        }

        public void OnPointerUp()
        {
            Debug.Log("Pointer Up");    
            var distancesFromCenterScreen = GetElementsDistancesFromCenter();
            Debug.Log(string.Join("; ", distancesFromCenterScreen));

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

            StopAllCoroutines();
            StartCoroutine(RecenterOn(index));
        }

        private IEnumerator RecenterOn(int iElement)
        {
            var dist = GetElementsDistancesFromCenter()[iElement];
            while (Mathf.Abs(dist) > .1f)
            {
                content.transform.position -= new Vector3(dist * Time.deltaTime * speed,0,0);
                yield return null;
                dist = GetElementsDistancesFromCenter()[iElement];
            }
        }

        private float[] GetElementsDistancesFromCenter()
        {
            return _elements.Select(e => e.position.x - _xMiddle).ToArray();
        }
    }
}
