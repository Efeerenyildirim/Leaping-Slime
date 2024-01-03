using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MenuScript : MonoBehaviour
{
    public float moveSpeed = 7.0f;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        startPosition = new Vector3(-30f, 6f, 0f);
        transform.position = startPosition;
    }

    private void Update()
    {
        targetPosition = new Vector3(780f, 6f, 0f);
        float step = moveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (transform.position == targetPosition)
        {
            transform.position = startPosition;
        }
    }
}