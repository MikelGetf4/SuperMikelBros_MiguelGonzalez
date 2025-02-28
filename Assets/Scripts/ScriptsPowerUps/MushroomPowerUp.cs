using Assets.Scripts;
using UnityEngine;

public class MushroomPowerUp : MonoBehaviour
{
    [SerializeField]
    GameObject Toad;

    public void Activate()
    {
        if (Toad != null) // Evitar NullReferenceException
        {
            var toadController = Toad.GetComponent<ToadController>();
            if (toadController != null)
            {
                toadController.PowerUpSeta();
            }
        }
    }
}

