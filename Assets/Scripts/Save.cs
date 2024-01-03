using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    public const string LEVEL_KEY = "PlayerLevel";

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void SavePlayerProgress(int level)
    {
        PlayerPrefs.SetInt(LEVEL_KEY, level);
        PlayerPrefs.Save();
    }

    public int LoadPlayerProgress()
    {
        if (PlayerPrefs.HasKey(LEVEL_KEY))
        {
            return PlayerPrefs.GetInt(LEVEL_KEY);
        }
        else
        {
            return 0;
        }
    }

    public void ResetPlayerProgress()
    {
        Debug.Log("reset");
        PlayerPrefs.DeleteKey(LEVEL_KEY);
        PlayerPrefs.Save();
    }
}
