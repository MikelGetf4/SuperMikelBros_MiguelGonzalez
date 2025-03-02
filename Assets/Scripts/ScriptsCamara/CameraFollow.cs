using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject followObject; //Objeto a seguir por la camara
    public Vector2 followOffset; //
    public float speed = 3f; //Velocidad a la que va a seguir la camara
    Vector3 velocity = Vector3.zero;
    public Vector2 threshold; //Umbral a partir del cual se movera la camara
    private Rigidbody2D rb; //Rigibody de Toad

    void Start()
    {
        threshold = calculateThreshold(); // Calcula los límites de la camara
        rb = followObject.GetComponent<Rigidbody2D>(); //Obtiene el rigibody del objeto que va a seguir

    }

    void LateUpdate()
    {
        Vector2 follow = followObject.transform.position; // Obtiene la posicion de Toad
        Vector3 newPosition = transform.position; //Posicion de la camara actual

        if (follow.x > transform.position.x)
        {
            newPosition.x = follow.x;
        }

        newPosition.y = transform.position.y; // Mantiene la cámara en la misma altura

        float moveSpeed = rb.velocity.magnitude > speed ? rb.velocity.magnitude : speed; //Usa la velocidad del objeto si esta es mayor que la de la camara

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, 0.2f); //Mueve la camara SUAVEMENTE hacia la nueva posicion de Toad
    }

    private Vector2 calculateThreshold() //Calcula el umbral del limite de la camara
    {
        Vector2 t = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
        t.x -= followOffset.x;
        t.y -= followOffset.y;
        return t;
    }

    private void OnDrawGizmos() //Permite ver el Umbral a partir del cual se movera la camara en el modo SCENE
    {
        Gizmos.color = Color.green;
        Vector2 border = calculateThreshold();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
    }
}
