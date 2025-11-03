using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuSettings : MonoBehaviour
{
    [SerializeField] private GameObject MainMenuObject;

    public void ClickingOnSettings()
    {
        Time.timeScale = 0;
        MainMenuObject.SetActive(true);
    }

    public void ResetGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
