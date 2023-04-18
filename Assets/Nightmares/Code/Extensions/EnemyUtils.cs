using Nightmares.Code.Control;
using Nightmares.Code.Model;
using UnityEngine;

namespace Nightmares.Code.Extensions
{
    public static class EnemyUtils
    {
        public static bool CheckEnemySeesPlayer(Vector3 pos)
        {
            var visible = true;
            var toPlayer = Player.Instance.transform.position - pos;

            var hits = Physics2D.RaycastAll(pos, toPlayer, toPlayer.magnitude);
            foreach (var hit in hits)
            {
                var layer = hit.transform.gameObject.layer;
                if (layer is Constants.LayerEnemy or Constants.LayerPlayer)
                {
                    continue;
                }

                visible = false;
                break;
            }

            return visible;
        }
    }
}