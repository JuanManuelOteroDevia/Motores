using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SelectorLevel");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Opciones");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Creditos");
    }
}
