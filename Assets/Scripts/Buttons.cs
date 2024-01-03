using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Buttons : MonoBehaviour
{
    [SerializeField] GameObject Menu1;
    [SerializeField] GameObject Menu2;

    [SerializeField] GameObject[] levelButtons;

    Save save;
    int playerLevel;

    void Start()
    {
        save = GetComponent<Save>();
        playerLevel = save.LoadPlayerProgress();
    }
    private void Update()
    {
        LevelButtonSaves();
    }

    public void LevelButtonSaves()
    {
        for (int i = 0; i <= playerLevel; i++)
        {
            levelButtons[i].gameObject.SetActive(true);
        }

        for (int i = 14; i > playerLevel; i--)
        {
            levelButtons[i].gameObject.SetActive(false);
        }
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void showLevels()
    {
        Menu1.SetActive(false);
        Menu2.SetActive(true);
    }

    public void showStart() 
    {
        Menu1.SetActive(true);
        Menu2.SetActive(false);
    }

    public void loadTutorial() 
    {
        SceneManager.LoadScene("tutorial");
    }

    public void loadLevel1() { SceneManager.LoadScene("level 1"); }
    public void loadLevel2() { SceneManager.LoadScene("level 2"); }
    public void loadLevel3() { SceneManager.LoadScene("level 3"); }
    public void loadLevel4() { SceneManager.LoadScene("level 4"); }
    public void loadLevel5() { SceneManager.LoadScene("level 5"); }

}
