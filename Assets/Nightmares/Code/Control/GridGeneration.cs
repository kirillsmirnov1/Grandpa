using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

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
        [SerializeField] private Tile platformMidTile;

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

            SpawnTopRow(wallRuleTile, -_levelDimensions.x / 2 - 1, _levelDimensions.x / 2, 0);
            SpawnWalls();
            SpawnPlatforms();
        }

        private void SpawnTopRow(TileBase tile, int xFrom, int xTo, int y)
        {
            for (int x = xFrom-1; x <= xTo+1; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y+1), tile);
                tilemap.SetTile(new Vector3Int(x, y), tile);
            }
        }

        private void SpawnWalls()
        {
            var xLeft = -_levelDimensions.x / 2 - 1;
            var xRight = _levelDimensions.x / 2;
            for (int y = 1; y <= _levelDimensions.y; y++)
            {
                tilemap.SetTile(new Vector3Int(xLeft, -y), wallRuleTile);
                tilemap.SetTile(new Vector3Int(xLeft - 1, -y), wallRuleTile);
                tilemap.SetTile(new Vector3Int(xRight, -y), wallRuleTile);
                tilemap.SetTile(new Vector3Int(xRight + 1, -y), wallRuleTile);
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
            for (int x = xLeft; x <= xRight; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y), platformMidTile);
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
