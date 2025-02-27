using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollider : MonoBehaviour, IPowerUp
{
    [SerializeField]
    GameObject Toad;
    private GameObject PowerUpPadre;

    private void Awake()
    {
        PowerUpPadre = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Activate();
        }
    }
    

    public void Activate()
    {
        if (Toad != null) // Evitar NullReferenceException
        {
            var toadController = Toad.GetComponent<ToadController>();
            if (toadController != null)
            {
                Destroy(gameObject);
                Destroy(PowerUpPadre);
                toadController.PowerUp();
                
            }
            else
            {
                Debug.LogWarning("ToadController no encontrado en Toad.");
            }
        }
        else
        {
            Debug.LogWarning("Toad no está asignado en PowerUpCollider.");
        }
    }
}
