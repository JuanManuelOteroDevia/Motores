using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("Configuración de escena")]
    public string sceneToLoad = "SelectLevel"; // Asegurate que coincida con el nombre exacto

    public float fadeDuration = 1f;
    private bool isTransitioning = false;
    private CanvasGroup fadeCanvas;

    private void Awake()
    {
        // Crear Canvas para fade
        GameObject fadeObj = new GameObject("FadeCanvas");
        Canvas canvas = fadeObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas = fadeObj.AddComponent<CanvasGroup>();
        fadeObj.AddComponent<CanvasRenderer>();

        UnityEngine.UI.Image img = fadeObj.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.black;

        RectTransform rt = fadeObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        fadeCanvas.alpha = 0;
        DontDestroyOnLoad(fadeObj); // Persistir entre escenas
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTransitioning && other.CompareTag("Player"))
        {
            StartCoroutine(TransitionScene());
        }
    }

    private IEnumerator TransitionScene()
    {
        isTransitioning = true;

        // Fade IN
        yield return StartCoroutine(Fade(1));

        // 🔑 Guardar que el jugador pasó por el portal del tutorial
        PlayerPrefs.SetInt("EliminarBarrera", 1);
        PlayerPrefs.Save();

        // Cargar la escena SelectLevel
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}
