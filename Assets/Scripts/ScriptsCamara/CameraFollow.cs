using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject followObject;
    public Vector2 followOffset;
    public float speed = 3f;
    Vector3 velocity = Vector3.zero;
    public Vector2 threshold;
    private Rigidbody2D rb;



    // Start is called before the first frame update
    void Start()
    {
        threshold = calculateThreshold();
        rb = followObject.GetComponent<Rigidbody2D>();

    }

    void LateUpdate()
    {
        Vector2 follow = followObject.transform.position;
        Vector3 newPosition = transform.position;

        if (follow.x > transform.position.x)
        {
            newPosition.x = follow.x;
        }

        newPosition.y = transform.position.y;

        float moveSpeed = rb.velocity.magnitude > speed ? rb.velocity.magnitude : speed;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, 0.2f); ;
    }

    private Vector2 calculateThreshold()
    {
        Vector2 t = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        t.x -= followOffset.x;
        t.y -= followOffset.y;
        return t;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 border = calculateThreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
    }
}
