using System;
using System.Collections;
using UnityEngine;

namespace Nightmares.Code.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void DelayAction(this MonoBehaviour ctx, Action action, float delay)
        {
            ctx.StartCoroutine(DelayCoroutine());

            IEnumerator DelayCoroutine()
            {
                yield return new WaitForSeconds(delay);
                action?.Invoke();
            }
        }
    }
}