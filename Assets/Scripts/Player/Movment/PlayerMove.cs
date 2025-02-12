using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform orientation;
    public Transform dashDirection;
    public Transform groundCheckObj;
    [Space]
    public PlayerStamina playerStamina;
    public PlayerHealth playerHealth;

    [Header("Inputs")]
    public KeyCode Run;
    public KeyCode JumpButton;
    public KeyCode RightClick;

    [Header("GroundCheck")]
    public float playerHaight;
    public LayerMask theGround;
    public bool grounded;
    public float groundDrag;
    private float groundDistance = 0.2f;

    [Header("Movment")]
    [SerializeField] float speed;
    [SerializeField] float speedExeleration;
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public bool hasMovmant;
    [Space]
    public float jumpForce = 3f;
    public float jumpCoolDown;
    public float airMultiplier;
    public float gravity = 9.81f;
    [Space]
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public bool dashing;
    [Space]
    public float targetSpeed;
    public float actuallSpeed;

    public MovmentStates moveState;

    public enum MovmentStates
    {
        walking,
        sprinting,
        air,
        dashing
    }

    [Header("Slope Handler")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    bool exitingSlope = false;

    bool readyToJump = true;
    float gravityScale;

    float horizontalImput; // Extra Falsch geschrieben
    float verticalImput; // Extra Falsch geschrieben

    Vector3 moveDirection;


    //[SerializeField]
    //[Space]

    // Start is called before the first frame update
    void Start()
    {
        speed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.isDed)
        {
            rb.freezeRotation = false;
            return;
        }

        ImputManager();
        StateHandler();
        GroundChecking();
        SpeedControll();
        TargetSpeedControll();

        if ((moveState == MovmentStates.walking || moveState == MovmentStates.sprinting) && !Input.GetKey(JumpButton))
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    // Update id called 50 times in a second
    void FixedUpdate()
    {
        if (playerHealth.isDed)
            return;

        MovePlayer();
        Gravity();
    }

    // Gets all the Imputs
    private void ImputManager()
    {
        // Movment imput
        horizontalImput = Input.GetAxisRaw("Horizontal");
        verticalImput = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = orientation.forward * verticalImput + orientation.right * horizontalImput;
        if (inputDir.magnitude >= 0.1f)
            hasMovmant = true;
        else
            hasMovmant = false;

        // Jump Imput
        if (Input.GetKey(JumpButton) && readyToJump && grounded && playerStamina.canJumpStam)
        {
            readyToJump = false;
            exitingSlope = true;

            Jump();

            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    public float desiriedMoveSpeed;
    public float lastdesiredMoveSpeed;
    private MovmentStates lastState;
    public bool keepMomentum;

    // State handler
    void StateHandler()
    {
        // Dashing Mode
        if (dashing)
        {
            moveState = MovmentStates.dashing;
            desiriedMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        // Sprint Mode
        else if (grounded && Input.GetKey(Run) && !Input.GetKey(RightClick) && playerStamina.canRunStam)
        {
            moveState = MovmentStates.sprinting;
            desiriedMoveSpeed = runSpeed;
            targetSpeed = runSpeed;
        }

        // Walk Mode
        else if (grounded && (!Input.GetKey(Run) || !Input.GetKey(RightClick)))
        {
            moveState = MovmentStates.walking;
            desiriedMoveSpeed = walkSpeed;
            targetSpeed = walkSpeed;
        }
        
        // air Mode
        else
        {
            moveState = MovmentStates.air;

            if (desiriedMoveSpeed < runSpeed)
                desiriedMoveSpeed = walkSpeed;
            else
                desiriedMoveSpeed = runSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiriedMoveSpeed != lastdesiredMoveSpeed;
        if (lastState == MovmentStates.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                speed = desiriedMoveSpeed;
            }
        }

        lastdesiredMoveSpeed = desiriedMoveSpeed;
        lastState = moveState;
    }

    // smoothlerp movmentSpeed to desired speed
    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiriedMoveSpeed - speed);
        float startValue = speed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            speed = Mathf.Lerp(startValue, desiriedMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        speed = desiriedMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    // Smooth transition between Running and sprinting
    private void TargetSpeedControll()
    {
        if (targetSpeed > actuallSpeed)
            actuallSpeed += speedExeleration;
        if (targetSpeed < actuallSpeed)
            actuallSpeed -= speedExeleration;

        if (!dashing || !keepMomentum && moveState != MovmentStates.dashing)
            speed = actuallSpeed;
    }

    // Moves the player in funky ways
    private void MovePlayer()
    {
        if (moveState == MovmentStates.dashing) return;

        // Moving Player
        moveDirection = orientation.forward * verticalImput + orientation.right * horizontalImput;

        // for Slopes
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * speed * 10f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if (grounded)
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);

        if (!grounded)
            rb.AddForce(moveDirection.normalized * walkSpeed * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    // Jump
    void Jump()
    {
        rb.drag = 0;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
        rb.drag = groundDrag;

        playerStamina.JumpToLooseStamina();
        playerStamina.didUseStamina = true;
    }

    // Ground Check
    void GroundChecking()
    {
        grounded = Physics.CheckSphere(groundCheckObj.position, groundDistance, theGround);
    }

    // Speed Controll
    void SpeedControll()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if ((flatVel.magnitude > speed))
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    // Gravity
    void Gravity()
    {
        if (!grounded)
        {
            gravityScale = gravity + gravityScale;

            rb.AddForce(transform.up.normalized * (-gravityScale), ForceMode.Force);
        }
        else
            gravityScale = 0;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHaight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
