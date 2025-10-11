using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;        
    public float sensitivity = 2f;
    public float distance = 6f;     
    public float heightOffset = 1.5f;
    public float minY = -20f;
    public float maxY = 60f;

    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        currentX += Input.GetAxis("Mouse X") * sensitivity;
        currentY -= Input.GetAxis("Mouse Y") * sensitivity;
        currentY = Mathf.Clamp(currentY, minY, maxY);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = new Vector3(0, heightOffset, -distance);
        Vector3 position = target.position + rotation * offset;

        transform.position = position;

      
        transform.LookAt(target.position + Vector3.up * heightOffset);
    }
}
