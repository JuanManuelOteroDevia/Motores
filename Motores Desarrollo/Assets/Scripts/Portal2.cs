using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal2 : MonoBehaviour
{
    [Header("Configuración de escena")]
    public string sceneToLoad;          // Nombre de la escena a cargar
    public float fadeDuration = 1f;     // Duración del fade

    private bool isTransitioning = false;
    private float fadeAlpha = 0f;
    private bool isFading = false;

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

        // Fade IN → pantalla a negro
        isFading = true;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeAlpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        // Cargar la escena directamente
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Fade OUT → pantalla vuelve a normal
        StartCoroutine(FadeOut());
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeAlpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        isFading = false;
        isTransitioning = false;
    }

    private void OnGUI()
    {
        if (isFading)
        {
            // Dibujar rectángulo negro encima de todo
            Color col = GUI.color;
            col.a = fadeAlpha;
            GUI.color = col;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.blackTexture);
            GUI.color = Color.white;
        }
    }
}
