using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"{name}I'm hit! by {other.gameObject.name}");
        if (other.gameObject.CompareTag("Player"))
        { 
            Destroy(gameObject);
        }
    }


}
