using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloqueInterrogacion : BloqueController, IBloque
{
    private Animator animator;
    private bool golpeado = false;

    public DentroBloque dentroBloque; // Selecciona que objeto generar
    public GameObject[] powerUpPrefabs; // Array de prefabs: 0 = Seta, 1 = Flor, 2 = OneUp, 3 = Moneda
    public Transform spawnPoint; // Punto donde apareceran los objeto

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


            //Todos los bloques dan setas, pero si Toad es grande o de fuego, se cambiara para que den una Flor de fuego

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

            GenerarObjeto(); //Genera el objeto

            animator.SetTrigger("Golpeado"); //Le dice al animator que ha sido golpeado
        }
    }

    private void GenerarObjeto()
    {
        GameObject prefabAInstanciar = null;

        // Determina qué objeto instanciar segun el enum
        switch (dentroBloque)
        {
            case DentroBloque.Seta: //Si el enum es Seta, se genera el Prefab 0 (Una seta)
                prefabAInstanciar = powerUpPrefabs[0];
                break;
            case DentroBloque.Flor: //Si el enum es Flor, se genera el Prefab 1 (Una flor)
                prefabAInstanciar = powerUpPrefabs[1];
                break;
            case DentroBloque.OneUp: //Si el enum es OneUp, se genera el Prefab 2 (Un One Up)
                prefabAInstanciar = powerUpPrefabs[2];
                break;
            case DentroBloque.Moneda: //Si el enum es Moneda, se genera el Prefab 3 (Una moneda)
                prefabAInstanciar = powerUpPrefabs[3];
                break;
        }

        if (prefabAInstanciar != null)
        {
            // Obtiene el tamaño del bloque para calcular la posición dentro del bloque (un poco hacia abajo del borde superior)
            float alturaBloque = GetComponent<Collider2D>().bounds.size.y;
            Vector3 posicionDentroDelBloque = transform.position + new Vector3(0, alturaBloque - 1f, 1); // Instanciar justo dentro del bloque

            // Instancia el objeto dentro del bloque
            GameObject powerUp = Instantiate(prefabAInstanciar, posicionDentroDelBloque, Quaternion.identity);

            if(dentroBloque == DentroBloque.Moneda)
            {
                //Si dentro del bloque hay una moneda, hara la animacion de la moneda
                StartCoroutine(MoverMonedaArriba(powerUp));
            }
            else
            {
                //Si dentro del bloque hay cualquier cosa menos una moneda, hara la animacion de los power Up
                StartCoroutine(MoverObjetoArriba(powerUp));
            }
            
        }        
    }

    private IEnumerator MoverObjetoArriba(GameObject powerUp)
    {
        // La distancia máxima que la seta debe subir
        float distanciaSubida = 0.5f; // Ajusta esta distancia segun lo necesites
        float tiempoSubida = 0.6f; // Tiempo que tarda en subir
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
        GameManager.Instance.AgregarMoneda(); //Suma moneda al GameManager
        GameManager.Instance.AgregarPuntos(200); //Agrega puntos al GameManager

        float alturaSubida = 2.5f;  // Cuanto sube la moneda
        float alturaBajada = 0.5f;  // Cuanto baja antes de desaparecer
        float tiempoSubida = 0.3f;  // Tiempo rapido para subir
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
