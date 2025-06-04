using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolBarMenu : MonoBehaviour
{
    public Transform theMiddle;
    public Transform theLeft;
    public Transform theRight;
    public Transform movables;
    public float slideSpeed = 2f;
    public bool moveToFight;
    public bool moveToInventory;
    public bool moveToShop;

    public void PressFightToolBarButton()
    {
        moveToFight = true;
        moveToInventory = false;
        moveToShop = false;
    }

    public void PressInventoryToolBarButton()
    {
        moveToFight = false;
        moveToInventory = true;
        moveToShop = false;
    }

    public void PressShopToolBarButton()
    {
        moveToFight = false;
        moveToInventory = false;
        moveToShop = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(moveToFight)
            movables.position = Vector3.Slerp(movables.position, theMiddle.position, Time.deltaTime * slideSpeed);

        if (moveToInventory)
            movables.position = Vector3.Slerp(movables.position, theLeft.position, Time.deltaTime * slideSpeed);

        if (moveToShop)
            movables.position = Vector3.Slerp(movables.position, theRight.position, Time.deltaTime * slideSpeed);
    }
}
