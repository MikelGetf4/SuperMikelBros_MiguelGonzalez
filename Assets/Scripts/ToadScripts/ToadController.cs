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

    public ToadStatus estado;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool puedeActivarPowerUp = true;

    private bool invulnerable = false;
    private int statusAnimator = 0;
    private float lastDamageTime = 0f;
    private float invulnerabilityDuration = 2f;
    private float blinkRate = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        animator = rb.GetComponent<Animator>();
    }

    private void Start()
    {
        muerto = false;
        this.estado = ToadStatus.Small;
    }

    private void Update()
    {
        animator.SetInteger("Status", statusAnimator);
        switch (this.estado)
        {
            case ToadStatus.Small:
                spriteRenderer.color = Color.green;
                statusAnimator = 0;
                break;

            case ToadStatus.Mushroom:
                spriteRenderer.color = Color.blue;
                statusAnimator = 1;
                break;

            case ToadStatus.Flower:
                spriteRenderer.color = Color.red;
                statusAnimator = 2;
                break;
        }

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
        if (!invulnerable)
        {
            StartCoroutine(InvulnerabilityCooldown()); // Inicia la corrutina de invulnerabilidad

            animator.SetTrigger("StatusChange");

            switch (this.estado)
            {
                case ToadStatus.Small:
                    Debug.Log("Toad... ha muerto");
                    Death();
                    break;
                case ToadStatus.Mushroom:
                    estado = ToadStatus.Small;
                    break;
                case ToadStatus.Flower:
                    estado = ToadStatus.Mushroom;
                    break;
            }
        }
    }

    private IEnumerator InvulnerabilityCooldown()
    {
        invulnerable = true;  // Activa la invulnerabilidad

        float timePassed = 0f;
        if (estado != ToadStatus.Small)
        {
            while (timePassed < invulnerabilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled; // Alterna la visibilidad del SpriteRenderer
                timePassed += blinkRate; // Incrementa el tiempo transcurrido por el ritmo del parpadeo
                yield return new WaitForSeconds(blinkRate); // Espera un tiempo para el siguiente parpadeo
            }
        }

            spriteRenderer.enabled = true; // Asegúrate de que el sprite esté visible después de la invulnerabilidad
        invulnerable = false; // Desactiva la invulnerabilidad
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Vacio"))
        {
            DeathVoid();
        }
    }



    private void DeathVoid()
    {
        muerto = true;

        gameObject.layer = LayerMask.NameToLayer("ToadMuerto");

            // Eliminar todas las fuerzas y detener el movimiento
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Aplicar un pequeño salto (impulso hacia arriba)
        rb.velocity = new Vector2(0, 15f); // Puedes ajustar el valor 5f según necesites

        // Restaurar la gravedad para que caiga después del impulso
        rb.gravityScale = 3f; // Ajusta según la gravedad normal del juego

    }


    public void PowerUpSeta()
    {
        Debug.Log("Mario ha cogido la seta");
        this.estado = ToadStatus.Mushroom;
        animator.SetTrigger("StatusChange");
    }
    public void PowerUpFlor()
    {
        Debug.Log("Mario ha cogido la flor");
        this.estado = ToadStatus.Flower;
        animator.SetTrigger("StatusChange");
    }
    public void PowerUpOneUp()
    {
        Debug.Log("Mario ha cogido el OneUp");
    }
}


public enum ToadStatus
{
    Small,
    Mushroom,
    Flower
}