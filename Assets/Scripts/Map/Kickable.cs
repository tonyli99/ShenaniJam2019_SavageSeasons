namespace Blobber
{
    using UnityEngine;

    /// <summary>
    /// Identifies tile content such as doors that can be kicked to remove.
    /// </summary>
    public class Kickable : MonoBehaviour
    {
        public Color kickColor;
        public AudioClip kickAudioClip;
    }
}