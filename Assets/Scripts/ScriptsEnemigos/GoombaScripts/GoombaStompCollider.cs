using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaStompCollider : MonoBehaviour, IEnemigos
{
    private GameObject goombaPadre;
    private Rigidbody2D rbGoomba;
    private Collider2D colliderGoomba;
    private SpriteRenderer spriteRenderer;
    private MovimientoEnemigos movimiento;
    private Animator animator;

    public bool muerto = false;

    [SerializeField] private float tiempoMuerte = 0.3f; // Tiempo antes de destruir al Goomba

    private void Awake()
    {
        goombaPadre = transform.parent.gameObject; // Referencia al padre (Goomba)
        rbGoomba = goombaPadre.GetComponent<Rigidbody2D>();
        colliderGoomba = goombaPadre.GetComponent<Collider2D>();
        spriteRenderer = goombaPadre.GetComponent<SpriteRenderer>();
        movimiento = goombaPadre.GetComponent<MovimientoEnemigos>();
        animator = goombaPadre.GetComponent<Animator>();
    }

    // Método de la interfaz IEnemigos
    public void RecibirDanio()
    {
        Debug.Log("Muelto");
        StartCoroutine(Morir());
    }

    private IEnumerator Morir()
    {
        // Detener el movimiento del Goomba
        movimiento.Pausa();
        colliderGoomba.enabled = false; // Desactivar la colisión
        rbGoomba.bodyType = RigidbodyType2D.Static;

        // Aplastar visualmente al Goomba
        muerto = true;
        animator.SetBool("Muerto", muerto);

        yield return new WaitForSeconds(tiempoMuerte);

        Destroy(goombaPadre); // Destruir el Goomba completo
    }


    public void Activar()
    {

    }
}
