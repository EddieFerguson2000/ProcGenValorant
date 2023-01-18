using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class Explode : MonoBehaviour
{
    public float expansionSpeed;
    public float minExpansion;
    public float expansionSlowdown;
    public float size;
    public float timeToDissapear;
    float timeSinceExploded;
    // Start is called before the first frame update
    void Start()
    {
        timeSinceExploded = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x < size)
        {
            transform.localScale = transform.lossyScale * expansionSpeed;
        }

        if (expansionSpeed > minExpansion)
        {
            expansionSpeed -= Time.deltaTime/expansionSlowdown;

        }
        else
        {
            expansionSpeed = minExpansion;
        }

        timeSinceExploded += Time.deltaTime;

        if (timeToDissapear < timeSinceExploded)
        {
            Destroy(gameObject);
        }

    }
}
