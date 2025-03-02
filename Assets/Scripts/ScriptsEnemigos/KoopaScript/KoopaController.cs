using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaController : MonoBehaviour
{
    private Rigidbody2D rb; //Rigibody del Koopa
    private MovimientoEnemigos movimiento; //Script de movimiento
    private Animator animator; //Animator
    public KoopaStatus status; //Estado del Koopa

    public bool caparazon; //Booleano para decir que esta en el caparazon
    public bool sleeping; //Booleano para decir que esta durmiendo
    private Coroutine sleepCoroutine; //Corrutina de dormir

    private Collider2D stompCollider; //Collider hijo para que Toad lo pise
    private Collider2D damageCollider; //Collider hijo para hacer daño a Toad

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //Recoge el rigibody
        animator = GetComponent<Animator>(); //Recoge el animator
        movimiento = GetComponent<MovimientoEnemigos>(); //Recoge el Script MovimientoEnemigos

        // Buscar los transforms colliders en los hijos
        Transform stompTransform = transform.Find("KoopaStompCollider");
        Transform damageTransform = transform.Find("KoopaDamageCollider");

        //Coge los colliders de daño y atacar
        stompCollider = stompTransform.GetComponent<Collider2D>();
        damageCollider = damageTransform.GetComponent<Collider2D>();
    }

    private void Update()
    {
        // Dependiendo del estado de Koopa, activar o desactivar los colliders
        switch (this.status)
        {
            case KoopaStatus.Walk:
                SetCollidersActive(true); //Si Koopa esta normal, activa los colliders normalmente
                break;

            case KoopaStatus.Sleep:
                SetCollidersActive(false); //Si Koopa esta durmiendo, desactiva los colliders
                break;

            case KoopaStatus.Slide:
                break;
        }
    }

    private void SetCollidersActive(bool isActive)
    {
        // Cambia el estado de activación de los colliders
            stompCollider.enabled = isActive;
            damageCollider.enabled = isActive;
    }

    private void FixedUpdate()
    {
        //Le dice al animator cuando tiene que ponerlo en modo caparzon
        animator.SetBool("Caparazon", caparazon);
    }

    public void TakeDamage() //Cuando recibe daño, su funcion varia dependiendo de su estado
    {
        GameManager.Instance.AgregarPuntos(200); // Añade puntos al jugador cuando Koopa recibe daño
        switch (this.status)
        {
            case KoopaStatus.Walk:
                this.sleepCoroutine = StartCoroutine(Sleep()); // Si está caminando, inicia la corutina de dormir
                break;

            case KoopaStatus.Sleep:
                this.LaunchKoopa(); // Si ya está dormido, hacer que se deslice
                break;

            case KoopaStatus.Slide: //Si se esta deslizando, lo detiene
                this.StopKoopa();
                break;
        }
    }

    private IEnumerator Sleep()
    {
        if (this.status != KoopaStatus.Sleep) // Si Koopa no está durmiendo, sale de la coroutine
        {
            yield return null;
        }

        this.status = KoopaStatus.Sleep; //Marca a Koopa como dormido
        movimiento.Pausa(); //Pausa su movimiento

        //Detiene todas las fuerzas de la fisica
        rb.velocity = Vector2.zero; 
        rb.angularVelocity = 0f;

        caparazon = true; //Coloca a Koopa en el caparazon (Para el animator)
        sleeping = true; //Lo marca como durmiendo

        for (int i = 0; i < 5; i++) // 5 segundos de espera antes de que el Koopa se despierte
        {
            yield return new WaitForSeconds(1f);
        }

        movimiento.Activar(); //Vuelve a activar su movimiento
        movimiento.velocidad = 2f;

        caparazon = false; //Marca que no esta en su caparazon
        sleeping = false; //Marca que no esta durmiendo
        sleepCoroutine = null; //Limpiaa la referencia de la coroutine
        this.status = KoopaStatus.Walk; // Cambiar estado a caminar
    }

    public void LaunchKoopa()
    {
        if (this.status == KoopaStatus.Sleep) // Si Koopa esta dormido comienza a deslizarse
        {
            sleeping = false;
            // Detiene la corutina de sueño si está en curso
            if (sleepCoroutine != null)
            {
                StopCoroutine(sleepCoroutine);
                sleepCoroutine = null;  //Limpiar la referencia de la corutina
            }

            this.status = KoopaStatus.Slide; // Cambiar estado a Slide
            movimiento.velocidad = 8f; //Le sube ,ucho su velocidad
            movimiento.Activar(); //Activa el movimiento

            //Activar los colliders despues de 0.5 segundos
            StartCoroutine(EnableCollidersAfterDelay(0.5f));
        }
    }

    private IEnumerator EnableCollidersAfterDelay(float delay)
    {
        // Espera un tiempo especificado y luego activa los colliders
        yield return new WaitForSeconds(delay);
        SetCollidersActive(true);
    }

    private void StopKoopa() // Detener el movimiento del Koopa y lo vuelve a dormir
    {
        movimiento.Pausa();

        this.status = KoopaStatus.Sleep;
        this.sleepCoroutine = StartCoroutine(Sleep());
        sleeping = true;

        StartCoroutine(EnableCollidersAfterDelay(0.5f));
    }

    private void OnCollisionEnter2D(Collision2D collision) //Detecta si Toad lo golpeo y para el caparazon
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (this.status == KoopaStatus.Slide)
            {
                StopKoopa();
            }
        }
    }
}

public enum KoopaStatus //Estado del Koopa
{
    Walk,
    Sleep,
    Slide
}
