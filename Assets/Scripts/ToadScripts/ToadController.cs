using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadController : MonoBehaviour
{
    [SerializeField] private float direction;
    private Rigidbody2D rb;

    [SerializeField]    private float fuerzaAceleracion = 12f;
    [SerializeField]    private float fuerzaDeceleracion = 20f;
    [SerializeField]    private float maximaVelocidad = 5f;
    [SerializeField]    private float fuerzaSalto = 11f;
    [SerializeField]    private float anchoDePersonaje;
    [SerializeField]    private float distanciaRaycast;
    [SerializeField]    private LayerMask capaSuelo;
    [SerializeField] private float fuerzaSaltoFinal;
    [SerializeField] private float tiempoSaltoMaximo = 1f; // Tiempo máximo de salto sostenido
    [SerializeField] private float multiplicadorGravedad = 4f; // Gravedad extra al soltar el salto
    [SerializeField] private float multiplicadorGravedadCaida = 6f; // Gravedad aumentada al caer

    private bool isRunning;
    private bool estaSaltando;
    private float tiempoSaltoActual;


    public bool estaEnSuelo;
    public bool agachado;
    public bool muerto = false;

    private ToadStatus estado = ToadStatus.Small;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        muerto = false;
    }

    private void Update()
    {
        this.direction = Input.GetAxisRaw("Horizontal");
        // Mantener isRunning en true si está corriendo o si está en el aire.
        if (!estaEnSuelo)
        {
            // Si está en el aire, no desactivar isRunning, solo mantenerlo activo si se está moviendo
            if (Input.GetKey(KeyCode.LeftShift) && direction != 0)
            {
                this.isRunning = true;
            }
        }
        else
        {
            // Si está en el suelo, comprobar si está presionando el Shift para correr
            this.isRunning = Input.GetKey(KeyCode.LeftShift);
        }

        ComprobarSuelo();
        if (muerto == false)
        {
            Saltar();

            if (estaEnSuelo == true)
            {
                Agachado();
            }
        }
    }

    private void FixedUpdate()
    {
        if (this.agachado == false && this.muerto == false)
        {
            Movimiento();
        }
        else
        {
            float nuevaVelocidadX = Mathf.Lerp(rb.velocity.x, 0, Time.fixedDeltaTime * 5f);
            rb.velocity = new Vector2(nuevaVelocidadX, rb.velocity.y);
        }
    }

    private void Movimiento()
    {
        float fuerzaAceleracionFinal = this.fuerzaAceleracion;
        float fuerzaDeceleracionFinal = this.fuerzaDeceleracion;
        float maximaVelocidadFinal = this.maximaVelocidad;
        if (this.isRunning)
        {
            fuerzaAceleracionFinal = this.fuerzaAceleracion * 2;
            maximaVelocidadFinal = this.maximaVelocidad * 2;
        }

        if (Mathf.Abs(this.rb.velocity.y) > 0.1f)
        {
            fuerzaAceleracionFinal = this.fuerzaAceleracion / 4;
            fuerzaDeceleracionFinal = this.fuerzaDeceleracion / 4;
        }

        //Aceleracion
        var fuerzaAAplicarAToad = new Vector2(this.direction, 0) * fuerzaAceleracionFinal;

        //Frenado
        if (direction == 0 && Mathf.Abs(this.rb.velocity.x) > 0.1)
        {
            fuerzaAAplicarAToad = new Vector2 (Mathf.Sign(this.rb.velocity.x) * fuerzaDeceleracionFinal * -1, 0);
        }
        if(direction != 0 && Mathf.Sign(direction) != Mathf.Sign(this.rb.velocity.x))
        {
            fuerzaAAplicarAToad = new Vector2 (Mathf.Sign(this.rb.velocity.x) * fuerzaDeceleracionFinal * -1, 0);
        }

        // Parando
        if (Mathf.Abs(this.rb.velocity.x) < 0.1f && direction == 0)
            this.rb.velocity = new Vector2(0, this.rb.velocity.y);

        this.rb.AddForce(fuerzaAAplicarAToad);

        if (Mathf.Abs(this.rb.velocity.x) > maximaVelocidadFinal)
        {
            this.rb.velocity = new Vector2(Mathf.Clamp(this.rb.velocity.x, -maximaVelocidadFinal, maximaVelocidadFinal), this.rb.velocity.y);
        }

        // Girar personaje hacia donde mire
        if (direction != 0 && estaEnSuelo == true)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1);
        }


    }

    private void ComprobarSuelo()
    {
        bool estaEnSueloDerecho = Physics2D.Raycast(transform.position + Vector3.right * anchoDePersonaje, Vector2.down, distanciaRaycast, capaSuelo);
        bool estaEnSueloIzquierdo = Physics2D.Raycast(transform.position + Vector3.left * anchoDePersonaje, Vector2.down, distanciaRaycast, capaSuelo);
        estaEnSuelo = estaEnSueloDerecho || estaEnSueloIzquierdo;

        if (estaEnSuelo)
        {
            estaSaltando = false;
            tiempoSaltoActual = 0;
        }
    }

    private void Saltar()
    {
        if (Input.GetButtonDown("Jump") && estaEnSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            estaSaltando = true;
            tiempoSaltoActual = 0;
        }

        if (Input.GetButton("Jump") && estaSaltando && tiempoSaltoActual < tiempoSaltoMaximo)
        {
            rb.velocity += new Vector2(0, 0.2f); // Pequeño impulso mientras se mantiene presionado
            tiempoSaltoActual += Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            estaSaltando = false;
        }

        // Ajuste de gravedad
        if (rb.velocity.y < 0) // Si está cayendo, aplicar más gravedad
        {
            rb.velocity += Vector2.down * multiplicadorGravedadCaida * Time.deltaTime;
        }
        else if (!estaSaltando) // Si suelta el botón antes de tiempo, cae más rápido
        {
            rb.velocity += Vector2.down * multiplicadorGravedad * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // Raycast izquierdo
        Gizmos.DrawRay(transform.position + Vector3.left * anchoDePersonaje, Vector2.down * distanciaRaycast);

        // Raycast derecho
        Gizmos.DrawRay(transform.position + Vector3.right * anchoDePersonaje, Vector2.down * distanciaRaycast);
    }

    private void Agachado()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            agachado = true;
        }
        else
        {
            agachado = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Muerte"))
        {
            Damagable();
        }
    }

    public void Damagable()
    {
        switch (this.estado)
        {
            case ToadStatus.Small:
                Debug.Log("Toad... ha muerto");
                Death();
                break;
            case ToadStatus.Mushroom:
                estado = ToadStatus.Mushroom;
                break;
            case ToadStatus.Flower:
                estado = ToadStatus.Mushroom;
                break;
        }
    }

    private void Death()
    {
        muerto = true;

        gameObject.layer = LayerMask.NameToLayer("ToadMuerto");

        // Eliminar todas las fuerzas y detener el movimiento
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Aplicar un pequeño salto (impulso hacia arriba)
        rb.velocity = new Vector2(0, 5f); // Puedes ajustar el valor 5f según necesites

        // Restaurar la gravedad para que caiga después del impulso
        rb.gravityScale = 2f; // Ajusta según la gravedad normal del juego

    }

}

public enum ToadStatus
{
    Small,
    Mushroom,
    Flower
}