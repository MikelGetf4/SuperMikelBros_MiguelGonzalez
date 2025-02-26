using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    public Transform cameraTransform;
    public float offset = 13.7f;
    void Update()
    {
        transform.position = new Vector3(cameraTransform.position.x - offset, transform.position.y, transform.position.z);
    }
}
