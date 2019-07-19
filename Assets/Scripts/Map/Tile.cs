namespace Blobber
{
    using UnityEngine;

    /// <summary>
    /// Base tile for grid-based map. Tiles can contain TileContent.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Tile", order = 0)]
    public class Tile : ScriptableObject
    {
        public string description;

        public Sprite icon;

        public GameObject model;

        public bool open;

    }
}