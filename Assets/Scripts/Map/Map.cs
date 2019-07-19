namespace Blobber
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Map
    {
        public const string DefaultTitle = "New Map";
        public const int DefaultWidth = 20;
        public const int DefaultHeight = 20;
        public const int NumMessages = 10;

        public string title = DefaultTitle;
        public int width;
        public int height;
        public List<int> tileIDs = new List<int>();
        public List<int> tileContentIDs = new List<int>();

        public List<string> messages = new List<string>();

        public Map() { }

        public Map(Map other)
        {
            title = other.title;
            width = other.width;
            height = other.height;
            tileIDs = new List<int>(other.tileIDs);
            tileContentIDs = new List<int>(other.tileContentIDs);
            messages = new List<string>(other.messages);
        }

        public void Initialize(int width, int height)
        {
            this.width = width;
            this.height = height;
            tileIDs = new List<int>();
            tileContentIDs = new List<int>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tileIDs.Add(0);
                    tileContentIDs.Add(-1);
                }
            }
            messages = new List<string>();
            for (int i = 0; i< NumMessages; i++)
            {
                messages.Add(string.Empty);
            }
        }

        public int GetIndex(int x, int y)
        {
            return (y * width) + x;
        }

        public int GetTileID(int x, int y)
        {
            var index = GetIndex(x, y);
            return (0 <= index && index < tileIDs.Count) ? tileIDs[index] : -1;
        }

        public void SetTileID(int x, int y, int tileID)
        {
            var index = GetIndex(x, y);
            if (0 <= index && index < tileIDs.Count) tileIDs[index] = tileID;
        }

        public int GetTileContentID(int x, int y)
        {
            var index = GetIndex(x, y);
            return (0 <= index && index < tileContentIDs.Count) ? tileContentIDs[index] : -1;
        }

        public static string GetFullPath(string filename)
        {
#if UNITY_STANDALONE_WIN
            var separator = "\\";
#else
                var separator = "/";
#endif
            return Application.persistentDataPath + separator + filename + ".map";
        }

    }
}
