using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } //Instancia el Game Manager

    public int vidas = 3; //Vidas del jugador
    public int puntaje = 0; //Puntos del jugador
    public int monedas = 0; //Monedas del jugador
    public int nivelActual = 1; //Nivel en el que nos encontramos
    public float tiempoMaximo = 300f; // Tiempo del jugador
    private float tiempoRestante; //Tiempo restante

    private int tamaño = 0; //Tamño del Toad

    private ToadController toadController; //Script del Toad

    private void Awake()    // Implementación del Singleton
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        tiempoRestante = tiempoMaximo; //Deja el tiempo en 300 segundos
        StartCoroutine(ContadorTiempo()); //Comienza la corrutina para que cuente el tiempo 
        SceneManager.sceneLoaded += OnSceneLoaded; // Evento cuando se carga una nueva escena

        GameObject toadObject = GameObject.Find("Toad"); //Busca a Toad y saca su script controller
        if (toadObject != null)
        {
            toadController = toadObject.GetComponent<ToadController>();
            Debug.Log("Toad se encontrado en la nueva escena.");
        }
        else
        {
            Debug.Log("Toad no encontrado en la nueva escena.");
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        GameObject toadObject = GameObject.Find("Toad"); // Busca a Toad en la escena nueva y saca otra vez su scipt Controller
        if (toadObject != null)
        {
            toadController = toadObject.GetComponent<ToadController>();
            Debug.Log("Toad encontrado en la nueva escena.");
        }
        else
        {
            Debug.Log("Toad no encontrado en la nueva escena.");
        }
    }


    public void AgregarPuntos(int cantidad) //Añade puntos al total de puntos
    {
        puntaje += cantidad;
    }

    public void AgregarMoneda() //Añade una moneda al total
    {
        monedas++;
    }

    public void AgregarVida() //Añade una vida al total
    {
        vidas++;
    }

    public void PerderVida() //Resta una vida al total y te manda al GameOver si tienes 0 vidas
    {
        vidas--;

        if (vidas <= 0)
        {
            GameOver();
        }
    }


    private void GameOver() //Carga la escena GameOver
    {
        SceneManager.LoadScene("GameOverScene");
    }

    private IEnumerator ContadorTiempo() //Cuenta hacia atras el tiempo. Si el tiempo llega a 0, mata a Toad
    {
        while (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        toadController.Death();
    }

    public float GetTiempoRestante() //Adquiere el tiempo que queda
    {
        return Mathf.Max(0, tiempoRestante);
    }

    public void GuardarTamaño() //Guarda el tamaño de Toad en la escena
    {
        toadController.statusAnimator = tamaño;
    }


}