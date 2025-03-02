using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaStompCollider : MonoBehaviour, IEnemigos
{
    private KoopaController koopaController;

    private void Awake()
    {
        koopaController = GetComponentInParent<KoopaController>();
    }
    public void RecibirDanio() //Al recibir daño desde la Interfaz IEnemigos, activa la funcion de KoopaColllider TakeDamage()
    {
        if (koopaController != null)
        {
            koopaController.TakeDamage();
        }
    }

    public void Activar()
    {
        return;
    }
}
