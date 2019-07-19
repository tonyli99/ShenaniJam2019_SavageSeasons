namespace Blobber
{
    using UnityEngine;

    public class SplashScene : MonoBehaviour
    {
        public void LoadMainMenu()
        {
            SceneChanger.LoadScene(SceneNames.MainMenu);
        }
    }
}