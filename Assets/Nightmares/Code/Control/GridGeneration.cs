using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control
{
    public class GridGeneration : MonoBehaviour
    {
        [SerializeField] private Vector2Int dimensions = new Vector2Int(9, 40);
        [SerializeField] private Tilemap tilemap;
        
        [Header("Tiles")]
        [SerializeField] private Tile wallLeft;
        [SerializeField] private Tile wallRight;
        [SerializeField] private Tile wallTop;
        [SerializeField] private Tile wallBottom;

        [ContextMenu("Clean Tile Map")]
        public void CleanTileMap()
        {
            tilemap.ClearAllTiles();
        }

        [ContextMenu("Spawn Tile Map")]
        public void SpawnTileMap()
        {
            CleanTileMap();
            SpawnRow(wallTop, -dimensions.x / 2, dimensions.x / 2, 0);
            SpawnWalls();
            SpawnRow(wallBottom, -dimensions.x / 2, dimensions.x / 2, dimensions.y);
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
            var xLeft = -dimensions.x / 2;
            var xRight = dimensions.x / 2;
            for (int y = 1; y < dimensions.y; y++)
            {
                tilemap.SetTile(new Vector3Int(xLeft, y), wallLeft);
                tilemap.SetTile(new Vector3Int(xRight, y), wallRight);
            }
        }
    }
}
