using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class PickUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Attacker")
        {
            collision.collider.GetComponent<PlayerMovement>().pickUpBomb(gameObject);
        }
    }

    //private void OnCollisionEnter(Collider other)

}
