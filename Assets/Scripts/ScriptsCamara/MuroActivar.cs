using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuroActivar : MonoBehaviour
{
        public Transform cameraTransform; //El transform de la camara
        public float offset = -13.7f; //Distanca del centro de la camara a la que permancera constantemente
        void Update() //Constantemente se mueve junto con la camara
        {
            transform.position = new Vector3(cameraTransform.position.x - offset, transform.position.y, transform.position.z);
        }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto con el que colisionamos tiene la interfaz IEnemigos para saber sin son Koopas o Goombas
        IEnemigos enemigo = collision.GetComponent<IEnemigos>();

        if (enemigo != null)
        {
            // Si el enemigo tiene la interfaz activa el metodo Activar()
            enemigo.Activar();
        }
    }
}
