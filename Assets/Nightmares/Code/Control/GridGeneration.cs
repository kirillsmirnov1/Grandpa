using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control
{
    public class GridGeneration : MonoBehaviour
    {
        [Range(1, 5)]
        [SerializeField] private int debugDifficulty = 1;
        
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
        
        [Header("Platforms")]
        [SerializeField] private bool spawnPlatforms = true;
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
            
            if(spawnPlatforms) SpawnPlatforms();
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
            var difficulty = debugDifficulty;
            
            var ledgeCount = (int)(_levelDimensions.y * ledgesPerUnit.Evaluate(difficulty));
            var ledgeYPos = Enumerable.Range(0, ledgeCount)
                .Select(_ => Random.Range(_bottomWall + verticalGap.y, _topWall - verticalGap.y))
                .OrderByDescending(x => x)
                .ToList();

            foreach (var yCenter in ledgeYPos)
            {
                var totalHeight = 
                    (int) Random.Range(ledgeHeightMin.Evaluate(difficulty), ledgeHeightMax.Evaluate(difficulty));
                var maxWidth = (int)((_rightWall - _leftWall - 1) 
                                * Random.Range(ledgeWidthMin.Evaluate(difficulty), ledgeWidthMax.Evaluate(difficulty)));
                var left = Random.Range(0, 2) == 0;
                var xRange = left
                    ? new Vector2Int(_leftWall + 1, _leftWall + maxWidth)
                    : new Vector2Int(_rightWall - maxWidth, _rightWall - 1);

                // TODO refactor into non-rect spawn
                for (var x = left ? xRange.x : xRange.y; 
                     left ? x <= xRange.y : x >= xRange.x; 
                     x += left ? 1 : -1)
                {
                    for (int y = yCenter; y >= yCenter - totalHeight; y--)
                    {
                        var pos = new Vector3Int(x, y);
                        if (CanPutTileOn(pos, left ? 0 : 2, left ? 2 : 0))
                        {
                            tilemap.SetTile(pos, wallRuleTile);
                        }
                    }
                }
            }
        }

        private bool CanPutTileOn(Vector3Int pos, int emptyLeft, int emptyRight)
        {
            if (tilemap.HasTile(pos)) return false;

            // TODO refactor into one 2d cycle 
            
            // Left check
            for (int x = pos.x - 1; x >= pos.x - emptyLeft; x--)
            {
                if (tilemap.HasTile(new Vector3Int(x, pos.y))) return false;
            }
            
            // Right check
            for (int x = pos.x + 1; x <= pos.x + emptyRight; x++)
            {
                if (tilemap.HasTile(new Vector3Int(x, pos.y))) return false;
            }

            return true;
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
