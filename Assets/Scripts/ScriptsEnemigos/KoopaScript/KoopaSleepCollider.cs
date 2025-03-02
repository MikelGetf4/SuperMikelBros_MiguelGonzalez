using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaSleepCollider : MonoBehaviour
{
    private KoopaController koopaController;
    [SerializeField]
    private Collider2D sleepCollider; 

    private void Awake() //Obtiene elementos del Koopa padre
    {
        koopaController = GetComponentInParent<KoopaController>();
        sleepCollider = GetComponentInParent<Collider2D>();
    }

    private void Start() //Cuando comienza el juego, para el collider de dormir
    {
        sleepCollider.enabled = false;
    }

    private void Update() // Si Koopa esta en estado de sueño y el sleepCollider no esta activado, activamos el collider despues de un pequeño retraso
    {
        if (koopaController.sleeping != sleepCollider.enabled)
        {
            StartCoroutine(ActivarCollider(0.5f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) // Si Toad pisa el collider de sleep el Koopa se inicia la funcion de LaunchKoopa()
    {
        if (collision.CompareTag("Player"))
        {
            koopaController.LaunchKoopa();
        }
    }

    private IEnumerator ActivarCollider(float delay)
    {
        // Espera 0.5 segundos antes de activar el collider
        yield return new WaitForSeconds(delay);
        sleepCollider.enabled = true;
    }

}
