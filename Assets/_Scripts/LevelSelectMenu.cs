using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSelectMenu : MonoBehaviour
{
    public string mainMenu = "Main Menu";
   
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
}
