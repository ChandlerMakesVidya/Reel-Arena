using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

//this is modified from the RigidbodyFirstPersonController from the Unity Standard Assets available on the Asset Store
public class PlayerMovePhys : MonoBehaviour
{
    public float MaxGroundSpeed;
    private float MaxAirSpeed;
    public float GroundAcceleration;
    public float FrictionlessAcceleration;
    public float AirAcceleration;
    public float FrictionCoefficient;
    //public float GrappleSpeedModifier;
    public float minGrappleSpeed;
    private float GrappleSpeed;
    public float JumpForce;
    public AudioSource jumpSound;
    public AudioSource doubleJumpSound;
    //public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));

    public float maxGrappleDistance = 100.0f;
    public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
    //public GroundCheck groundCheck;
    public float stickToGroundHelperDistance = 0.5f; // stops the character
    public float slopeLimit = 45.0f;
    public LayerMask environmentMask;
    //public float slowDownRate = 128f; // rate at which the controller comes to a stop when there is no input
    public bool airControl = true; // can the user control the direction that is being moved in the air
    [Tooltip("set it to 0.1 or more if you get stuck in wall")]
    public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
    public float handbobMagnitude;
    public float handbobInterval;
    public GameObject hand;
    private float handbobMidpointX;
    private float handbobMidpointY;
    private float handbobTimer;
    public AnimationCurve yVelocityBob;

    public float successfulGrappleCooldown { get; private set; } = 2.0f;
    public float failedGrappleCooldown { get; private set; } = 0.3f;
    [HideInInspector] public float sgReset { get; private set; }
    [HideInInspector] public float fgReset { get; private set; }

    private Rigidbody m_RigidBody;
    private CapsuleCollider m_Capsule;
    private Vector3 desiredMove;
    private float m_YRotation;
    private Vector3 m_GroundContactNormal;
    private bool m_PreviouslyGrounded, m_Jumping, doubleJump, m_IsGrounded, m_onSteepSlope, isGrappling;
    private Vector3 grappleTarget, grappleDirection;
    private float speedCap;

    private LineRenderer grappleLine;
    private Camera cam;
    float rbMass;
    public Text Speedometer;
    public bool strictlyDisplayHorizontalVelocity;

    public Vector3 Velocity
    {
        get { return m_RigidBody.velocity; }
    }

    public Vector3 HorizontalVelocity
    {
        get { return new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z); }
    }

    public bool Grounded
    {
        get { return m_IsGrounded; }
    }

    public bool Jumping
    {
        get { return m_Jumping; }
    }


    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        grappleLine = GetComponent<LineRenderer>();
        cam = Camera.main;
        rbMass = m_RigidBody.mass;
        sgReset = successfulGrappleCooldown;
        fgReset = failedGrappleCooldown;
        successfulGrappleCooldown = 0f;
        failedGrappleCooldown = 0f;
        speedCap = MaxGroundSpeed;
        doubleJump = false;
        handbobMidpointX = hand.transform.localPosition.x;
        handbobMidpointY = hand.transform.localPosition.y;
        Speedometer = UIManager.UI.speedometer;
    }


    private void Update()
    {
        if (!m_IsGrounded && !m_onSteepSlope && !isGrappling)
        {
            if (Input.GetButtonDown("Jump") && !doubleJump)
            {
                doubleJump = true;
                if (m_RigidBody.velocity.y > 0f)
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, m_RigidBody.velocity.z);
                else
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0, m_RigidBody.velocity.z);
                m_RigidBody.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.VelocityChange);
                m_Jumping = true;
                doubleJumpSound.Play();
            }
        }

        if (successfulGrappleCooldown > 0f)
            successfulGrappleCooldown -= Time.deltaTime;
        else successfulGrappleCooldown = 0f;

        if (failedGrappleCooldown > 0f)
            failedGrappleCooldown -= Time.deltaTime;
        else failedGrappleCooldown = 0f;
        
        GrappleInput();
        //speedcap, to prevent acceleration to infinite speed
        Vector3 speed = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
        /*if (speed.sqrMagnitude > speedCap * speedCap)
        {
            speed.Normalize();
            speed *= speedCap;
            speed.y = m_RigidBody.velocity.y;
            m_RigidBody.velocity = speed;
        }*/

        //hand bob with move
        Vector2 input = GetInput();
        float waveslice = 0.0f;
        if(Mathf.Abs(input.x) == 0 && Mathf.Abs(input.y) == 0)
        {
            handbobTimer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(handbobTimer);
            handbobTimer += handbobInterval;
            if(handbobTimer > Mathf.PI * 2)
            {
                handbobTimer -= (Mathf.PI * 2);
            }
        }
        if(waveslice != 0.0f && m_IsGrounded)
        {
            float translateChange = waveslice * handbobMagnitude;
            float totalAxes = Mathf.Abs(input.x) + Mathf.Abs(input.y);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange *= totalAxes;
            Vector3 handLocalPos = hand.transform.localPosition;
            handLocalPos.x = handbobMidpointX + translateChange;
            handLocalPos.y = handbobMidpointY + Mathf.Abs(translateChange);
            hand.transform.localPosition = handLocalPos;
        }
        else
        {
            hand.transform.localPosition = new Vector3(
                Mathf.Lerp(hand.transform.localPosition.x, handbobMidpointX, 0.1f),
                Mathf.Lerp(hand.transform.localPosition.y, handbobMidpointY, 0.1f),
                hand.transform.localPosition.z);
        }

        //rotate hand up and down based on y velocity
        float angle = yVelocityBob.Evaluate(Mathf.Abs(Velocity.y));
        angle = Velocity.y >= 0.0f ? angle : -angle;
        Quaternion rot = Quaternion.Euler(angle, 0.0f, 0.0f);
        hand.transform.localRotation = Quaternion.Lerp(hand.transform.localRotation, rot, 0.3f);
    }


    private void FixedUpdate()
    {
        Vector2 horizonalVelocity = new Vector2(m_RigidBody.velocity.x, m_RigidBody.velocity.z);
        if (!isGrappling)
        {
            GroundCheck();
            Vector2 input = GetInput();
            MaxAirSpeed = horizonalVelocity.magnitude;

            if (m_IsGrounded)
            {
                if(Randomizer.FrictionModifier != 0.0f && m_PreviouslyGrounded) GroundFriction(MaxGroundSpeed, FrictionCoefficient);
                speedCap = Mathf.Lerp(speedCap, MaxGroundSpeed * Randomizer.PlayerSpeedModifier, 4f * Time.fixedDeltaTime);
                //m_RigidBody.drag = 5f * Randomizer.FrictionModifier;
                doubleJump = false;
                if (Input.GetButton("Jump"))
                {
                    //m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.VelocityChange);
                    m_Jumping = true;
                    jumpSound.Play();
                }
                
                /*if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 0.5f)
                {
                    m_RigidBody.Sleep();
                }*/
            }
            else if (m_onSteepSlope)
            {
                //m_RigidBody.drag = 0f;

                if (Input.GetButton("Jump"))
                {
                    if (m_RigidBody.velocity.y > 0f)
                        m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, m_RigidBody.velocity.z);
                    else
                        m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce((m_GroundContactNormal.normalized * JumpForce), ForceMode.VelocityChange);
                    m_Jumping = true;
                    jumpSound.Play();
                }
            }
            else
            {
                //m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon))
            {
                desiredMove = transform.forward * input.y + transform.right * input.x;

                //desiredMove.x = desiredMove.x * MovementSpeed;
                //desiredMove.z = desiredMove.z * MovementSpeed;
                desiredMove.y = 0.0f; //desiredMove.y * MovementSpeed;
                if (!m_onSteepSlope)
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
                Debug.DrawRay(transform.position, desiredMove, Color.green);
                /*ForceMode fm;
                float accel;
                if (Randomizer.FrictionModifier == 1.0f)
                {
                    fm = ForceMode.VelocityChange;
                    accel = GroundAcceleration;
                }
                else
                {
                    fm = ForceMode.Acceleration;
                    accel = FrictionlessAcceleration;
                }*/

                if (m_IsGrounded)
                {
                    //m_RigidBody.AddForce(desiredMove * accel, fm);
                    //print("ground");
                    Acceleration(desiredMove, MaxGroundSpeed * Randomizer.PlayerSpeedModifier, 
                        Randomizer.FrictionModifier != 0.0f ? GroundAcceleration : FrictionlessAcceleration);
                }
                else if (!m_IsGrounded)
                {
                    //m_RigidBody.AddForce(desiredMove * AirAcceleration, ForceMode.VelocityChange);
                    //print("air");
                    Acceleration(desiredMove, MaxGroundSpeed * Randomizer.PlayerSpeedModifier, AirAcceleration);
                    //Acceleration(desiredMove, AirAcceleration, 32.0f);
                }
            }
        }

        //print(speedCap);
        Speedometer.text = !strictlyDisplayHorizontalVelocity ? Velocity.magnitude.ToString("F1") + "m/s" : HorizontalVelocity.magnitude.ToString("F1") + "m/s";
    }

    private void GrappleInput()
    {
        if (Input.GetButtonDown("Grapple"))
        {
            GrappleTry();
        }

        if (isGrappling)
        {
            Grapple();
        }

        if(Input.GetButtonUp("Grapple") && isGrappling)
        {
            isGrappling = false;
            grappleLine.enabled = false;
        }
    }

    private void GrappleTry()
    {
        GrappleSpeed = m_RigidBody.velocity.magnitude > minGrappleSpeed ? m_RigidBody.velocity.magnitude : minGrappleSpeed;
        if(successfulGrappleCooldown == 0f && failedGrappleCooldown == 0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxGrappleDistance, environmentMask))
            {
                isGrappling = true;
                grappleTarget = hit.point;
                grappleLine.SetPosition(1, grappleTarget);
                grappleLine.enabled = true;
                //m_RigidBody.drag = 0f;
            }
            else
            {
                failedGrappleCooldown = fgReset;
            }
        }
    }

    private void Grapple()
    {
        grappleDirection = (grappleTarget - transform.position).normalized;
        grappleLine.SetPosition(0, transform.position);
        float velocity = m_RigidBody.velocity.magnitude;
        m_RigidBody.velocity = grappleDirection * GrappleSpeed;
        successfulGrappleCooldown = sgReset;
        speedCap = m_RigidBody.velocity.magnitude;
        doubleJump = false;
        //print(m_RigidBody.velocity.magnitude);
        if(Vector3.Distance(transform.position, grappleTarget) < 2.0f)
        {
            isGrappling = false;
            grappleLine.enabled = false;
        }
    }


    /*private float SlopeMultiplier()
    {
        float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
        return SlopeCurveModifier.Evaluate(angle);
    }*/


    private void StickToGroundHelper()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
                               ((m_Capsule.height / 2f) - m_Capsule.radius) +
                               stickToGroundHelperDistance, environmentMask, QueryTriggerInteraction.Ignore))
        {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
            }
        }
    }


    private Vector2 GetInput()
    {

        Vector2 input = new Vector2
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = Input.GetAxisRaw("Vertical")
        };
        return input;
    }

    /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
    private void GroundCheck()
    {
        m_PreviouslyGrounded = m_IsGrounded;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
                               ((m_Capsule.height / 2f) - m_Capsule.radius) + groundCheckDistance, environmentMask, QueryTriggerInteraction.Ignore))
        {
            m_GroundContactNormal = hitInfo.normal;
            if (Vector3.Angle(m_GroundContactNormal, Vector3.up) < slopeLimit)
            {
                m_IsGrounded = true;
                m_onSteepSlope = false;
            } else
            {
                m_IsGrounded = false;
                m_onSteepSlope = true;
            }
        }
        else
        {
            m_IsGrounded = false;
            m_onSteepSlope = false;
            m_GroundContactNormal = Vector3.up;
        }
        if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
        {
            m_Jumping = false;
        }
    }

    void Acceleration(Vector3 WishDir, float WishSpeed, float Accel)
    {
        float currentSpeed = Vector3.Dot(WishDir, Velocity);
        float addSpeed = WishSpeed - currentSpeed;
        if (addSpeed <= 0.0f)
        {
            return;
        }

        float accelerationSpeed = Mathf.Min(Accel * WishSpeed / PhysStepRate(), addSpeed);
        m_RigidBody.velocity += accelerationSpeed * WishDir;
    }

    void GroundFriction(float StopSpeed, float FrictionCoefficient)
    {
        float speed = Velocity.magnitude;
        float drop, control;

        control = speed < StopSpeed ? StopSpeed : speed;
        drop = control * FrictionCoefficient / PhysStepRate();

        float NewSpeed = (speed - drop <= 0 ? 0 : speed - drop) / speed;
        if (float.IsNaN(NewSpeed)) NewSpeed = 0.0f;
        Vector3 vel = new Vector3(
            m_RigidBody.velocity.x * NewSpeed,
            m_RigidBody.velocity.y,
            m_RigidBody.velocity.z * NewSpeed);
        m_RigidBody.velocity = vel;
    }

    float Framerate()
    {
        return Time.frameCount / Time.time;
    }

    float PhysStepRate()
    {
        return 1.0f / Time.fixedDeltaTime;
    }
}

    /*public float movementSpeed = 16.0f;
    public float grappleSpeed = 32.0f;
    public float groundAcceleration = 32.0f;
    public float airAcceleration = 5.0f;
    public float jumpSpeed = 8.0f;
    public float slopeLimit = 45f;
    public float groundCheckRadius = 0.4f;

    float inputX, inputY;
    bool jumpInput, grappleInput;
    Vector3 moveVector;
    Vector3 velocity;
    float speed;

    bool groundedLastFrame;
    Vector3 groundNormal;
    bool grappling;
    float fallSpeed;
    Dictionary<int, ContactPoint[]> contactPoints;
    
    Rigidbody rb;
    CapsuleCollider coll;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask environmentMask;

    bool Grounded()
    {
        groundNormal = Vector3.zero;
        //first check, if anything in the environmentMask is in the groundCheck sphere;
        bool sphereCheck = Physics.CheckSphere(groundCheck.position, groundCheckRadius, environmentMask);
        if (!sphereCheck) return false;
        //else continue

        //now we check if the slope is too steep to stand on.
        RaycastHit hit;
        bool g = false;
        foreach(ContactPoint[] contacts in contactPoints.Values)
        {
            for(int i = 0; i < contacts.Length; i++)
            {
                if(Physics.Raycast(contacts[i].point + Vector3.up, Vector3.down, out hit, 1.1f))
                {
                    if(Vector3.Angle(hit.normal, Vector3.up) <= slopeLimit)
                    {
                        //groundNormal += hit.normal;
                        g = true;
                    }
                }
            }
        }
        if (g) return true;
        else return false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();

        moveVector = Vector3.zero;
        groundedLastFrame = false;
        groundNormal = Vector3.zero;
        grappling = false;
        contactPoints = new Dictionary<int, ContactPoint[]>();
    }

    private void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        jumpInput = Input.GetButtonDown("Jump");

        if (jumpInput)
        {
            if (Grounded())
            {
                rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);
            }
        }

        Vector3 forwardMove = transform.forward * inputY;
        Vector3 rightMove = transform.right * inputX;
        moveVector = (forwardMove + rightMove).normalized;

        velocity = rb.velocity;
        velocity.y = 0f;
        speed = Vector3.Magnitude(velocity);
    }

    private void FixedUpdate()
    {
        if (Grounded())
        {
            rb.drag = 5f;
            rb.AddForce(moveVector * groundAcceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
        else
        {
            rb.drag = 0f;
            rb.AddForce(moveVector * airAcceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        Debug.Log(Grounded() + " " + speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        contactPoints.Add(collision.gameObject.GetInstanceID(), collision.contacts);
    }

    private void OnCollisionStay(Collision collision)
    {
        contactPoints[collision.gameObject.GetInstanceID()] = collision.contacts;
    }

    private void OnCollisionExit(Collision collision)
    {
        contactPoints.Remove(collision.gameObject.GetInstanceID());
    }*/
