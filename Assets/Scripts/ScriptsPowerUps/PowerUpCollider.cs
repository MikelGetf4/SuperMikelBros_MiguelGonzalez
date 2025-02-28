using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollider : MonoBehaviour, IPowerUp
{
    [SerializeField]
    GameObject Toad;
    private GameObject PowerUpPadre;
    public PowerUpCorresponde EsPowerUp;

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

    private void Start()
    {
        if (PowerUpPadre.CompareTag("Seta"))
        {
            EsPowerUp = PowerUpCorresponde.Seta;
        }
        if (PowerUpPadre.CompareTag("Flor"))
        {
            EsPowerUp = PowerUpCorresponde.Flor;
        }
        if (PowerUpPadre.CompareTag("OneUp"))
        {
            EsPowerUp = PowerUpCorresponde.OneUp;
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
                if (EsPowerUp == PowerUpCorresponde.Seta)
                {
                    toadController.PowerUpSeta();
                }

                if (EsPowerUp == PowerUpCorresponde.Flor)
                {
                    toadController.PowerUpFlor();
                }

                if (EsPowerUp == PowerUpCorresponde.OneUp)
                {
                    toadController.PowerUpOneUp();
                }

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

public enum PowerUpCorresponde
{
    Seta,
    Flor,
    OneUp
}