using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovingPlatform : MonoBehaviour
{
    public float moveDistance = 3f; // Khoảng cách di chuyển
    public float speed = 2f;        // Tốc độ

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.PingPong(Time.time * speed, moveDistance);
        transform.position = startPos + Vector3.up * yOffset;
    }
}