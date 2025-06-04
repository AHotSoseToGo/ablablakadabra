using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public KeyCode PauseButton;
    public bool isPaused;

    public GameObject thirdPersonCam;
    public GameObject combatCam;
    public ThirdPersonCam thirdPersonCamScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(PauseButton) && isPaused)
        {
            isPaused = false;
        }
        else if (Input.GetKeyDown(PauseButton) && !isPaused)
        {
            isPaused = true;
        }
    }
}
