namespace Blobber
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Singleton that maintains a list of all traits.
    /// </summary>
    public class TraitAssets : MonoBehaviour
    {
        public List<Trait> traits;

        public static TraitAssets instance = null;

        private void Awake()
        {
            instance = this;
        }

        public Trait GetTrait(int traitID)
        {
            return (0 <= traitID && traitID < traits.Count) ? traits[traitID] : null;
        }

        public List<int> GetShuffledTraitIDList()
        {
            var list = new List<int>();
            for (int i = 0; i < traits.Count; i++)
            {
                list.Add(i);
            }
            var n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int r = i + Random.Range(0, n - i);
                var t = list[r];
                list[r] = list[i];
                list[i] = t;
            }
            return list;
        }
    }
}