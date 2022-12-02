using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float movementSpeed = 16.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = -9.81f;
    public float grappleSpeed;
    public float maxGrappleDistance;
    public Vector3 grappleTarget;
    public Vector3 grappleDirection;
    public RaycastHit hit;
    public LayerMask grapplableSurfaces;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    bool isGrounded;
    bool isGrappling;

    private Vector3 moveVector = Vector3.zero;
    private Vector3 jumpVelocity;

    private Camera cam;
    private CharacterController charController;
    private LineRenderer grappleLine;

    private void Awake()
    {
        cam = Camera.main;
        charController = GetComponent<CharacterController>();
        grappleLine = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        PlayerMovement();
        GrappleInput();
    }

    private void PlayerMovement()
    {
        if (!isGrappling)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            float vertInput = Input.GetAxis("Vertical");
            float horiInput = Input.GetAxis("Horizontal");
            bool jumpInput = Input.GetButton("Jump");

            Vector3 forwardMovement = transform.forward * vertInput;
            Vector3 rightMovement = transform.right * horiInput;
            //moveVector = Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f);
            moveVector = (forwardMovement + rightMovement) * movementSpeed;

            if (isGrounded && jumpVelocity.y < 0)
            {
                charController.slopeLimit = 45.0f;
                jumpVelocity.y = -2f;
            }

            if (jumpInput && isGrounded)
            {
                charController.slopeLimit = 90.0f;
                jumpVelocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
            }

            jumpVelocity.y += gravity * Time.deltaTime;

            charController.Move((moveVector + jumpVelocity) * Time.deltaTime);
        }
    }

    private void GrappleInput()
    {
        if (Input.GetButtonDown("Grapple"))
        {
            Grapple();
        }

        if (isGrappling)
        {
            GrappleMove();
        }

        if(Input.GetButtonUp("Grapple") && isGrappling)
        {
            isGrappling = false;
            grappleLine.enabled = false;
            jumpVelocity.y = -2f;
        }
    }

    private void Grapple()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, maxGrappleDistance, grapplableSurfaces))
        {
            isGrounded = false;
            isGrappling = true;
            grappleTarget = hit.point;
            grappleLine.enabled = true;
            grappleLine.SetPosition(1, grappleTarget);
            grappleDirection = (grappleTarget - transform.position).normalized;
        }
    }

    private void GrappleMove()
    {
        charController.Move(grappleDirection * grappleSpeed * Time.deltaTime);
        grappleLine.SetPosition(0, transform.position);

        if (Vector3.Distance(transform.position, grappleTarget) < 2f)
        {
            isGrappling = false;
            grappleLine.enabled = false;
            jumpVelocity.y = -2f;
        }
    }

    public bool Grounded()
    {
        return isGrounded;
    }

    public float GetVelocity()
    {
        return charController.velocity.magnitude;
    }
}
