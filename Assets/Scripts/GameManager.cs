using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Stats")]
    public int vidas = 3;
    public int puntaje = 0;
    public int monedas = 0;
    public int nivelActual = 1;
    public float tiempoMaximo = 300f; // 5 minutos por nivel
    private float tiempoRestante;

    private ToadController toadController;

    private void Awake()
    {
        // Implementación del patrón Singleton
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
        tiempoRestante = tiempoMaximo;
        StartCoroutine(ContadorTiempo());
        SceneManager.sceneLoaded += OnSceneLoaded; // Suscribirse al evento de carga de escena
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Buscar a Toad en la nueva escena
        GameObject toadObject = GameObject.Find("Toad");
        if (toadObject != null)
        {
            toadController = toadObject.GetComponent<ToadController>();
            Debug.Log("Toad encontrado en la nueva escena.");
        }
        else
        {
            Debug.LogWarning("Toad no encontrado en la nueva escena.");
        }
    }

    private void Update()
    {
        if (toadController != null)
        {
            bool estaMuerto = toadController.muerto; // Cambia "miBool" por el nombre correcto
            if (estaMuerto == true )
            {
                GameOver();
            }
        }
    }

    public void AgregarPuntos(int cantidad)
    {
        puntaje += cantidad;
        Debug.Log("Puntaje: " + puntaje);
    }

    public void AgregarMoneda()
    {
        monedas++;
        Debug.Log("Monedas: " + monedas);
    }

    public void AgregarVida()
    {
        vidas++;
        Debug.Log("¡Vida extra! Vidas: " + vidas);
    }

    public void PerderVida()
    {
        vidas--;
        Debug.Log("Vidas restantes: " + vidas);

        if (vidas <= 0)
        {
            GameOver();
        }
        else
        {
            ReiniciarNivel();
        }
    }

    public void ReiniciarNivel()
    {
        tiempoRestante = tiempoMaximo;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CargarSiguienteNivel()
    {
        nivelActual++;
        tiempoRestante = tiempoMaximo;
        SceneManager.LoadScene(nivelActual);
    }

    private void GameOver()
    {
        Debug.Log("¡Game Over!");
        SceneManager.LoadScene("GameOverScene");
    }

    private IEnumerator ContadorTiempo()
    {
        while (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("¡Tiempo agotado!");
        PerderVida();
    }

    public float GetTiempoRestante()
    {
        return Mathf.Max(0, tiempoRestante);
    }
}