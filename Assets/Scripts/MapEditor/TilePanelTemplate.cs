namespace Blobber
{
    using UnityEngine;
    using UnityEngine.UI;

    public class TilePanelTemplate : MonoBehaviour
    {
        public Image tileImage;
        public Image contentImage;

        public void Assign(int tileID, int tileContentID)
        {
            tileImage.sprite = MapAssets.instance.tiles[tileID].icon;
            var hasContent = tileContentID != -1;
            contentImage.gameObject.SetActive(hasContent);
            if (hasContent) contentImage.sprite = MapAssets.instance.tileContents[tileContentID].icon;
        }
    }
}