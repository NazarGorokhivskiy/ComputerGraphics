using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivatePickUp : MonoBehaviour {
    void Start() {
        Invoke("Deactivate", Random.Range(6f, 10f));
    }

    void Deactivate() {
        gameObject.SetActive(false);
    }
}