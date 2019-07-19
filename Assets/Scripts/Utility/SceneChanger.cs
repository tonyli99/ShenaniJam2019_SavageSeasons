namespace Blobber
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    /// SceneManager.LoadScene wrapper that does a nice fade out/fade in.
    /// </summary>
    public class SceneChanger : MonoBehaviour
    {
        public const float fadeDuration = 0.5f;

        public Canvas canvas;
        public Image image;

        public static SceneChanger instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        IEnumerator LoadSceneNicely(string sceneName)
        {
            canvas.enabled = true;
            float elapsed = 0;
            while (elapsed < fadeDuration)
            {
                image.color = new Color(0, 0, 0, elapsed / fadeDuration);
                yield return null;
                elapsed += Time.deltaTime;
            }
            image.color = new Color(0, 0, 0, 1);
            SceneManager.LoadScene(sceneName);
            elapsed = 0;
            while (elapsed < fadeDuration)
            {
                image.color = new Color(0, 0, 0, 1 - (elapsed / fadeDuration));
                yield return null;
                elapsed += Time.deltaTime;
            }
            image.color = new Color(0, 0, 0, 0);
            canvas.enabled = false;
        }

        public static void LoadScene(string sceneName)
        {
            instance.StartCoroutine(instance.LoadSceneNicely(sceneName));
        }
    }
}
