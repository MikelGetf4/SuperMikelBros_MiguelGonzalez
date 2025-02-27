using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloqueController : MonoBehaviour, IBloque
{
    private bool isBouncing = false;

    public virtual void Hit()
    {
        Debug.Log("Mario ha chocado");

        if (isBouncing == false)
        {
            StartCoroutine(this.Bouncing());
        }
    }

    private IEnumerator Bouncing()
    {
        isBouncing = true;
        float time = 0f;
        float duration = 0.1f;


        Vector2 startPosition = this.transform.position;
        Vector2 endPosition = (Vector2)this.transform.position + (Vector2.up * 0.5f);

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
