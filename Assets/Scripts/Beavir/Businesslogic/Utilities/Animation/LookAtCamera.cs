using Beavir.Businesslogic.Controllers;
using UnityEngine;

namespace Beavir.Businesslogic.Utilities
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform playerCamera;
        private void Start()
        {
            playerCamera = BeavirAppManager.Instance.UserManager.Player.PlayerCamera.transform;
        }
        // Update is called once per frame
        void Update()
        {
            // Rotate the camera every frame so it keeps looking at the target
            Vector3 difference = playerCamera.position - transform.position;
            float rotationY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
        }
    }
}
