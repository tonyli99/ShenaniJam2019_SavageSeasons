namespace Blobber
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Maintains lists of all Tiles and all TileContents.
    /// </summary>
    public class MapAssets : MonoBehaviour
    {
        public List<Tile> tiles;
        public Tile defaultTile;

        public List<TileContent> tileContents;
        public TileContent startTileContent;

        public static MapAssets instance = null;

        private void Awake()
        {
            instance = this;
        }

        public Tile GetTile(int tileID)
        {
            return (0 <= tileID && tileID < tiles.Count) ? tiles[tileID] : tiles[0];
        }

        public TileContent GetTileContent(int tileContentID)
        {
            return (0 <= tileContentID && tileContentID < tileContents.Count) ? tileContents[tileContentID] : null;
        }
    }
}