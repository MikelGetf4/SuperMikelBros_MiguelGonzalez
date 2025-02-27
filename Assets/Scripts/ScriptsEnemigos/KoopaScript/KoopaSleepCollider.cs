using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaSleepCollider : MonoBehaviour
{
    private KoopaController koopaController;
    [SerializeField]
    private bool IsSleep = false;
    [SerializeField]
    private Collider2D sleepCollider; 

    private void Awake()
    {
        koopaController = GetComponentInParent<KoopaController>();
        sleepCollider = GetComponentInParent<Collider2D>();
    }

    private void Start()
    {
        sleepCollider.enabled = false;
    }

    private void Update()
    {
        if (koopaController.sleeping != sleepCollider.enabled)
        {
            StartCoroutine(ActivarCollider(0.5f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que Mario tenga el tag "Player"
        {
            koopaController.LaunchKoopa();
        }
    }

    private IEnumerator ActivarCollider(float delay)
    {
        // Espera 0.5 segundos antes de activar el collider
        yield return new WaitForSeconds(delay);

        // Ahora activamos el collider
        sleepCollider.enabled = true;
    }

}
