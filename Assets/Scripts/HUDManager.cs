using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; } //Instancia el HUD

    public Text vidasTexto;
    public Text monedasTexto;
    public Text puntajeTexto;
    public Text tiempoTexto;

    private void Awake() //Se asegura de que solo haya un HUD
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        ActualizarHUD();
    }

    public void ActualizarHUD()   // Actualiza todas las variables en el HUD
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        vidasTexto.text = GameManager.Instance.vidas.ToString();
        monedasTexto.text = GameManager.Instance.monedas.ToString();
        puntajeTexto.text = GameManager.Instance.puntaje.ToString();

        float tiempo = GameManager.Instance.GetTiempoRestante();
        tiempoTexto.text = Mathf.CeilToInt(tiempo).ToString();
    }

}
