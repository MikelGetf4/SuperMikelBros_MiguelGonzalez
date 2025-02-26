using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaInter : MonoBehaviour, IEnemigos
{
    // Implementación del método RecibirDanio()
    public void RecibirDanio()
    {
        Debug.Log("El enemigo ha recibido daño.");
    }

    // Implementación del método Pausa()
    public void Pausa()
    {
        Debug.Log("El enemigo está pausado.");
    }

    // Implementación del método Activar()
    public void Activar()
    {
        Debug.Log("El enemigo está activado.");
    }
}
