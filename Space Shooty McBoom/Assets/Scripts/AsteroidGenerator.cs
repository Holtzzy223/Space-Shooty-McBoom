﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class AsteroidGenerator : MonoBehaviour
{
    public float spawnRange;
    public float amountToSpawn;
    private Vector3 spawnPoint;
    public GameObject asteroid;
    public float startSafeRange;
    private List<GameObject> objectsToPlace = new List<GameObject>();
    public GameObject player;
    public CinemachineSmoothPath track;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            PickSpawnPoint();

            //pick new spawn point if too close to player start
            while (Vector3.Distance(spawnPoint, transform.position) < startSafeRange)
            {
                PickSpawnPoint();
            }

            objectsToPlace.Add(Instantiate(asteroid, spawnPoint, Quaternion.Euler(Random.Range(0f,360f), Random.Range(0f, 360f), Random.Range(0f, 360f))));
            objectsToPlace[i].transform.parent = this.transform;
        }

        asteroid.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickSpawnPoint()
    {
        spawnPoint = new Vector3(
            Random.Range(-1f,1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));

     //   if(spawnPoint.magnitude > 1)
     //   {
     //       spawnPoint.Normalize();
     //   }
        if (spawnPoint == player.transform.position) { PickSpawnPoint(); return;}
        //for (int i = 0; i < track.m_Waypoints.Length; i++)
        //{         
        //    if (spawnPoint == track.m_Waypoints[i].position) { PickSpawnPoint();return;}
        //}
        spawnPoint *= spawnRange;
        
    }
}

