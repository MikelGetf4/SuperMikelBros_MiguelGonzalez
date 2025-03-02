using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaGoombaCollider : MonoBehaviour
{

    private KoopaController controller;

    private void Awake()
    {
        controller = GetComponentInParent<KoopaController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el Koopa esta en el estado 'Slide'
        if (controller.status == KoopaStatus.Slide)
        {
            // Verifica si el objeto con el que choca tiene la interfaz IEnemigos y por tanto es un goomba
            IEnemigos enemigo = collision.GetComponent<IEnemigos>();

            if (enemigo != null)
            {
                //Si es un goomba lo destruye y da puntos
                Destroy(collision.gameObject);
                GameManager.Instance.AgregarPuntos(300);
            }
        }
    }
}


