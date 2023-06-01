using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control.Enemy
{
    public class EnemySpawn : MonoBehaviour
    {
        [SerializeField] private Enemy[] enemyPrefabs;
        [SerializeField] private float step = 1;
        [SerializeField] private AnimationCurve budget;
        [SerializeField] private PlatformerGameManager gameManager;
        [SerializeField] private Tilemap mainTileMap;
        
        private float _balance;
        private int[] _spawnCost;
        
        public void SpawnEnemies()
        {
            _spawnCost = enemyPrefabs.Select(e => e.Points).ToArray();
            
            var levelDimensions = gameManager.LevelDimensions;
            float yLimit = levelDimensions.y;
            for (float y = 0; y < yLimit; y += step)
            {
                _balance += budget.Evaluate(y / yLimit);
                if (_balance >= 1)
                {
                    var enemyIndex = Random.Range(0, enemyPrefabs.Length);
                    while (_spawnCost[enemyIndex] > _balance)
                    {
                        enemyIndex--;
                    }
                    _balance -= _spawnCost[enemyIndex];
                    SpawnEnemy(enemyIndex, -y);
                }
            }

            void SpawnEnemy(int enemyIndex, float yPos)
            {
                var pos = new Vector3Int((int)Random.Range(-levelDimensions.x / 2f + 1, levelDimensions.x / 2f - 1), (int)yPos);

                if (mainTileMap.HasTile(pos))
                {
                    var dimensions = gameManager.LevelDimensions;
                    var from = -dimensions.x / 2 - 1;
                    var to = dimensions.x / 2;

                    for (int x = from; x <= to; x++)
                    {
                        var nextPos = new Vector3Int(x, pos.y);
                        if(mainTileMap.HasTile(nextPos)) continue;
                        pos = nextPos;
                        InstantiateEnemy(pos);
                        return;
                    }
                }
                else
                {
                    InstantiateEnemy(pos);
                }

                void InstantiateEnemy(Vector3Int tileCoordPos)
                {
                    var finPos = new Vector3(tileCoordPos.x + .5f, tileCoordPos.y + .5f);
                    Instantiate(
                        enemyPrefabs[enemyIndex],
                        finPos,
                        Quaternion.identity,
                        transform);
                }
            }
        }
    }
}