namespace Blobber
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Collection of the party's characters and their combined currency and keys found.
    /// </summary>
    [Serializable]
    public class Party
    {
        public const int FullSize = 4;

        public List<Character> characters = new List<Character>();
        public int currency = 0;
        public int keysFound = 0;

        public int GetNumLivingCharacters()
        {
            int count = 0;
            foreach (var character in characters)
            {
                if (character != null && character.isAlive) count++;
            }
            return count;
        }

        public int GetNumFrontCharacters() // 2 characters in front, 2 in back.
        {
            int count = 0;
            for (int i = 0; i < 2; i++)
            {
                if (i >= characters.Count) continue;
                if (characters[i] != null && characters[i].isAlive) count++;
            }
            return count;
        }
    }
}