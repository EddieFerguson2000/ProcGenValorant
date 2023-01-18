using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : KeyPoint
{
    public void AddSpawns(int num) //adds a given number of spawn locations to this area in randomised locations
    {
        int spawnCounter = 0;
        while (spawnCounter < num)
        {
            int randX = Random.Range(1, size.x - 1);
            int randY = Random.Range(1, size.y - 1);

            if (contents[randX][randY] != 's')
            {
                contents[randX][randY] = 's';
                spawnCounter++;
            }
        }
    }

    public void AddBomb() //adds a bomb to this area in a randomised location
    {
        bool bombCreated = false;
        while (!bombCreated)
        {
            int randX = Random.Range(size.x / 3, 2 * (size.x / 3));
            int randY = Random.Range(size.y / 3, 2 * (size.y / 3));

            if (contents[randX][randY] != 's')
            {
                contents[randX][randY] = 'b';
                bombCreated = true;
            }
        }
    }

}
