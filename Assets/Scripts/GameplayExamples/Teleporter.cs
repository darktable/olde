using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

    [SerializeField]
    Transform target;

    void OnTriggerEnter(Collider other)
    {
        other.transform.position = target.position;
		other.transform.rotation = target.rotation;
    }
}
