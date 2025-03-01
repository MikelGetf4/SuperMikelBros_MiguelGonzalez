using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollider : MonoBehaviour, IPowerUp
{
    private GameObject PowerUpPadre;
    public PowerUpCorresponde EsPowerUp;

    private void Awake()
    {
        PowerUpPadre = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var JugadorChocado = collision.GetComponent<ToadController>();
        if (JugadorChocado != null)
        {
            Activate(collision.gameObject);
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


    public void Activate(GameObject objetoCoisionado)
    {
        var JugadorChocado = objetoCoisionado.GetComponent<ToadController>();
        if (JugadorChocado != null) // Evitar NullReferenceException
        {
            var toadController = JugadorChocado.GetComponent<ToadController>();
            if (toadController != null)
            {
                Destroy(gameObject);
                Destroy(PowerUpPadre);
                if (EsPowerUp == PowerUpCorresponde.Seta)
                {
                    toadController.PowerUpSeta();
                    GameManager.Instance.AgregarPuntos(1000);
                }

                if (EsPowerUp == PowerUpCorresponde.Flor)
                {
                    toadController.PowerUpFlor();
                    GameManager.Instance.AgregarPuntos(1000);
                }

                if (EsPowerUp == PowerUpCorresponde.OneUp)
                {
                    toadController.PowerUpOneUp();
                    GameManager.Instance.AgregarPuntos(1000);
                    GameManager.Instance.AgregarVida();
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