using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloqueController : MonoBehaviour, IBloque
{
    private bool isBouncing = false;

    public virtual void Hit() //Detecta desde la interfaz si Toad ha chocado
    {

        if (isBouncing == false)
        {
            StartCoroutine(this.Bouncing());
        }
    }

    private IEnumerator Bouncing() //Crea la pequeña animacion en la que el bloque da un bote al ser golpeado
    {
        isBouncing = true;
        float time = 0f;
        float duration = 0.1f;


        Vector2 startPosition = this.transform.position;
        Vector2 endPosition = (Vector2)this.transform.position + (Vector2.up * 0.2f);

        while (time < duration)
        {
            this.transform.position = Vector2.Lerp(startPosition, endPosition, time / duration);
            time = time + Time.deltaTime;
            yield return null;
        }

        this.transform.position = endPosition;

        time = 0f;
        while (time < duration)
        {
            this.transform.position = Vector2.Lerp(endPosition, startPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        this.transform.position = startPosition;

        this.isBouncing = false;
    }
}
