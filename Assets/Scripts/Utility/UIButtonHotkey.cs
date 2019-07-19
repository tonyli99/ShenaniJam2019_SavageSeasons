namespace Blobber
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Maps a keycode to a UI button.
    /// </summary>
    public class UIButtonHotkey : MonoBehaviour
    {
        public KeyCode hotkey;

        private void Update()
        {
            if (Input.GetKeyDown(hotkey))
            {
                GetComponent<Button>().onClick.Invoke();
            }
        }
    }
}