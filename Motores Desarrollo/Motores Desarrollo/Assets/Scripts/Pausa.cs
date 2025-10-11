using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject objetoPausa; 
    public MonoBehaviour scriptCamara; 

    private bool juegoPausado = false;

    void Start()
    {
        if (objetoPausa != null)
            objetoPausa.SetActive(false); 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    public void PausarJuego()
    {
        Time.timeScale = 0f;
        juegoPausado = true;

        if (objetoPausa != null)
            objetoPausa.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (scriptCamara != null)
            scriptCamara.enabled = false;
    }

    public void ReanudarJuego()
    {
        Time.timeScale = 1f;
        juegoPausado = false;

        if (objetoPausa != null)
            objetoPausa.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (scriptCamara != null)
            scriptCamara.enabled = true;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
