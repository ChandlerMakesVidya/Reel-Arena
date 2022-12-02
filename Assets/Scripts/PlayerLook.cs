using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform viewPunch;
    [SerializeField] private float viewTilt = 3.5f;
    [SerializeField] private float viewStablizationRate = 0.5f;

    private float xAxisClamp;

    private void Awake()
    {
        xAxisClamp = 0.0f;
    }

    private void Update()
    {
        CameraRotation();
        TiltView();
        if(viewPunch.localEulerAngles != Vector3.zero)
        {
            StablizeView();
        }
    }

    private void CameraRotation()
    {
        float mX = Input.GetAxisRaw("Mouse X") * GameManager.GM.mouseSensitivity * Time.deltaTime;
        float mY = Input.GetAxisRaw("Mouse Y") * GameManager.GM.mouseSensitivity * Time.deltaTime;

        xAxisClamp += mY;

        if(xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        playerHead.Rotate(Vector3.left * mY);
        playerBody.Rotate(Vector3.up * mX);
    }

    private void TiltView()
    {
        float horiInput = Input.GetAxisRaw("Horizontal");
        float tilt = -horiInput * viewTilt;
        Vector3 t = new Vector3(0f, 0f, tilt);
        Vector3 c = new Vector3(0f, 0f, Mathf.LerpAngle(transform.localEulerAngles.z, t.z, 5f * Time.deltaTime));

        transform.localEulerAngles = c;
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = playerHead.eulerAngles;
        eulerRotation.x = value;
        playerHead.eulerAngles = eulerRotation;
    }

    private void StablizeView()
    {
        viewPunch.localEulerAngles = new Vector3(
            Mathf.LerpAngle(viewPunch.localEulerAngles.x, 0f, viewStablizationRate * Time.deltaTime),
            Mathf.LerpAngle(viewPunch.localEulerAngles.y, 0f, viewStablizationRate * Time.deltaTime),
            Mathf.LerpAngle(viewPunch.localEulerAngles.z, 0f, viewStablizationRate * Time.deltaTime));

    }

    public void PunchView(float force)
    {
        /*float xforce = Random.Range(-force, force);
        float yforce = Random.Range(-force, force);
        float zforce = Random.Range(-force, force);*/
        //viewPunch.Rotate(new Vector3(_force, _force, _force));
        viewPunch.localEulerAngles = new Vector3(-force, 0f, 0f);
    }
}
