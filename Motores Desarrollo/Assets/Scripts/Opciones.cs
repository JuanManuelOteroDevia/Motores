using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpcionesUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumenSlider;
    public Toggle pantallaCompletaToggle;
    public Text toggleLabel;

    void Start()
    {
        // Inicializar volumen
        volumenSlider.value = AudioListener.volume;
        volumenSlider.onValueChanged.AddListener(CambiarVolumen);

        // Inicializar pantalla completa
        pantallaCompletaToggle.isOn = Screen.fullScreen;
        ActualizarToggleLabel(Screen.fullScreen);
        pantallaCompletaToggle.onValueChanged.AddListener(CambiarPantallaCompleta);
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
    }

    public void CambiarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
        ActualizarToggleLabel(pantallaCompleta);
    }

    void ActualizarToggleLabel(bool pantallaCompleta)
    {
        toggleLabel.text = pantallaCompleta ? "Ventana" : "Pantalla Completa";
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
