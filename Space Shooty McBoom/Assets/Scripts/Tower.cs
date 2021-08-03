using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Tower : MonoBehaviour
{

    [Header("Tower")]

    [SerializeField] Transform weapon;
    [SerializeField] float range = 10f;
    public float Range { get { return range; } }
    private float maxRange = 30f;
    [SerializeField] float damage = 2f;
    [SerializeField] float damageMod = 1f;
    private float maxDamage = 15f;
    public float Damage { get { return damage; } }
    float targetDistance;
    [SerializeField] ParticleSystem projectileParticle;
    public AudioSource audioSource;
    public AudioClip fire;
    Transform target;
    PlayerStats enemy;
    private void Start()
    {
        enemy = FindObjectOfType<PlayerStats>();
    }
    private void Update()
    {

        FindClosestTarget();
        AimWeapon();

    }



    void FindClosestTarget()
    {
       
        Transform closestTarget = null;
        float maxDistance = Mathf.Infinity;

        targetDistance = Vector3.Distance(transform.position, enemy.transform.position);
        if (targetDistance < maxDistance)
        {
            closestTarget = enemy.transform;
            maxDistance = targetDistance;
        }
   
        target = closestTarget;
    }
    void AimWeapon()
    {
        if (target != null)
        {
            targetDistance = Vector3.Distance(transform.position, target.position);

            weapon.LookAt(target);
            if (targetDistance <= range)
            {
                Attack(true);
            }
            else
            {
                Attack(false);
            }
        }
    }
    void Attack(bool isActive)
    {

        var emissionComp = projectileParticle.emission;
        emissionComp.enabled = isActive;
    }
}