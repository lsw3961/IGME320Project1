using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button narrativeButton;
    public Image narrativeImage;
    public Image menuImage;
    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void Narrative()
    {
        this.gameObject.SetActive(false);
        menuImage.gameObject.SetActive(false);
        narrativeButton.gameObject.SetActive(true);
        narrativeImage.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
