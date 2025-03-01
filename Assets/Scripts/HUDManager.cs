using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("UI Elements")]
    public Text vidasTexto;
    public Text monedasTexto;
    public Text puntajeTexto;
    public Text tiempoTexto;

    private void Awake()
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

    // Actualiza todas las estadísticas en el HUD
    public void ActualizarHUD()
    {
        if (GameManager.Instance == null) return;

        vidasTexto.text = GameManager.Instance.vidas.ToString();
        monedasTexto.text = GameManager.Instance.monedas.ToString();
        puntajeTexto.text = GameManager.Instance.puntaje.ToString();

        float tiempo = GameManager.Instance.GetTiempoRestante();
        tiempoTexto.text = Mathf.CeilToInt(tiempo).ToString();
    }

}
