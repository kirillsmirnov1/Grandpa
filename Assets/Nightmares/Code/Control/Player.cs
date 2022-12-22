using UnityEngine;

namespace Nightmares.Code.Control
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}