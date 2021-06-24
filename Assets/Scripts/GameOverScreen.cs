using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public Text statusText;
    public void Setup(bool hasWon)
    {
        gameObject.SetActive(true);
        if (hasWon)
        {
            Debug.Log("Game over, player win!");
            statusText.text = "You Win!";
        }
        else
        {
            Debug.Log("Game over, player lose!");
            statusText.text = "You Lose!";
        }
    }

    public void NextButton()
    {
        SceneManager.LoadScene("GamePlayCheat");
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
