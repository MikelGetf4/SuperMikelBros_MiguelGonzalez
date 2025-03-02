using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    //IGUAL QUE EL MURO ACTIVAR, PERO ESTE SOLO SE MUEVE POR DETRAS DE LA CAMARA, IMPIDIENDO EL PASO DE TOAD
    public Transform cameraTransform; //Transform de la camara
    public float offset = 13.7f; //Distancia del centro de la camara a la que permanecera constamente el muro
    void Update() //Se mueve junto a la camara
    {
        transform.position = new Vector3(cameraTransform.position.x - offset, transform.position.y, transform.position.z);
    }
}
