using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthPackController : MonoBehaviour
{
    private BoxCollider colliderComp;
    public UnityEvent onHealthPickup;

    // Start is called before the first frame update
    void Start()
    {
        colliderComp = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            onHealthPickup.Invoke();
            Debug.Log("Got items");
            Destroy(gameObject);
        }
    }

    public void EndRound()
    {
        Destroy(gameObject);
    }
}
