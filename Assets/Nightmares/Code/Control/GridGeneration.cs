using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control
{
    public class GridGeneration : MonoBehaviour
    {
        [SerializeField] private float operationStepDuration = .1f;
        [SerializeField] private bool slowDown = false;

        [Header("Components")]
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private PlatformerGameManager gameManager;

        [Header("Wall Tiles")]
        [SerializeField] private RuleTile wallRuleTile;

        [Header("Ledges")]
        [SerializeField] private bool spawnLedges = true;
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

        private WaitForSeconds _wfs;

        [ContextMenu("Clean Tile Map")]
        public void CleanTileMap()
        {
            StopAllCoroutines();
            tilemap.ClearAllTiles();
            platforms.Clear();
        }

        [ContextMenu("Spawn Tile Map")]
        public void SpawnTileMap() => SpawnTileMap(out _);
        public void SpawnTileMap(out Coroutine impl)
        {
            CleanTileMap();
            _wfs = new WaitForSeconds(operationStepDuration);

            impl = StartCoroutine(Impl());
            IEnumerator Impl()
            {
                UpdDimensions();

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

                if (spawnLedges) yield return SpawnLedges();

                if (spawnPlatforms) yield return SpawnPlatforms();

                yield return null;
            }
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

        private IEnumerator SpawnLedges()
        {
            var difficulty = gameManager.Difficulty;
            
            var ledgeCount = (int)(_levelDimensions.y * ledgesPerUnit.Evaluate(difficulty));
            var ledgeYPos = Enumerable.Range(0, ledgeCount)
                .Select(_ => Random.Range(_bottomWall + verticalGap.y, _topWall - verticalGap.y))
                .OrderByDescending(x => x)
                .ToList();

            foreach (var yStart in ledgeYPos)
            {
                var totalHeight = 
                    (int) Random.Range(ledgeHeightMin.Evaluate(difficulty), ledgeHeightMax.Evaluate(difficulty));
                var yEnd = yStart - totalHeight;
                yEnd = Mathf.Max(yEnd, _bottomWall + verticalGap.y);
                
                var maxWidth = (int)((_rightWall - _leftWall - 1) 
                                * Random.Range(ledgeWidthMin.Evaluate(difficulty), ledgeWidthMax.Evaluate(difficulty)));
                var onLeft = Random.Range(0, 2) == 0;
                var xRange = onLeft
                    ? new Vector2Int(_leftWall + 1, _leftWall + maxWidth)
                    : new Vector2Int(_rightWall - maxWidth, _rightWall - 1);

                SpawnLedge(yStart, yEnd, xRange, onLeft);

                if(slowDown) yield return _wfs;
            }
        }

        private void SpawnLedge(int yTop, int yBottom, Vector2Int xRange, bool onLeft)
        {
            var positions = new List<Vector3Int>();

            var pi = Mathf.PI;
            var rangeStart = Random.Range(0f, pi);
            var radRange = new Vector2(rangeStart, Random.Range(rangeStart, pi));
            
            for (int y = yBottom; y <= yTop; y++)
            {
                var radVal = Mathf.Lerp(radRange.x, radRange.y, Mathf.InverseLerp(yBottom, yTop, y));
                var sinVal = Mathf.Sin(radVal);

                var from = onLeft ? xRange.x : Mathf.FloorToInt(Mathf.Lerp(xRange.y, xRange.x, sinVal));
                var to = onLeft ? Mathf.CeilToInt(Mathf.Lerp(xRange.x, xRange.y, sinVal)) : xRange.y;
                
                for (var x = from; x <= to; x++)
                {
                    var pos = new Vector3Int(x, y);
                    if (CanPutTileOn(pos,
                            onLeft ? Mathf.Min(2, x - _leftWall - 1) : 2,
                            onLeft ? 2 : Mathf.Min(2, _rightWall - x - 1),
                            2, 2))
                    {
                        positions.Add(pos);
                    }
                }

            }

            foreach (var pos in positions)
            {
                tilemap.SetTile(pos, wallRuleTile);
            }
        }

        private bool CanPutTileOn(Vector3Int pos, int emptyLeft, int emptyRight, int emptyUp, int emptyDown)
        {
            for (int x = pos.x - emptyLeft; x <= pos.x + emptyRight; x++)
            {
                for (int y = pos.y - emptyDown; y <= pos.y + emptyUp; y++)
                {
                    if (tilemap.HasTile(new Vector3Int(x, y))) return false;
                }       
            }

            return true;
        }

        private IEnumerator SpawnPlatforms()
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

                if(slowDown) yield return _wfs;
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
            int type = tilemap.GetTile(new Vector3Int(xLeft - 1, y)) == wallRuleTile
                ? -1 // Left wall connection
                : tilemap.GetTile(new Vector3Int(xRight + 1, y)) == wallRuleTile 
                    ? 1 // Right wall connection
                    : 0; // Free platform

            var tileToUse = type == 0 ? platform : wallRuleTile;
            var positions = new List<Vector3Int>();
            for (int x = xLeft; x <= xRight; x++)
            {
                var pos = new Vector3Int(x, y);
                if (CanPutTileOn(pos,
                        emptyLeft: type == -1 ? 0 : 1,
                        emptyRight: type == 1 ? 0 : 1,
                        emptyUp: 2,
                        emptyDown: 2))
                {
                    positions.Add(pos);
                }
            }

            foreach (var pos in positions)
            {
                tilemap.SetTile(pos, tileToUse);
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

        public void MergeIn(Tilemap bossTilemap)
        {
            var bounds = bossTilemap.cellBounds;
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y <= bounds.yMax; y++)
                {
                    var pos = new Vector3Int(x, y);
                    if (bossTilemap.HasTile(pos))
                    {
                        tilemap.SetTile(pos - new Vector3Int(0, _levelDimensions.y), bossTilemap.GetTile(pos));
                    }
                }
            }
            
            bossTilemap.gameObject.SetActive(false);
        }
    }
}
