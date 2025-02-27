using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoChampiñon : MonoBehaviour
{
    Rigidbody2D rb;
    public float velocidad;
    private float direccion = 1;
    public bool pause = true;
    private bool isFalling = false;

    public float velocidadActual;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pause = false;
    }



    void FixedUpdate()
    {
        if (pause)
        {
            return;
        }

        if (Mathf.Abs(rb.velocity.x) < 0.1f && isFalling == false)
        {
            direccion = -direccion;
        }


        this.isFalling = Mathf.Abs(rb.velocity.y) > 0.1f;

        if (pause == false)
        {
            Movimiento();
        }
    }

    public void Movimiento()
    {

        this.rb.velocity = new Vector2(direccion * velocidad, this.rb.velocity.y);
    }

    public void Activar()
    {
        pause = false;
    }

    public void Pausa()
    {
        pause = true;
        velocidad = 0;
    }
}

