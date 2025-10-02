using UnityEngine;

public class UICameraFix : MonoBehaviour
{
    void Start()
    {
        // 🔓 Desbloquear y mostrar el cursor para que funcione la UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 📷 Revisar si ya hay una cámara en la escena
        Camera cam = Camera.main;

        if (cam == null)
        {
            // Si no existe, crear una cámara nueva para la UI
            GameObject camObj = new GameObject("UICamera");
            cam = camObj.AddComponent<Camera>();

            // Configuración básica
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.cullingMask = LayerMask.GetMask("UI"); // Solo la capa UI
            cam.orthographic = true;
        }

        // 🎯 Vincular la cámara al Canvas si existe
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = cam;
        }
    }
}
