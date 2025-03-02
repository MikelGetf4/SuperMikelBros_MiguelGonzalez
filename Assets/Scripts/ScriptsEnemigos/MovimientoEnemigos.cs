using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoEnemigos : MonoBehaviour, IEnemigos
{
    Rigidbody2D rb; //Rigibody del enemigo
    public float velocidad; //Velocidad a la que se movera el enemigo
    private float direccion = 1; //Direccion en la que empieza mirando
    public bool pause = true; //Si esta en pausa
    private bool isFalling = false; //Si esta cayendo
    private SpriteRenderer spriterender; //Su spriteRenderer

    public float velocidadActual; //La velocidad actual a la que se mueve

    private void Start() //Recibe varios elementos
    {
        rb = GetComponent<Rigidbody2D>();
        spriterender = rb.GetComponent<SpriteRenderer>();
        pause = true;
    }



    void FixedUpdate()
    {
        if (pause) //Si esta en pausa, no hacer nada
        {
            return;
        }

        if (Mathf.Abs(rb.velocity.x) < 0.1f && isFalling == false) //Al chocar con algo, su velocidad sera 0 y se girara. Ademas, no se girara al caer desde alto
        {
            direccion = -direccion;
            spriterender.flipX = !spriterender.flipX;
        }


        this.isFalling = Mathf.Abs(rb.velocity.y) > 0.1f;

        if (pause == false) //Si no esta en pausa, se movera
        {
            Movimiento();
        }
    }

    public void Movimiento() //Añade fuerzas para que este se mueva
    {

        this.rb.velocity = new Vector2(direccion * velocidad, this.rb.velocity.y);
    }

    public void Activar() //Lo activa para que vuelva a moverse
    {
        pause = false;
    }

    public void Pausa() //Deja su velocidad a 0 y lo marca como pausado
    {
        pause = true;
        velocidad = 0;
    }

    public void RecibirDanio() //IGNORAR
    {
        return;
    }
}

