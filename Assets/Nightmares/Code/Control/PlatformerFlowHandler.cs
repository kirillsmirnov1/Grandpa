using System;
using Nightmares.Code.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nightmares.Code.Control
{
    public class PlatformerFlowHandler : MonoBehaviour
    {
        private void Awake()
        {
            Player.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnDestroy()
        {
            Player.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            // TODO pop-up
            this.DelayAction(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name), 1.5f);
        }
    }
}