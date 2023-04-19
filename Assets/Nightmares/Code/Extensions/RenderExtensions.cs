using System;
using UnityEngine;

namespace Nightmares.Code.Extensions
{
    [RequireComponent(typeof(Renderer))]
    public class RenderExtensions : MonoBehaviour
    {
        public event Action onBecameVisible;
        
        private void OnBecameVisible() => onBecameVisible?.Invoke();
    }
}