namespace Blobber
{
    using UnityEngine;

    public class AlwaysFaceCamera : MonoBehaviour
    {
        private Camera m_mainCamera = null;

        private void Update()
        {
            if (m_mainCamera == null) m_mainCamera = Camera.main;
            if (m_mainCamera == null) return;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, m_mainCamera.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.x);
        }
    }
}
