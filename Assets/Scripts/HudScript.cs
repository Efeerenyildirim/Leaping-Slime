using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HudScript : MonoBehaviour
{
    public GameObject slime;
    PlayerSwipe playerCode;
    int playersHealth = 3;

    public Sprite fullHeart;
    public Sprite hollowHeart;

    public GameObject[] hearths;
    SpriteRenderer[] spriteRenderers;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameoverMenu;
    [SerializeField] GameObject victoryMenu;
    public bool isPaused = false;

    bool stopInput = false;
    

    void Start()
    {
        spriteRenderers = new SpriteRenderer[3];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i] = hearths[i].GetComponent<SpriteRenderer>();
        }
        playerCode = slime.GetComponent<PlayerSwipe>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!stopInput) TogglePause();
        }

        playersHealth = playerCode.playersHealth;

        if (playersHealth == 3)
        {
            spriteRenderers[0].sprite = fullHeart;
            spriteRenderers[1].sprite = fullHeart;
            spriteRenderers[2].sprite = fullHeart;
        }
        else if (playersHealth == 2)
        {
            spriteRenderers[0].sprite = fullHeart;
            spriteRenderers[1].sprite = fullHeart;
            spriteRenderers[2].sprite = hollowHeart;
        }
        else if (playersHealth == 1)
        {
            spriteRenderers[0].sprite = fullHeart;
            spriteRenderers[1].sprite = hollowHeart;
            spriteRenderers[2].sprite = hollowHeart;
        }
        else if (playersHealth == 0)
        {
            spriteRenderers[0].sprite = hollowHeart;
            spriteRenderers[1].sprite = hollowHeart;
            spriteRenderers[2].sprite = hollowHeart;
        }

        if (playersHealth == 0)
        {
            stopInput = true;
            isPaused = true;
            gameoverMenu.SetActive(isPaused);
        }

        if(playerCode.playerWon) 
        {
            stopInput = true;
            isPaused = true;
            victoryMenu.SetActive(isPaused);
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

    }

    public void loadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void returnMenu()
    {
        SceneManager.LoadScene("menu");
    }
    public void continueGame()
    {
        TogglePause();
    }
    public void exitGame()
    {
        Application.Quit();
    }
}
