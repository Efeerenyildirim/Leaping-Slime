using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 1f;
    public Vector3 offset;

    public bool follow = false;
    public bool isJumpedOnce = false;

    public float smoothTime = 0.1f;
    public float forwardSpeed = 4.0f;

    private Vector3 velocity = Vector3.zero;

    [SerializeField] GameObject canvas;
    private HudScript hudScript;

    void Start()
    {
        hudScript = canvas.GetComponent<HudScript>();

    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isJumpedOnce = true;
        }

        if (follow == true || isJumpedOnce == false)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else if (follow == false && isJumpedOnce == true)
        {
            if (!hudScript.isPaused)
            {
                float posY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref velocity.y, smoothTime);
                float posX = transform.position.x + forwardSpeed * Time.deltaTime;
                transform.position = new Vector3(posX, posY, transform.position.z);
            }
        }
    }
}
