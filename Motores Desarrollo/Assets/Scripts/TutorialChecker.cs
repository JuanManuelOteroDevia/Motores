using UnityEngine;
using System.Collections;

public class TutorialChecker : MonoBehaviour
{
    private void Start()
    {
        int flag = PlayerPrefs.GetInt("EliminarBarrera", 0);
        Debug.Log($"🧪 TutorialChecker activo. Bandera EliminarBarrera = {flag}");

        if (flag == 1)
        {
            GameObject[] barreras = GameObject.FindGameObjectsWithTag("BRT1");
            Debug.Log($"🔍 Encontradas {barreras.Length} barreras con tag BRT1");

            foreach (GameObject b in barreras)
            {
                Destroy(b);
                Debug.Log($"✅ Barrera '{b.name}' eliminada porque el jugador pasó por el portal del tutorial");
            }

            PlayerPrefs.DeleteKey("EliminarBarrera");
            Debug.Log("🧹 Bandera EliminarBarrera borrada");
        }

        CanvasGroup fadeCanvas = GameObject.Find("FadeCanvas")?.GetComponent<CanvasGroup>();
        if (fadeCanvas != null)
        {
            Debug.Log("🎬 Ejecutando FadeOut en SelectLevel");
            StartCoroutine(FadeOut(fadeCanvas));
        }
        else
        {
            Debug.Log("⚠️ No se encontró FadeCanvas en SelectLevel");
        }
    }

    private IEnumerator FadeOut(CanvasGroup canvas)
    {
        float t = 0f;
        float duration = 1f;
        float startAlpha = canvas.alpha;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(startAlpha, 0f, t / duration);
            yield return null;
        }

        canvas.alpha = 0f;
    }
}
