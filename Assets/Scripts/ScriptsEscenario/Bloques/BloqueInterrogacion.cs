using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloqueInterrogacion : BloqueController, IBloque
{
    private Animator animator;
    private bool golpeado = false;

    [Header("Power-Up Settings")]
    public DentroBloque dentroBloque; // Selecciona qué objeto generar
    public GameObject[] powerUpPrefabs; // Array de prefabs: 0 = Seta, 1 = Flor, 2 = OneUp, 3 = Moneda
    public Transform spawnPoint; // Punto donde aparecerá el objeto

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public override void Hit()
    {
        if (!golpeado)
        {
            golpeado = true;
            


            ToadController jugador = FindObjectOfType<ToadController>();

            if (jugador != null && dentroBloque != DentroBloque.OneUp && dentroBloque != DentroBloque.Moneda)
            {
                if (jugador.estado == ToadStatus.Small)
                {
                    dentroBloque = DentroBloque.Seta;
                }
                else
                {
                    dentroBloque = DentroBloque.Flor;
                }
            }

            GenerarObjeto();
            animator.SetTrigger("Golpeado");
        }
    }

    private void GenerarObjeto()
    {
        GameObject prefabAInstanciar = null;

        // Determina qué objeto instanciar según el enum
        switch (dentroBloque)
        {
            case DentroBloque.Seta:
                prefabAInstanciar = powerUpPrefabs[0];
                break;
            case DentroBloque.Flor:
                prefabAInstanciar = powerUpPrefabs[1];
                break;
            case DentroBloque.OneUp:
                prefabAInstanciar = powerUpPrefabs[2];
                break;
            case DentroBloque.Moneda:
                prefabAInstanciar = powerUpPrefabs[3];
                break;
        }

        // Verifica si hay un prefab válido
        if (prefabAInstanciar != null)
        {
            // Obtiene el tamaño del bloque para calcular la posición dentro del bloque (un poco hacia abajo del borde superior)
            float alturaBloque = GetComponent<Collider2D>().bounds.size.y;
            Vector3 posicionDentroDelBloque = transform.position + new Vector3(0, alturaBloque - 1f, 1); // Instanciar justo dentro del bloque

            // Instancia el objeto dentro del bloque
            GameObject powerUp = Instantiate(prefabAInstanciar, posicionDentroDelBloque, Quaternion.identity);

            if(dentroBloque == DentroBloque.Moneda)
            {
                StartCoroutine(MoverMonedaArriba(powerUp));
            }
            else
            {
                // Llama a la corrutina para mover el power-up hacia arriba lentamente
                StartCoroutine(MoverObjetoArriba(powerUp));
            }
            
        }
        else
        {
            Debug.LogWarning("Prefab no asignado en " + gameObject.name);
        }
    }

    private IEnumerator MoverObjetoArriba(GameObject powerUp)
    {
        // La distancia máxima que la seta debe subir
        float distanciaSubida = 0.5f; // Ajusta esta distancia según lo necesites
        float tiempoSubida = 0.6f; // Tiempo que tarda en subir (1 segundo)
        Vector3 posicionInicial = powerUp.transform.position;
        Vector3 posicionFinal = posicionInicial + new Vector3(0, distanciaSubida, 0); // Mueve hacia arriba

        float tiempoTranscurrido = 0f;

        // Mueve el power-up de abajo hacia arriba
        while (tiempoTranscurrido < tiempoSubida)
        {
            powerUp.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, tiempoTranscurrido / tiempoSubida);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Asegura que se coloque en la posición final
        powerUp.transform.position = posicionFinal;
    }

    private IEnumerator MoverMonedaArriba(GameObject moneda)
    {
        GameManager.Instance.AgregarMoneda();
        GameManager.Instance.AgregarPuntos(200);

        float alturaSubida = 2.5f;  // Cuánto sube la moneda
        float alturaBajada = 0.5f;  // Cuánto baja antes de desaparecer
        float tiempoSubida = 0.3f;  // Tiempo rápido para subir
        float tiempoBajada = 0.2f;  // Tiempo para bajar un poco

        Vector3 posicionInicial = moneda.transform.position;
        Vector3 posicionFinalSubida = posicionInicial + new Vector3(0, alturaSubida, 0);
        Vector3 posicionFinalBajada = posicionFinalSubida - new Vector3(0, alturaBajada, 0);

        float tiempoTranscurrido = 0f;

        // 1. Movimiento hacia arriba
        while (tiempoTranscurrido < tiempoSubida)
        {
            moneda.transform.position = Vector3.Lerp(posicionInicial, posicionFinalSubida, tiempoTranscurrido / tiempoSubida);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        moneda.transform.position = posicionFinalSubida;

        // 2. Pequeña bajada
        tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < tiempoBajada)
        {
            moneda.transform.position = Vector3.Lerp(posicionFinalSubida, posicionFinalBajada, tiempoTranscurrido / tiempoBajada);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        moneda.transform.position = posicionFinalBajada;

        // 3. Destruir moneda después de un pequeño retraso
        yield return new WaitForSeconds(0.1f);
        Destroy(moneda);
    }
}

public enum DentroBloque
{
    Seta,
    Flor,
    OneUp,
    Moneda
}
