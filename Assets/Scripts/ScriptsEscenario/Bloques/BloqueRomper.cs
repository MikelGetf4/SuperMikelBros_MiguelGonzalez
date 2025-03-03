using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloqueRomper : BloqueController, IBloque
{
    private Animator animator;
    //Todos los prefabs de los fragmentos
    [SerializeField] private GameObject fragmentoArribaIzquierda;
    [SerializeField] private GameObject fragmentoArribaDerecha;
    [SerializeField] private GameObject fragmentoAbajoIzquierda;
    [SerializeField] private GameObject fragmentoAbajoDerecha;

    [SerializeField] private float fuerza = 3f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Hit() //Si Toad es grande, hace que los bloques se rompan y salgan fragmentos volando
    {
        ToadController toad = FindObjectOfType<ToadController>();

        if (toad != null && toad.estado != ToadStatus.Small)

        {
            RomperBloque();
        }

        else
        {
            base.Hit();
        }
    }

    private void RomperBloque() //Crea los fragmentos del bloque que salen disparados
    {
        CrearFragmento(fragmentoArribaIzquierda, new Vector2(-1, 2));
        CrearFragmento(fragmentoArribaDerecha, new Vector2(1, 2));
        CrearFragmento(fragmentoAbajoIzquierda, new Vector2(-1, -1));
        CrearFragmento(fragmentoAbajoDerecha, new Vector2(1, -1));

        Destroy(gameObject);
    }

    private void CrearFragmento(GameObject prefab, Vector2 direccion) //Crea individualmente cada fragmento y le da sus propiedades
    {
        GameObject fragmento = Instantiate(prefab, transform.position, Quaternion.identity);

        Rigidbody2D rbFragmento = fragmento.GetComponent<Rigidbody2D>();

        if (rbFragmento != null)
        {
            rbFragmento.gravityScale = 2f;
            rbFragmento.velocity = new Vector2(0, 2f);
            rbFragmento.AddForce(direccion * fuerza, ForceMode2D.Impulse);
        }

        Destroy(fragmento, 3f);
    }
}
