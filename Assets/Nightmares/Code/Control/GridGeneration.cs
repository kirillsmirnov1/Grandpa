using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Nightmares.Code.Control
{
    public class GridGeneration : MonoBehaviour
    {
        [SerializeField] private Vector2Int levelDimensions = new(13, 40);
        [SerializeField] private Vector2Int verticalGap = new(3, 5);

        [Header("Components")]
        [SerializeField] private Tilemap tilemap;
        
        [Header("Wall Tiles")]
        [SerializeField] private Tile wallLeft;
        [SerializeField] private Tile wallRight;
        [SerializeField] private Tile wallTop;
        [SerializeField] private Tile wallBottom;

        [Header("Platform Tile")]
        [SerializeField] private Tile platformMidTile;

        [ContextMenu("Clean Tile Map")]
        public void CleanTileMap()
        {
            tilemap.ClearAllTiles();
        }

        [ContextMenu("Spawn Tile Map")]
        public void SpawnTileMap()
        {
            CleanTileMap();
            
            SpawnRow(wallTop, -levelDimensions.x / 2, levelDimensions.x / 2, 0);
            SpawnWalls();
            SpawnRow(wallBottom, -levelDimensions.x / 2, levelDimensions.x / 2, -levelDimensions.y);
            
            SpawnPlatforms();
        }

        private void SpawnRow(Tile tile, int xFrom, int xTo, int y)
        {
            for (int x = xFrom + 1; x < xTo; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y), tile);
            }
        }

        private void SpawnWalls()
        {
            var xLeft = -levelDimensions.x / 2;
            var xRight = levelDimensions.x / 2;
            for (int y = 1; y < levelDimensions.y; y++)
            {
                tilemap.SetTile(new Vector3Int(xLeft, -y), wallLeft);
                tilemap.SetTile(new Vector3Int(xRight, -y), wallRight);
            }
        }

        private void SpawnPlatforms()
        {
            var ranges = GenerateRanges();
            var nextRangeIndex = 0;
            for (int y = verticalGap.y; y < levelDimensions.y - verticalGap.y; )
            {
                var range = ranges[nextRangeIndex];
                
                var left = Random.Range(range.x, range.y - 1);
                var right = Random.Range(left + 1, range.y + 1);

                SpawnPlatform(left, right, -y);
                
                y += Random.Range(verticalGap.x, verticalGap.y + 1);
                nextRangeIndex = Random.Range(0, 3);
            }
        }

        private void SpawnPlatform(int xLeft, int xRight, int y)
        {
            for (int x = xLeft; x <= xRight; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y), platformMidTile);
            }
        }

        private Vector2Int[] GenerateRanges()
        {
            int maxPlatformWidth = MaxPlatformWidth();
            return new[]
            {
                new Vector2Int(-levelDimensions.x / 2 + 1, -levelDimensions.x / 2 + maxPlatformWidth),
                new Vector2Int(-maxPlatformWidth / 2, maxPlatformWidth / 2),
                new Vector2Int(levelDimensions.x / 2 - maxPlatformWidth, levelDimensions.x / 2 - 1),
            };
        }

        private int MaxPlatformWidth()
        {
            var maxPlatformWidth = levelDimensions.x / 3 + 1;
            if (levelDimensions.x % 2 == 0)
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
