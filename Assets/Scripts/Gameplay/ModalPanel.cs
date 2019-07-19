namespace Blobber
{
    using UnityEngine;

    /// <summary>
    /// Add to a window to pause the game when the window is active.
    /// </summary>
    public class ModalPanel : MonoBehaviour
    {
        public static int numModalPanelsOpen = 0;

        private void OnEnable()
        {
            numModalPanelsOpen++;
            Gameplay.isPaused = true;
        }

        private void OnDisable()
        {
            numModalPanelsOpen--;
            if (numModalPanelsOpen == 0)
            {
                Gameplay.isPaused = false;
            }
        }
    }
}