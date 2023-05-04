using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nightmares.Code.Control
{
    public class GridGeneration : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Tile tile;

        [ContextMenu("Clean Tile Map")]
        public void CleanTileMap()
        {
            tilemap.ClearAllTiles();
        }

        [ContextMenu("Spawn Tile Map")]
        public void SpawnTileMap()
        {
            CleanTileMap();
            for (int i = 0; i < 10; i++)
            {
                tilemap.SetTile(new Vector3Int(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5)), tile);
            }
        }
    }
}
