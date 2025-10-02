using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPortal : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    public string sceneToLoad = "TutorialLevel1";

    [Header("Texto de interacci�n (Canvas UI)")]
    public GameObject interactionText;

    private bool playerIsNear = false;

    void Start()
    {
        if (interactionText != null)
            interactionText.SetActive(false); 
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(sceneToLoad); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            if (interactionText != null)
                interactionText.SetActive(true); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            if (interactionText != null)
                interactionText.SetActive(false);
        }
    }
}
