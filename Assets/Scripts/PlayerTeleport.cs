using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    
    [SerializeField] private GameObject floor1spawn;
    [SerializeField] private GameObject floorminus1spawn;
    [SerializeField] private GameObject spawnpoint;

    // Update is called once per frame
    void OnCollisionEnter(Collision collision) {

        if( collision.gameObject.tag.Equals("UpTeleporter"))
        {
            gameObject.transform.position = floor1spawn.transform.position;
            Debug.Log("Teleporting to floor 1.");
        }
        if( collision.gameObject.tag.Equals("DownTeleporter"))
        {
            gameObject.transform.position = floorminus1spawn.transform.position;
            Debug.Log("Teleporting to floor -1.");
        }
        if( collision.gameObject.tag.Equals("ToFloor0Teleporter"))
        {
            gameObject.transform.position = spawnpoint.transform.position;
            Debug.Log("Teleporting to floor 0.");
        }
    }
}
