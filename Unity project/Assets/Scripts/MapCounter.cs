using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class MapCounter : MonoBehaviour
{
    GameObject player;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            player = GameObject.Find("Player(Clone)");
        }
        else
        {
            float x = 150 - player.transform.position.x;
            float y = player.transform.position.z;

            x *= (4f / 3f);
            y *= (4f / 3f);

            transform.position = new Vector3(startPos.x + x, startPos.y - y, transform.localPosition.z); 
        }
    }
}
