using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
public void LoadScene(string sceneName)
{
Time.timeScale = 1;
SceneManager.LoadScene(sceneName);
}
public void TogglePause() { GameManager.Instance.TogglePause(); }
public void QuitGame()
{
Debug.Log("Quit Game");
Application.Quit();
}
}