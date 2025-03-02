using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToadController : MonoBehaviour
{
    public float direction; //Direccion del Input Horizontal
    private Rigidbody2D rb; // Rigibody del Toad

    [SerializeField]    private float fuerzaAceleracion = 12f; //Fuerda de aceleracion con la que comenzara Toad
    [SerializeField]    private float fuerzaDeceleracion = 20f; //Fuerza con la que desacelerará Toad
    [SerializeField]    private float maximaVelocidad = 5f; // Maxima velocidad a la que Toad será capaz de llegar
    [SerializeField]    private float fuerzaSalto = 11f; //Fuerza que se le aplica a Toad para dar el salto
    [SerializeField]    private float anchoDePersonaje; //Ancho del personaje para calcular los raycast
    [SerializeField]    private float distanciaRaycast; //Distancia desde los pies de Toad hasta colisionar con el suelo
    [SerializeField]    private LayerMask capaSuelo; //Capa suelo
    [SerializeField]    private float fuerzaSaltoFinal; // La maxima fuerza que llegará a tener Toad en un salto al maximo
    [SerializeField]    private float tiempoSaltoMaximo = 1f; // Tiempo maximo de salto sostenido
    [SerializeField]    private float multiplicadorGravedad = 4f; // Gravedad extra al soltar el salto
    [SerializeField]    private float multiplicadorGravedadCaida = 6f; // Gravedad aumentada al caer

    public bool isRunning; //Si Toad esta corriendo
    public bool estaSaltando; //Si Toad esta saltando
    private float tiempoSaltoActual; //Tiempo que lleva el jugador pulsando el boton de salto


    public bool estaEnSuelo; //Si Toad esta tocando el suelo
    public bool agachado; //Si Toad esta agachado
    public bool muerto = false; //Si Toad ha muerto
    private bool yaMurio = false; //Si Toad ya se ha muerto (evita conflictos con el animator)

    public ToadStatus estado; //Estado del Toad, puede ser pequeño, grande o flor de fuego
    private SpriteRenderer spriteRenderer; //El SpriteRenderer del Toad
    private Animator animator; //El Animator del Toad
    public int statusAnimator = 0; //El estado de Toad. Sirve para mandarlo al animator

    private bool invulnerable = false; //Si Toad se encuentra Invulnerable por haber recibido un golpe
    private float invulnerabilityDuration = 2f; //Duracion de la invencibilidad tras recibir un golpe
    private float blinkRate = 0.1f; //Velocidad a la que Toad parpadea al recibir un golpe

    private void Awake() //Aqui se obtienen elementos importantes de Toad
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        animator = rb.GetComponent<Animator>();
    }

    private void Start() //Aqui se aseguran que algunas variables comiencen de cierta forma
    {
        muerto = false;
        this.estado = ToadStatus.Small;
        yaMurio = false;
    }

    private void Update()
    {
        animator.SetInteger("Status", statusAnimator); //Para poder mandarle el estado al animator 


        switch (this.estado) //Para que el animator conozca el estado de Toad
        {
            case ToadStatus.Small:
                statusAnimator = 0;
                break;

            case ToadStatus.Mushroom:
                statusAnimator = 1;
                break;

            case ToadStatus.Flower:
                statusAnimator = 2;
                break;
        }

        this.direction = Input.GetAxisRaw("Horizontal"); //Guarda el Input Horizontal


        if (!estaEnSuelo)
        {
            if (Input.GetKey(KeyCode.LeftShift) && direction != 0) // Si esta en el aire, no desactiva isRunning, solo mantenerlo activo si se está moviendo
            {
                this.isRunning = true;
            }
        }
        else // Si esta en el suelo, se comprueba si esta presionando el Shift para correr
        {
            
            this.isRunning = Input.GetKey(KeyCode.LeftShift);
        }

        ComprobarSuelo(); //Comprueba que Toad esta tocando el suelo


        if (muerto == false)
        {
            Saltar(); //Permite saltar

            if (estaEnSuelo == true)
            {
                Agachado(); //Permite agacharse solo si estas tocando el suelo
            }
        }
    }

    private void FixedUpdate()
    {
        if (this.agachado == false && this.muerto == false)
        {
            Movimiento(); //Si no se esta ni agachado ni muerto, te permite moverte
        }
        else
        {
            //Hace que el personaje pierda velocidad poco a poco
            float nuevaVelocidadX = Mathf.Lerp(rb.velocity.x, 0, Time.fixedDeltaTime * 5f);
            rb.velocity = new Vector2(nuevaVelocidadX, rb.velocity.y);
        }
    }

    private void Movimiento()
    {
        float fuerzaAceleracionFinal = this.fuerzaAceleracion;
        float fuerzaDeceleracionFinal = this.fuerzaDeceleracion;
        float maximaVelocidadFinal = this.maximaVelocidad;

        if (this.isRunning) //Aumenta la velocidad si se esta corriendo
        {
            fuerzaAceleracionFinal = this.fuerzaAceleracion * 2;
            maximaVelocidadFinal = this.maximaVelocidad * 2;
        }

        if (Mathf.Abs(this.rb.velocity.y) > 0.1f) //Hace mas complicado cambiar la direccion una vez se esta en el aire
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

    private void ComprobarSuelo() //Mediante Raycast comprueba constantemete que ambos extremos de Toad se encuentren en el suelo
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
        // Si el jugador presiona el boton de salto y esta en el suelo se le aplica una fuerza de salto
        if (Input.GetButtonDown("Jump") && estaEnSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            estaSaltando = true;
            tiempoSaltoActual = 0;
        }

        // Si se mantiene el boton de salto se siguen aplicando pequeñas fuerzas constantes para prolongar el salto hasta llegar al tiempo limite
        if (Input.GetButton("Jump") && estaSaltando && tiempoSaltoActual < tiempoSaltoMaximo)
        {
            rb.velocity += new Vector2(0, 0.2f);
            tiempoSaltoActual += Time.deltaTime;
        }

        // Si se suelta el botón de salto y aun está subiendo le reduce la velocidad de subida
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            estaSaltando = false;
        }

        // Si Toad esta cayendo aumenta la gravedad para acelerar la caida (para parecerse mas al Mario Bros original)
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.down * multiplicadorGravedadCaida * Time.deltaTime;
        }

        // Si no esta saltando aplica gravedad extra para hacerlo caer mas rapido (para parecerse mas al Mario Bros original)
        else if (!estaSaltando)
        {
            rb.velocity += Vector2.down * multiplicadorGravedad * Time.deltaTime;
        }
    }

    private void OnDrawGizmos() //Para ver los raycast que colisionan con el suelo en modo Scene
    {
        Gizmos.color = Color.green;
        // Raycast izquierdo
        Gizmos.DrawRay(transform.position + Vector3.left * anchoDePersonaje, Vector2.down * distanciaRaycast);

        // Raycast derecho
        Gizmos.DrawRay(transform.position + Vector3.right * anchoDePersonaje, Vector2.down * distanciaRaycast);
    }

    private void Agachado() //Comprueba que el personaje este agachado si se pulsa la tecla "Abajo"
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

    private void OnTriggerEnter2D(Collider2D collision) //Si se entra en contacto con un enemigo con el Tag Muerte, Toad recibe daño
    {
        if (collision.gameObject.CompareTag("Muerte"))
        {
            Damagable();
        }
    }

    public void Damagable() //Si Toad no es invulnerable, comienza la corrutina para hacerlo invulnerable y cambia de estado dependiendo de su estado o muere
    {
        if (!invulnerable)
        {
            StartCoroutine(InvulnerabilityCooldown()); // Inicia la corrutina de invulnerabilidad

            animator.SetTrigger("StatusChange"); //Manda al animator el aviso de que Toad ha cambiado para que este cambie su animacion

            switch (this.estado)
            {
                case ToadStatus.Small: //Si Toad es pequeño, muere
                    Debug.Log("Toad ha muerto");
                    Death();
                    break;
                case ToadStatus.Mushroom: //Si Toad es grande, pasa a ser pequeño
                    estado = ToadStatus.Small;
                    break;
                case ToadStatus.Flower: //Si Toad esta en modo flor de fuego, pasa a ser simplemente grande
                    estado = ToadStatus.Mushroom;
                    break;
            }
        }
    }

    private IEnumerator InvulnerabilityCooldown() // Activa la invulnerabilidad tras recibir un golpe
    {
        invulnerable = true;  

        float timePassed = 0f;
        if (estado != ToadStatus.Small) //Si Toad es pequeño (y por tanto ha muerto) y se activa la anmimacion invencibilidad, causara una animacion extraña
        {
            while (timePassed < invulnerabilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled; // Cambia la visibilidad con el SpriteRenderer
                timePassed += blinkRate; // Sube el tiempo transcurrido por el ritmo del parpadeo
                yield return new WaitForSeconds(blinkRate); // Espera un tiempo para el siguiente parpadeo
            }
        }

            spriteRenderer.enabled = true; // Asegura que el sprite sea visible despues de la invulnerabilidad
            invulnerable = false; // Desactiva la invulnerabilidad
    }

    public void Death() //Animacion de muerte
    {
        if (yaMurio) //Sin esto, hay veces por colliders que Toad pierde varias vidas antes de cambiar de capa
        {
            return;
        }
        yaMurio = true;
        muerto = true;

        gameObject.layer = LayerMask.NameToLayer("ToadMuerto"); //Cambia de capa a Toad para caiga fuera del escenario

        // Realiza una pequeña animacion en la que Toad da un salto y cae al vacio
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.velocity = new Vector2(0, 5f);
        rb.gravityScale = 2f;

        StartCoroutine(EsperarYRecargar(2f)); //Reinicia el nivel tras acabar la animación

        GameManager.Instance.PerderVida(); //Le dice al GameManager que se ha perdido una vida

    }

    private void OnTriggerExit2D(Collider2D collision)  //Al SALIR del ColliderTrigger que hay al caer al vacio, Toad morirá
    {                                                   //He decidido que sea un Trigger porque me gustaba mas para la animacion de muerte
        if (collision.CompareTag("Vacio")) 
        {
            DeathVoid();
            Destroy(collision.gameObject); //Destruye el Vacio para que, en caso de recargarse la escena, Toad no se quede realizando la animacion de Muerte para siempre
        }
    }



    private void DeathVoid() //Parecido a Death(), pero en el vacio. Lo unico que cambia son las fuerzas que se le aplican a la animacion
    {
        if (yaMurio) //Sin esto, hay veces por colliders que Toad pierde varias vidas antes de cambiar de capa
        {
            return ;
        }
        yaMurio = true;

        muerto = true;

        gameObject.layer = LayerMask.NameToLayer("ToadMuerto"); //Cambia de capa a Toad para caiga fuera del escenario

        // Realiza una pequeña animacion en la que Toad da un salto y cae al vacio. Mas fuerte que la de la Death()
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.velocity = new Vector2(0, 15f);
        rb.gravityScale = 3f;

        StartCoroutine(EsperarYRecargar(1.5f)); //Reinicia el nivel tras acabar la animación (Menos duracion que en Detah())

        GameManager.Instance.PerderVida(); //Pierde una vida
    }

    private IEnumerator EsperarYRecargar(float tiempo) //Reinicia la escena tras cierto tiempo
    {
        yield return new WaitForSeconds(tiempo);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void PowerUpSeta() //Cambia a Mario a Grande
    {
        Debug.Log("Toad ha cogido la seta");
        this.estado = ToadStatus.Mushroom;
        animator.SetTrigger("StatusChange");
    }
    public void PowerUpFlor() //Cambia a Toad a Flor de fuego
    {
        Debug.Log("Toad ha cogido la flor");
        this.estado = ToadStatus.Flower;
        animator.SetTrigger("StatusChange");
    }
    public void PowerUpOneUp() //Avisa de que Toad ha cogido un OneUp. Esta funcion es prescindible
    {
        Debug.Log("Toad ha cogido el OneUp");
    }
}


public enum ToadStatus //Estado del Toad
{
    Small,
    Mushroom,
    Flower
}