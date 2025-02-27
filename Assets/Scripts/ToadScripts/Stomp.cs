using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stomp : MonoBehaviour
{
    private Rigidbody2D rbToad;
    private ToadController toadController;

    [SerializeField] private float fuerzaSalto = 5f;

    private void Awake()
    {
        rbToad = GetComponentInParent<Rigidbody2D>();

        if (rbToad == null)
        {
            Debug.Log("No encuentra el rb");
        }

        toadController = GetComponentInParent<ToadController>();
    }

    private void FixedUpdate()
    {
        if (toadController.muerto == true)
        {
            gameObject.layer = LayerMask.NameToLayer("ToadMuerto");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto con el que colisionamos tiene la interfaz IEnemigos
        IEnemigos enemigo = collision.GetComponent<IEnemigos>();

        if (enemigo != null)
        {
            // Si el enemigo tiene la interfaz, activar su método RecibirDanio
            enemigo.RecibirDanio();

            if (rbToad != null)
            {
                // Aplicar un impulso controlado hacia arriba
                // Establecer directamente la velocidad vertical del Rigidbody2D
                rbToad.velocity = new Vector2(rbToad.velocity.x, 0);  // Reseteamos cualquier velocidad vertical previa
                rbToad.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            }
        }

        
    }
}
