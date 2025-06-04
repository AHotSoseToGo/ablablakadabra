using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightMenuScript : MonoBehaviour
{
    public void StartFight()
    {
        SceneManager.LoadScene("TsetGameScene");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainManuScene");
    }
}
