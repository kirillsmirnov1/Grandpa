using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control
{
    public class GridGeneration : MonoBehaviour
    {
        [SerializeField] private Vector2Int verticalGap = new(3, 5);

        [Header("Components")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private PlatformerGameManager gameManager;

        [Header("Wall Tiles")]
        [SerializeField] private RuleTile wallRuleTile;
        
        [Header("Platform Tile")]
        [SerializeField] private RuleTile platform;

        public List<Vector3> platforms = new();
        
        private Vector2Int _levelDimensions;

        [ContextMenu("Clean Tile Map")]
        public void CleanTileMap()
        {
            tilemap.ClearAllTiles();
        }

        [ContextMenu("Spawn Tile Map")]
        public void SpawnTileMap()
        {
            _levelDimensions = gameManager.LevelDimensions;
            
            CleanTileMap();

            // Top wall
            SpawnWallRegion(wallRuleTile, 
                new Vector2Int(-_levelDimensions.x / 2 - 2, 0), 
                new Vector2Int(_levelDimensions.x / 2 + 1, 1));
            
            // Left wall
            SpawnWallRegion(wallRuleTile, 
                new Vector2Int(-_levelDimensions.x / 2 - 2, -_levelDimensions.y), 
                new Vector2Int(-_levelDimensions.x / 2 - 1, -1));
            
            // Right wall
            SpawnWallRegion(wallRuleTile, 
                new Vector2Int(_levelDimensions.x / 2, -_levelDimensions.y), 
                new Vector2Int(_levelDimensions.x / 2 + 1, -1));

            SpawnPlatforms();
        }

        private void SpawnWallRegion(TileBase tile, Vector2Int from, Vector2Int to)
        {
            for (int x = from.x; x <= to.x; x++)
            {
                for (int y = from.y; y <= to.y; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y), tile);
                }
            }
        }

        private void SpawnPlatforms()
        {
            var ranges = GenerateRanges();
            var nextRangeIndex = 0;
            for (int y = verticalGap.y; y < _levelDimensions.y - verticalGap.x; )
            {
                var range = ranges[nextRangeIndex];
                
                var left = Random.Range(range.x, range.y - 1);
                var right = Random.Range(left + 1, range.y + 1);

                SpawnPlatform(left, right, -y);
                
                y += Random.Range(verticalGap.x, verticalGap.y + 1);
                
                nextRangeIndex = GetNextRangeIndex(nextRangeIndex);
            }
        }

        private static int GetNextRangeIndex(int rangeIndex)
        {
            int next;
            do
            {
                next = Random.Range(0, 3);
            } while (next == rangeIndex);

            rangeIndex = next;
            return rangeIndex;
        }

        private void SpawnPlatform(int xLeft, int xRight, int y)
        {
            var tileToUse =
                tilemap.GetTile(new Vector3Int(xLeft - 1, y)) == wallRuleTile
                || tilemap.GetTile(new Vector3Int(xRight + 1, y)) == wallRuleTile
                    ? wallRuleTile
                    : platform;
            for (int x = xLeft; x <= xRight; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y), tileToUse);
            }
            platforms.Add(new Vector3(xLeft, xRight, y));
        }

        private Vector2Int[] GenerateRanges()
        {
            int maxPlatformWidth = MaxPlatformWidth();
            return new[]
            {
                new Vector2Int(-_levelDimensions.x / 2 , -_levelDimensions.x / 2 + maxPlatformWidth - 1),
                new Vector2Int(-maxPlatformWidth / 2 - 1, maxPlatformWidth / 2),
                new Vector2Int(_levelDimensions.x / 2 - maxPlatformWidth, _levelDimensions.x / 2 - 1),
            };
        }

        private int MaxPlatformWidth()
        {
            var maxPlatformWidth = _levelDimensions.x / 3 + 1;
            if (_levelDimensions.x % 2 == 0)
            {
                if (maxPlatformWidth % 2 != 0)
                {
                    maxPlatformWidth++;
                }
            }
            else
            {
                if (maxPlatformWidth % 2 == 0)
                {
                    maxPlatformWidth++;
                }
            }

            return maxPlatformWidth;
        }
    }
}
