using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class EnemyCart : MonoBehaviour
{
    [SerializeField] CinemachineDollyCart enemyDollyCart;
    public float maxCartSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyDollyCart.m_Speed = maxCartSpeed;
        }
    }
}
