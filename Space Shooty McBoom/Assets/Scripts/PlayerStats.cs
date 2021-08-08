using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerStats : MonoBehaviour
{
    public float health = 100;
    public Slider healthUI;    
    
    // Start is called before the first frame update
    void Start()
    {
        healthUI.maxValue = health;
        healthUI.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log($"{name}I'm hit! by {other.gameObject.name}");
        if (!other.gameObject.CompareTag("Player"))
        {
            health -= 5f;
            UpdateSlider();
        }
        
        
       
    }
    private void OnCollisionEnter(Collision other)
    {
        health -= 5f;
        UpdateSlider();
        
    }
    private void UpdateSlider()
    {
        healthUI.value = health;
    }

}
