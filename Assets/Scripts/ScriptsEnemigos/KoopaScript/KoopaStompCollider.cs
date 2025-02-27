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
    public void RecibirDanio()
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
