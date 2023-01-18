using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//There are two parts of this fitness test
// 1: distance to the plant sites from spawn areas. If the attacker spawn is closer to a plant site than defender spawn, then the map fails the test
// 2: Line of Sights. If there are many line of sights that are longer than the "maximum line of sight", then the map fails the test

public class FitnessTest : MonoBehaviour
{

    char[,] map = new char[200, 200];
    MapGenerator mapGen;
    public bool distTest;
    public int LoSTest;

        
    public bool Test(MapGenerator newMapGen) //Performs both tests on the map, returns a boolean pass or fail
    {
        mapGen = newMapGen;
        distTest = DistanceToSiteTest();
        LoSTest = NumOfLongLoS(50f);
        bool LoSTestBool = LoSTest < 5; // Max Long Lines of Sight is 5, if its greater than 5, the map fails the test
        return (LoSTestBool && distTest);
    }

    bool DistanceToSiteTest() //Uses a dijkstra path finding alorithm to find if attack spawn is closer to the plant sites than defence spawn. ALso checks that the spawns are roughly equidistant to mid
    {
        DijkstraPathFinding dijk = new DijkstraPathFinding();

        float AttackToA = dijk.PerformDijkstra(map, mapGen.attackerSpawn.GetPosition().x, mapGen.attackerSpawn.GetPosition().y, mapGen.aSite.GetPosition().x, mapGen.aSite.GetPosition().y);
        float AttackToB = dijk.PerformDijkstra(map, mapGen.attackerSpawn.GetPosition().x, mapGen.attackerSpawn.GetPosition().y, mapGen.bSite.GetPosition().x, mapGen.bSite.GetPosition().y);
        float DefToA = dijk.PerformDijkstra(map, mapGen.defenderSpawn.GetPosition().x, mapGen.defenderSpawn.GetPosition().y, mapGen.aSite.GetPosition().x, mapGen.aSite.GetPosition().y);
        float DefToB = dijk.PerformDijkstra(map, mapGen.defenderSpawn.GetPosition().x, mapGen.defenderSpawn.GetPosition().y, mapGen.bSite.GetPosition().x, mapGen.bSite.GetPosition().y);

        float DefToMid = dijk.PerformDijkstra(map, mapGen.defenderSpawn.GetPosition().x, mapGen.defenderSpawn.GetPosition().y, mapGen.mid.GetPosition().x, mapGen.mid.GetPosition().y);
        float AttackToMid = dijk.PerformDijkstra(map, mapGen.attackerSpawn.GetPosition().x, mapGen.attackerSpawn.GetPosition().y, mapGen.mid.GetPosition().x, mapGen.mid.GetPosition().y);

        bool midEquidistant = (DefToMid >= AttackToMid - (AttackToMid / 10) && DefToMid <= AttackToMid + (AttackToMid / 10));
        
        return (DefToA < AttackToA && DefToB < AttackToB && midEquidistant);
    }


    int NumOfLongLoS(float maxSight) 
    {
        //Goes through every element of the map 4 times, each time from a different direction.
        //Every time a free space is found, the number of free spaces until the next non-free space is counted
        //If that number is larger than the max sightline, 1 is added to the "LongLoSCount" variable

        char[,] Layout = mapGen.MapLayout;
        int LongLoSCount = 0;

        //Left to Right
        for (int i = 0; i < mapGen.mapSize; i++)
        {
            bool midLoS = false;
            float currLoSLength = 0f;
            for (int j = 0; j < mapGen.mapSize; j++)
            {
                if (Layout[i, j] != '1' && Layout[i, j] != 'C')
                {
                    if (midLoS)
                    {
                        currLoSLength += 1f;
                    }
                    else 
                    {
                        midLoS = true;
                        currLoSLength = 0f;
                    }
                }
                else
                {
                    if (midLoS)
                    {
                        midLoS = false;
                        if (currLoSLength > maxSight) { LongLoSCount++; }
                    }
                }
            }
        }

        //Top to Bottom
        for (int j = 0; j < mapGen.mapSize; j++)
        {
            bool midLoS = false;
            float currLoSLength = 0f;
            for (int i = 0; i < mapGen.mapSize; i++)
            {
                if (Layout[i, j] != '1' && Layout[i, j] != 'C')
                {
                    if (midLoS)
                    {
                        currLoSLength += 1f;
                    }
                    else
                    {
                        midLoS = true;
                        currLoSLength = 0f;
                    }
                }
                else
                {
                    if (midLoS)
                    {
                        midLoS = false;
                        if (currLoSLength > maxSight) { LongLoSCount++; }
                    }
                }
            }
        }
        
        //Diagonals, split into lower left and upper right
        //Lower Left
        for (int j = 0; j < mapGen.mapSize; j++)
        {
            bool midLoS = false;
            float currLoSLength = 0f;
            int i = 0;
            while (i + j < mapGen.mapSize)
            {
                if (Layout[i, i + j] != '1' && Layout[i, i+j] != 'C')
                {
                    if (midLoS)
                    {
                        currLoSLength += 1.414f;
                    }
                    else
                    {
                        midLoS = true;
                        currLoSLength = 0f;
                    }
                }
                else
                {
                    if (midLoS)
                    {
                        midLoS = false;
                        if (currLoSLength > maxSight) { LongLoSCount++; }
                    }
                }
                i++;
            }
        }
        
        //Upper Right
        for (int i = 0; i < mapGen.mapSize; i++)
        {
            bool midLoS = false;
            float currLoSLength = 0f;
            int j = 0;
            while (i + j < mapGen.mapSize)
            {
                if (Layout[i + j, j] != '1' && Layout[i + j, j] != 'C')
                {
                    if (midLoS)
                    {
                        currLoSLength += 1.414f;
                    }
                    else
                    {
                        midLoS = true;
                        currLoSLength = 0f;
                    }
                }
                else
                {
                    if (midLoS)
                    {
                        midLoS = false;
                        if (currLoSLength > maxSight) { LongLoSCount++; }
                    }
                }
                j++;
            }
        }
        
        //Diagonals in the other direction, split into upper left and lower right
        //Upper Left
        for (int j = 0; j < mapGen.mapSize; j++)
        {
            bool midLoS = false;
            float currLoSLength = 0f;
            int i = 0;
            while (j - i >= 0)
            {
                if (Layout[i, j - i] != '1' && Layout[i, j - i] != 'C')
                {
                    if (midLoS)
                    {
                        currLoSLength += 1.414f;
                    }
                    else
                    {
                        midLoS = true;
                        currLoSLength = 0f;
                    }
                }
                else
                {
                    if (midLoS)
                    {
                        midLoS = false;
                        if (currLoSLength > maxSight) { LongLoSCount++; }
                    }
                }
                i++;
            }
        }
        
        //Lower Right
        for (int j = 0; j < mapGen.mapSize; j++)
        {
            bool midLoS = false;
            float currLoSLength = 0f;
            int i = 0;
            while (j - i >= 0)
            {
                if (Layout[(mapGen.mapSize) - i, (mapGen.mapSize ) - (j - i)] != '1' && Layout[(mapGen.mapSize) - i, (mapGen.mapSize) - (j - i)] != 'C')
                {
                    if (midLoS)
                    {
                        currLoSLength += 1.414f;
                    }
                    else
                    {
                        midLoS = true;
                        currLoSLength = 0f;
                    }
                }
                else
                {
                    if (midLoS)
                    {
                        midLoS = false;
                        if (currLoSLength > maxSight) { LongLoSCount++; }
                    }
                }
                i++;
            }
        }
        return LongLoSCount;
    }
}
