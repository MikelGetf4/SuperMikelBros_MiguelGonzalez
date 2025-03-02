using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BanderaManager : MonoBehaviour
{
    private ToadController toadController;

    private void Start() //Encuentra tanto a Toad como a su script
    {
        GameObject toadObject = GameObject.Find("Toad");
        toadController = toadObject.GetComponent<ToadController>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ToadController toad = collision.GetComponent<ToadController>(); // Verifica si el objeto que entra es Toad

        if (toad != null) 
        {
            GameManager.Instance.GuardarTamaño(); //Almacena su estado
            GameManager.Instance.AgregarPuntos(1000); //Añade puntos
            StartCoroutine(Ganar(toadController, 4f)); //Comienza la Corrutina
        }
    }

    private IEnumerator Ganar(ToadController toadController, float tiempo) //Esta corrutina obliga a Toad a andar hacia el castillo, haciendo como si el jugador pulsara hacia la derecha
    {

        float tiempoRestante = tiempo;
        while (tiempoRestante > 0)
        {
            toadController.isRunning = false; //No permite correr
            toadController.direction = 1; //hace como si se pulsara a la derecha
            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        // Espera un poco antes de cargar el siguiente nivel
        yield return new WaitForSeconds(0.5f);

        // Cambiar el nivel
        SceneManager.LoadScene("Level 1-2");
    }
}
