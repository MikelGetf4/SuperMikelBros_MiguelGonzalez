using Assets.Scripts;
using UnityEngine;

public class MushroomPowerUp : MonoBehaviour
{
    [SerializeField]
    GameObject Toad;

    public void Activate() //Al coger Toad la seta, usa su funcion PowerUpSeta();
    {
        if (Toad != null) 
        {
            var toadController = Toad.GetComponent<ToadController>();
            if (toadController != null)
            {
                toadController.PowerUpSeta();
            }
        }
    }
}

