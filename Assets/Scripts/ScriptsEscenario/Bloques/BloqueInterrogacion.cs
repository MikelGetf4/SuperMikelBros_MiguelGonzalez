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
            Vector3 posicionDentroDelBloque = transform.position + new Vector3(0, alturaBloque - 1.5f, 0); // Instanciar justo dentro del bloque

            // Instancia el objeto dentro del bloque
            GameObject powerUp = Instantiate(prefabAInstanciar, posicionDentroDelBloque, Quaternion.identity);

            // Llama a la corrutina para mover el power-up hacia arriba lentamente
            StartCoroutine(MoverSetaArriba(powerUp));
        }
        else
        {
            Debug.LogWarning("Prefab no asignado en " + gameObject.name);
        }
    }

    private IEnumerator MoverSetaArriba(GameObject powerUp)
    {
        // La distancia máxima que la seta debe subir
        float distanciaSubida = 1.0f; // Ajusta esta distancia según lo necesites
        float tiempoSubida = 1f; // Tiempo que tarda en subir (1 segundo)
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
}

public enum DentroBloque
{
    Seta,
    Flor,
    OneUp,
    Moneda
}
