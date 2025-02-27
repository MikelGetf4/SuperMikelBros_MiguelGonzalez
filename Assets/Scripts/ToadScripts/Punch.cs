using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto con el que colisionamos tiene la interfaz IEnemigos
        IBloque bloque = collision.GetComponent<IBloque>();

        if (bloque != null)
        {
            // Si el enemigo tiene la interfaz, activar su método RecibirDanio
            bloque.Hit();

        }
    }
}
