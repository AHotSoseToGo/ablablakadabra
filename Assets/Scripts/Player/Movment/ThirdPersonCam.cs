using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public PlayerHealth playerHealth;

    public KeyCode Swich;

    public float rotationSpeed;

    public Transform combatLookAt;

    public CamStyle currentCamStyle;
    public enum CamStyle
    {
        Basic,
        Combat
    }

    public GameObject thirdPersonCam;
    public GameObject combatCam;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.isDed)
        {
            return;
        }

        if (Input.GetKeyDown(Swich)) SwichCameraStyles(CamStyle.Combat);
        if (Input.GetKeyUp(Swich)) SwichCameraStyles(CamStyle.Basic);

        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        if (currentCamStyle == CamStyle.Basic)
        {
            float horizontalImput = Input.GetAxisRaw("Horizontal");
            float verticalImput = Input.GetAxisRaw("Vertical");
            Vector3 inputDir = orientation.forward * verticalImput + orientation.right * horizontalImput;

            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }

        else if (currentCamStyle == CamStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = Vector3.Slerp(playerObj.forward, dirToCombatLookAt.normalized, Time.deltaTime * rotationSpeed);
        }
    }

    private void SwichCameraStyles(CamStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);

        if (newStyle == CamStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CamStyle.Combat) combatCam.SetActive(true);

        currentCamStyle = newStyle;
    }
}
