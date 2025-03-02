using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCollider : MonoBehaviour, IPowerUp
{
    private GameObject PowerUpPadre;
    public PowerUpCorresponde EsPowerUp;

    private void Awake() //Al encontrarse este PowerUp dentro de un GameObject, aqui busca el GameObject padre
    {
        PowerUpPadre = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision) //Al chocar con el jugador, activa la funcion de la interfaz
    {
        var JugadorChocado = collision.GetComponent<ToadController>();
        if (JugadorChocado != null)
        {
            Activate(collision.gameObject);
        }
    }

    private void Start() //Se busca el Tag del padre para determinar que PowerUp es
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


    public void Activate(GameObject objetoCoisionado) //Dependiendo del PowerUp que sea, hara unas cosas u otras
    {
        var JugadorChocado = objetoCoisionado.GetComponent<ToadController>();
        if (JugadorChocado != null) 
        {
            var toadController = JugadorChocado.GetComponent<ToadController>();
            if (toadController != null)
            {
                Destroy(gameObject);
                Destroy(PowerUpPadre); //Destruye el powerUp nada mas es recogido
                if (EsPowerUp == PowerUpCorresponde.Seta)  //Si el PowerUp es una seta, activa en Toad la funcion de seta
                {
                    toadController.PowerUpSeta();
                    GameManager.Instance.AgregarPuntos(1000); 
                }

                if (EsPowerUp == PowerUpCorresponde.Flor) //Si el PowerUp es una flor, activa en Toad la funcion de flor
                {
                    toadController.PowerUpFlor();
                    GameManager.Instance.AgregarPuntos(1000);
                }

                if (EsPowerUp == PowerUpCorresponde.OneUp) //Si el PowerUp es un OneUp, activa en el GameManager la funcion de OneUp
                {
                    toadController.PowerUpOneUp();
                    GameManager.Instance.AgregarPuntos(1000);
                    GameManager.Instance.AgregarVida();
                }

            }
            
        }
    }
}

public enum PowerUpCorresponde
{
    Seta,
    Flor,
    OneUp
}