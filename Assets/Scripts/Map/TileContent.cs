namespace Blobber
{
    using UnityEngine;

    /// <summary>
    /// Something in a Tile, such as a monster or door.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Tiles/Tile Content", order = 0)]
    public class TileContent : ScriptableObject
    {
        public string description;

        public Sprite icon;

        public GameObject model;
    }
}