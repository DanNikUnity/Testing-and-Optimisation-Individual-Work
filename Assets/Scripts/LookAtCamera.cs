using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        float cameraRotationX = cam.transform.rotation.eulerAngles.x;
        transform.rotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }

    void Update()
    {
        // Получаем ротацию камеры по оси X

    }
}
