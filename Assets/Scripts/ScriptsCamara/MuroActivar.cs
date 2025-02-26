using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuroActivar : MonoBehaviour
{
        public Transform cameraTransform;
        public float offset = -13.7f;
        void Update()
        {
            transform.position = new Vector3(cameraTransform.position.x - offset, transform.position.y, transform.position.z);
        }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto con el que colisionamos tiene la interfaz IEnemigos
        IEnemigos enemigo = collision.GetComponent<IEnemigos>();

        if (enemigo != null)
        {
            // Si el enemigo tiene la interfaz, activar su método Activar
            enemigo.Activar();
        }
    }
}
