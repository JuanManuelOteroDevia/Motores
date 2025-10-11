using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI GameObjects")]
    public GameObject objetoPausa;       // GameObject con botones de pausa
    public GameObject objetoPerdiste;    // GameObject con botones de Game Over
    public MonoBehaviour scriptCamara;   // Script que mueve la cámara

    [Header("Scripts a desactivar en Game Over")]
    public MonoBehaviour[] scriptsADesactivar;

    private bool juegoPausado = false;

    void Start()
    {
        if (objetoPausa != null)
            objetoPausa.SetActive(false);

        if (objetoPerdiste == null)
            objetoPerdiste = GameObject.Find("Perdiste");

        if (objetoPerdiste != null)
            objetoPerdiste.SetActive(false);
        else
            Debug.LogWarning("GameObject 'Perdiste' no encontrado");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsGameOverActive())
        {
            if (juegoPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    // ---------------- PAUSA ----------------
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

    // ---------------- GAME OVER ----------------
    public void TriggerGameOver()
    {
        Debug.Log("TriggerGameOver llamado");

        if (objetoPerdiste != null)
        {
            objetoPerdiste.SetActive(true);

            TextMeshProUGUI texto = objetoPerdiste.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null)
                texto.text = "Moriste";

            if (scriptsADesactivar != null)
            {
                foreach (var script in scriptsADesactivar)
                {
                    if (script != null)
                        script.enabled = false;
                }
            }

            StartCoroutine(PauseAfterFrame());
        }
        else
        {
            Debug.LogWarning("GameObject 'Perdiste' no está asignado");
        }
    }

    IEnumerator PauseAfterFrame()
    {
        yield return null;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (scriptCamara != null)
            scriptCamara.enabled = false;

        UnityEngine.EventSystems.EventSystem.current.UpdateModules();

        Debug.Log("Juego pausado por Game Over");
    }

    public bool IsGameOverActive()
    {
        return objetoPerdiste != null && objetoPerdiste.activeSelf;
    }

    // ---------------- REINICIO Y MENU ----------------
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
