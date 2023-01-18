using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Resets the scene. Destroys everything and then spawns a new level generator, new UI, and new round manager
public class Refresh : MonoBehaviour
{
    public GameObject levelGen;
    public GameObject UI;
    public GameObject roundManager;
    public Camera minimapCam;

    // Update is called once per frame
    void Update() //Spawns maps with set seeds based on the number key pressed. The enter/return key uses a random seed
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DeleteAll(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DeleteAll(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DeleteAll(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DeleteAll(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DeleteAll(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DeleteAll(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            DeleteAll(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            DeleteAll(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            DeleteAll(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            DeleteAll(9);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DeleteAll(10);
        }
    }

    public void DeleteAll(int seed) //Deletes everything except itself
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            if (o.name != "SceneManager")
            {
                Destroy(o);
            }
        }
        GameObject newLevel = Instantiate(levelGen);
        newLevel.GetComponent<spawnLevel>().seed = seed;
        Instantiate(UI);
        Instantiate(roundManager);
        Instantiate(minimapCam);
    }
}
