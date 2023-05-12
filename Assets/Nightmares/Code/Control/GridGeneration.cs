using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control
{
    public class GridGeneration : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private PlatformerGameManager gameManager;

        [Header("Wall Tiles")]
        [SerializeField] private RuleTile wallRuleTile;

        [Header("Ledges")]
        [SerializeField] private AnimationCurve ledgesPerUnit;
        [SerializeField] private AnimationCurve ledgeWidthMin;
        [SerializeField] private AnimationCurve ledgeWidthMax;
        [SerializeField] private AnimationCurve ledgeHeightMin;
        [SerializeField] private AnimationCurve ledgeHeightMax;
        
        [Header("Platform Tile")]
        [SerializeField] private RuleTile platform;
        [SerializeField] private Vector2Int verticalGap = new(3, 5);

        public List<Vector3> platforms = new();
        
        private Vector2Int _levelDimensions;
        private int _leftWall;
        private int _rightWall;
        private int _topWall;
        private int _bottomWall;

        [ContextMenu("Clean Tile Map")]
        public void CleanTileMap()
        {
            tilemap.ClearAllTiles();
            platforms.Clear();
        }

        [ContextMenu("Spawn Tile Map")]
        public void SpawnTileMap()
        {
            UpdDimensions();

            CleanTileMap();

            // Top wall
            SpawnWallRegion(wallRuleTile, 
                new Vector2Int(_leftWall - 1, _topWall), 
                new Vector2Int(_rightWall + 1, _topWall + 1));
            
            // Left wall
            SpawnWallRegion(wallRuleTile, 
                new Vector2Int(_leftWall - 1, _bottomWall), 
                new Vector2Int(_leftWall, _topWall - 1));
            
            // Right wall
            SpawnWallRegion(wallRuleTile, 
                new Vector2Int(_rightWall, _bottomWall), 
                new Vector2Int(_rightWall + 1, _topWall - 1));

            SpawnLedges();
            
            SpawnPlatforms();
        }

        private void UpdDimensions()
        {
            _levelDimensions = gameManager.LevelDimensions;
            _leftWall = -_levelDimensions.x / 2 - 1;
            _rightWall = _levelDimensions.x / 2;
            _topWall = 0;
            _bottomWall = -_levelDimensions.y;
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

        private void SpawnLedges()
        {
            // TODO    
        }

        private void SpawnPlatforms()
        {
            var ranges = GenerateRanges();
            var nextRangeIndex = 0;
            for (int y = -verticalGap.y; y >= _bottomWall + verticalGap.x; )
            {
                var range = ranges[nextRangeIndex];
                
                var left = Random.Range(range.x, range.y - 1);
                var right = Random.Range(left + 1, range.y + 1);

                SpawnPlatform(left, right, y);
                
                y -= Random.Range(verticalGap.x, verticalGap.y + 1);
                
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
            
            return next;
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
