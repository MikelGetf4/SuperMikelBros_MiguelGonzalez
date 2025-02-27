using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaController : MonoBehaviour
{
    private Rigidbody2D rb;
    private MovimientoEnemigos movimiento;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private KoopaStatus status;

    public bool caparazon;
    public bool sleeping;
    private Coroutine sleepCoroutine;

    private Collider2D stompCollider;
    private Collider2D damageCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movimiento = GetComponent<MovimientoEnemigos>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Buscar los colliders en los hijos
        Transform stompTransform = transform.Find("KoopaStompCollider");
        Transform damageTransform = transform.Find("KoopaDamageCollider");

        if (stompTransform != null)
            stompCollider = stompTransform.GetComponent<Collider2D>();

        if (damageTransform != null)
            damageCollider = damageTransform.GetComponent<Collider2D>();
    }

    private void Update()
    {
        switch (this.status)
        {
            case KoopaStatus.Walk:
                spriteRenderer.color = Color.green;
                SetCollidersActive(true);
                break;

            case KoopaStatus.Sleep:
                spriteRenderer.color = Color.blue;
                SetCollidersActive(false);
                break;

            case KoopaStatus.Slide:
                spriteRenderer.color = Color.red;
                break;
        }
    }

    private void SetCollidersActive(bool isActive)
    {
        if (stompCollider != null)
            stompCollider.enabled = isActive;

        if (damageCollider != null)
            damageCollider.enabled = isActive;
    }

    private void FixedUpdate()
    {
        animator.SetBool("Caparazon", caparazon);
    }

    public void TakeDamage()
    {
        switch (this.status)
        {
            case KoopaStatus.Walk:
                this.sleepCoroutine = StartCoroutine(Sleep());
                break;

            case KoopaStatus.Sleep:
                this.LaunchKoopa();
                break;

            case KoopaStatus.Slide:
                this.StopKoopa();
                break;
        }
    }

    private IEnumerator Sleep()
    {
        if (this.status != KoopaStatus.Sleep)
        {
            yield return null;
        }

        this.status = KoopaStatus.Sleep;
        movimiento.Pausa();
        Debug.Log("Iniciando Sleep");

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        caparazon = true;
        sleeping = true;

        for (int i = 0; i < 5; i++) // 5 segundos de espera
        {
            Debug.Log("Han pasado " + i + " segundos");
            yield return new WaitForSeconds(1f); // Espera 1 segundo por iteración
        }

        Debug.Log("Finalizando Sleep");
        movimiento.Activar();
        movimiento.velocidad = 2f;

        caparazon = false;
        sleeping = false;
        sleepCoroutine = null;
        this.status = KoopaStatus.Walk;
        Debug.Log("Iniciando Walk");
    }

    public void LaunchKoopa()
    {
        if (this.status == KoopaStatus.Sleep)
        {
            sleeping = false;
            Debug.Log("Iniciando Slide");
            // Detener la corutina de sueño si está en curso.
            if (sleepCoroutine != null)
            {
                StopCoroutine(sleepCoroutine);
                sleepCoroutine = null;  // Asegúrate de limpiar la referencia de la corutina.
            }

            this.status = KoopaStatus.Slide;
            movimiento.velocidad = 8f;
            movimiento.Activar();

            // Llamar a la corutina para activar los colliders después de 1 segundo
            StartCoroutine(EnableCollidersAfterDelay(0.5f));
        }
    }

    private IEnumerator EnableCollidersAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetCollidersActive(true);
    }

    private void StopKoopa()
    {
        Debug.Log("Parando Slide");
        movimiento.Pausa();

        this.status = KoopaStatus.Sleep;
        this.sleepCoroutine = StartCoroutine(Sleep());
        sleeping = true;

        StartCoroutine(EnableCollidersAfterDelay(0.5f));
    }

    // Método que detecta si Mario salta encima del Koopa
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))  // Asegúrate de que Mario tenga el tag "Player"
        {
            // Verificar si el Koopa está en estado 'Slide' y si Mario lo golpeó desde arriba
            if (this.status == KoopaStatus.Slide && collision.contacts[0].normal.y > 0.5f)
            {
                // Si Mario lo golpea desde arriba, cambiar a 'Sleep'
                StopKoopa();
            }
        }
    }
}

public enum KoopaStatus
{
    Walk,
    Sleep,
    Slide
}
