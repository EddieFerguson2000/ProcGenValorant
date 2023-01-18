using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class Enemy : MonoBehaviour
{
    public float health;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(float damage)
    {
        health -= damage;
    }
}
