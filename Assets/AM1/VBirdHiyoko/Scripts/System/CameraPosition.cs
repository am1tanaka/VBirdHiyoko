using UnityEngine;

namespace AM1.VBirdHiyoko
{
    public class CameraPosition : MonoBehaviour
    {
        void Start()
        {
            Camera.main.transform.position = transform.position;
            Camera.main.transform.rotation = transform.rotation;
            Camera.main.transform.localScale = transform.localScale;

            var myCamera = GetComponent<Camera>();
            Camera.main.backgroundColor = myCamera.backgroundColor;

            Destroy(gameObject);
        }

    }
}