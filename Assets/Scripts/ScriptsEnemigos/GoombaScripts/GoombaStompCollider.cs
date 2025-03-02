using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaStompCollider : MonoBehaviour, IEnemigos
{
    private GameObject goombaPadre; //Como esto se encuentra en un GameObject hijo de Goomba, aqui se declara el padre
    private Rigidbody2D rbGoomba; //Rigibody del Goomba padre
    private MovimientoEnemigos movimiento; //Script de MovimientoEnemigos del padre Goomba
    private Animator animator; //Animator del padre Goomba

    [SerializeField]
    private GameObject goombaDeathCollider; //Collider que le hace daño a Toad

    private Collider2D colliderStomp; // Collider que permite a Toad matar al Goomba

    public bool muerto = false; // Bool para indicar que ha mierdo

    [SerializeField] private float tiempoMuerte = 0.3f; // Tiempo antes de destruir al Goomba

    private void Awake()
    {
        goombaPadre = transform.parent.gameObject; //Encuentra al padre
        rbGoomba = goombaPadre.GetComponent<Rigidbody2D>();
        movimiento = goombaPadre.GetComponent<MovimientoEnemigos>();
        animator = goombaPadre.GetComponent<Animator>();

        colliderStomp = GetComponent<Collider2D>();
    }

    public void RecibirDanio() //METODO DE IENEMIGOS. Agrega puntos y crea la corrutina de muerte
    {
        GameManager.Instance.AgregarPuntos(200);
        StartCoroutine(Morir());
    }

    private IEnumerator Morir()
    {
        movimiento.Pausa(); //Detiene el movimiento del Goomba
        colliderStomp.enabled = false; // Desactiva la colisión
        rbGoomba.bodyType = RigidbodyType2D.Static; //Hace el Rigibody static para asegurar que no se mueva por fuerzas externas
        goombaDeathCollider.SetActive(false); //Apaga su collider capaz de matar a Toad

        muerto = true; //Marca el booleano de muerto como true
        animator.SetBool("Muerto", muerto); //Manda el mensaje de Muerto al animator

        yield return new WaitForSeconds(tiempoMuerte);

        Destroy(goombaPadre); // Destruir el Goomba completo
    }


    public void Activar() //Ignorar
    {

    }
}
