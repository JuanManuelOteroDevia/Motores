using UnityEngine;

public class UICameraFix : MonoBehaviour
{
    void Start()
    {
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Camera cam = Camera.main;

        if (cam == null)
        {
            
            GameObject camObj = new GameObject("UICamera");
            cam = camObj.AddComponent<Camera>();

           
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.cullingMask = LayerMask.GetMask("UI"); 
            cam.orthographic = true;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = cam;
        }
    }
}
