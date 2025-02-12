using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMove pm;
    public Transform playerGRFX;
    public PlayerStamina playerStamina;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;

    [Header("DashCoolDown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Imput")]
    public KeyCode DashKey;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(DashKey) && playerStamina.canDashStam)
        {
            Dash();
        }

        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    // Dash
    private void Dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        pm.dashing = true;

        Transform forwardT;
        forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    // Delay Dash
    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    // Reset dash
    private void ResetDash()
    {
        pm.dashing = false;

        playerStamina.DashesToLooseStamina();
        playerStamina.didUseStamina = true;
    }

    // Get Direction
    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalImput = Input.GetAxisRaw("Horizontal");
        float verticalImput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        direction = forwardT.forward * verticalImput + forwardT.right * horizontalImput;

        if (verticalImput == 0 && horizontalImput == 0)
        {
            direction = playerGRFX.forward;
        }

        return direction.normalized;
    }
}
