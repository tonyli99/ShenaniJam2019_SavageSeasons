namespace Blobber
{
    using UnityEngine;

    /// <summary>
    /// Base encounter type -- something that can be on a tile, such as
    /// an enemy, text message, or loot box vendor.
    /// </summary>
    public class Encounter : MonoBehaviour
    {
        public virtual void StartEncounter()
        {
        }
    }
}