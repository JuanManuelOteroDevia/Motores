using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPortal : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    public string sceneToLoad = "TutorialLevel1";

    [Header("Texto de interacción (Canvas UI)")]
    public GameObject interactionText;

    private bool playerIsNear = false;

    void Start()
    {
        if (interactionText != null)
            interactionText.SetActive(false); // Oculta el texto al inicio
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(sceneToLoad); // Carga la escena
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            if (interactionText != null)
                interactionText.SetActive(true); // Muestra el texto
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (interactionText != null)
                interactionText.SetActive(false); // Oculta el texto
        }
    }
}
