using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class EnemySpawn : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private int[] spawnCost;
        [SerializeField] private AnimationCurve budget;
        [SerializeField] private PlatformerGameManager gameManager;

        private float _balance;
        
        public void SpawnEnemies()
        {
            var levelDimensions = gameManager.LevelDimensions;
            float yLimit = levelDimensions.y;
            for (float y = 0; y < yLimit; y++)
            {
                _balance += budget.Evaluate(y / yLimit);
                while (_balance >= 1)
                {
                    var enemyIndex = Random.Range(0, enemyPrefabs.Length);
                    _balance -= spawnCost[enemyIndex];
                    SpawnEnemy(enemyIndex, -y);
                }
            }

            void SpawnEnemy(int enemyIndex, float yPos)
            {
                Instantiate(
                    enemyPrefabs[enemyIndex],
                    new Vector3(Random.Range(-levelDimensions.x / 2f + 1, levelDimensions.x / 2f - 1), yPos),
                    Quaternion.identity,
                    transform);
            }
        }
    }
}