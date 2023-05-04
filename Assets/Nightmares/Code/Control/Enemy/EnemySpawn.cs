using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class EnemySpawn : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private PlatformerGameManager gameManager;
        
        public void SpawnEnemies()
        {
            var levelDimensions = gameManager.LevelDimensions;
            for (int y = 3; y < levelDimensions.y - 5; y++)
            {
                Instantiate(
                    enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                    new Vector3(Random.Range(-levelDimensions.x / 2f + 1, levelDimensions.x / 2f - 1), -y),
                    Quaternion.identity,
                    transform);
            }
        }
    }
}