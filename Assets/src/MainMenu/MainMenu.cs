using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Define Menus
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject multiplayerMenu;
    


    void Start()
    {
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (multiplayerMenu != null) multiplayerMenu.SetActive(false);
    }

    public void PlayGame() {
        // Do some other stuff here instead of instantly switching scenes. 
        SceneManager.LoadScene("Game");
    }

    public void openMultiplayerMenu() {
        Debug.Log("Multiplayer not implemented yet.");
    }

    public void openSettingsMenu() {
        Debug.Log("a");
    }

    public void QuitGame() {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
