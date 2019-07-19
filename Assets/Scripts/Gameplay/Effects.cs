namespace Blobber
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Utility class that shows effects such as flashes and messages.
    /// </summary>
    public class Effects : MonoBehaviour
    {
        [Header("Messages")]
        public float minMessageDuration = 1;
        public float messageCharsPerSec = 30;

        [Space]
        public GameObject blockingMessagePanel;
        public Text blockingMessageText;

        [Space]
        public GameObject nonblockingMessagePanel;
        public Text nonblockingMessageText;

        [Space]
        public GameObject dialogPanel;
        public Text dialogText;

        [Header("Flash")]
        public GameObject flashPanel;
        public Image flashImage;
        public float flashDuration = 0.25f;

        public System.Action onCloseDialog = null;

        public static Effects instance;


        private void Awake()
        {
            instance = this;
        }

        public static void Flash(Color color, AudioClip audioClip)
        {
            if (audioClip != null) AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
            instance.flashPanel.SetActive(true);
            instance.flashImage.color = color;
            instance.Invoke("HideFlash", instance.flashDuration);
        }

        public void HideFlash()
        {
            flashPanel.SetActive(false);
        }

        private float GetDuration(string message)
        {
            return Mathf.Max(minMessageDuration, message.Length / messageCharsPerSec);
        }

        public static void ShowBlockingMessage(string message)
        {
            instance.blockingMessagePanel.SetActive(true);
            instance.blockingMessageText.text = message;
            instance.Invoke("HideBlockingMessage", instance.GetDuration(message));
        }

        public void HideBlockingMessage()
        {
            blockingMessagePanel.SetActive(false);
        }

        public static void ShowNonblockingMessage(string message)
        {
            instance.nonblockingMessagePanel.SetActive(true);
            instance.nonblockingMessageText.text = message;
            instance.Invoke("HideNonblockingMessage", instance.GetDuration(message));
        }

        public void HideNonblockingMessage()
        {
            nonblockingMessagePanel.SetActive(false);
        }

        public static void ShowDialog(string message, System.Action closeDialogHandler)
        {
            instance.dialogPanel.SetActive(true);
            instance.dialogText.text = message;
            instance.onCloseDialog = closeDialogHandler;
        }

        public void CloseDialogClicked()
        {
            dialogPanel.SetActive(false);
            if (onCloseDialog != null) onCloseDialog();
        }

    }
}
