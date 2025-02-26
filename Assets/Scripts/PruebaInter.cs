using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaInter : MonoBehaviour, IEnemigos
{
    // Implementaci�n del m�todo RecibirDanio()
    public void RecibirDanio()
    {
        Debug.Log("El enemigo ha recibido da�o.");
    }

    // Implementaci�n del m�todo Pausa()
    public void Pausa()
    {
        Debug.Log("El enemigo est� pausado.");
    }

    // Implementaci�n del m�todo Activar()
    public void Activar()
    {
        Debug.Log("El enemigo est� activado.");
    }
}
