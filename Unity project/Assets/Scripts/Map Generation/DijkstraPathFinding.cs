using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fairly simple dijkstra path finding algorithm
public class DijkstraPathFinding : MonoBehaviour
{
    char[,] grid = new char[200, 200];
    int[,] gridPath = new int[200, 200];
	Vector2Int start, end;
    float dist;

    public float PerformDijkstra(char[,] newGrid, int sX, int sY, int eX, int eY)
    {
        dist = 0f;
        for (int i = 0; i < 150; i++)
        {
            for (int j = 0; j < 150; j++)
            {
                grid[i, j] = newGrid[i, j];
                gridPath[i, j] = -1;
            }
        }
        start.x = sX;
        start.y = sY;
        end.x = eX;
        end.y = eY;

        phase1();
        phase2();
        return dist;
    }

    bool phase1() //searches the grid until from the start point until it finds the end point
    {
		gridPath[start.x, start.y] = 0;
		bool endFound = false;
		bool thingFound = true;
		int counter = 0;

		while (endFound == false && thingFound == true && counter < 3000)
		{
			thingFound = false;
			for (int i = 0; i < 150; i++)
			{
				for (int j = 0; j < 150; j++)
				{
					if (gridPath[i, j] == counter)
					{
						thingFound = true;
						if (grid[i + 1, j] != '1' && gridPath[i + 1, j] == -1 && i < 149)
						{
							gridPath[i + 1, j] = counter + 1;
						}
						if (i + 1 == end.x && j == end.y) { endFound = true; }
						if (i > 0)
						{
							if (grid[i - 1, j] != '1' && gridPath[i - 1, j] == -1)
							{ gridPath[i - 1, j] = counter + 1; }
						}
						if (i - 1 == end.x && j == end.y) { endFound = true; }
						if (grid[i, j + 1] != '1' && gridPath[i, j + 1] == -1 && j < 149)
						{
							gridPath[i, j + 1] = counter + 1;
						}
						if (i == end.x && j + 1 == end.y) { endFound = true; }
						if (j > 0)
						{
							if (grid[i, j - 1] != '1' && gridPath[i, j - 1] == -1)
								gridPath[i, j - 1] = counter + 1;
						}

						if (i == end.x && j - 1 == end.y) { endFound = true; }
					}
				}
			}
			counter += 1;
		}
		if (thingFound == false)
		{
			return false;
		}
		else { return true; }
	}

    bool phase2() //Retraces steps from phase 1 while adding to the distance variable to find how long the shortest path is to the destination
    {
		List<Vector2Int> path = new List<Vector2Int>();

		Vector2Int currentPos;
		currentPos = end;
		bool found = false;
		path.Add(currentPos);

		//Cardinal directions add 1 to distance, while diagonals add 1.414f.
		while (found == false && dist < 5000)
		{
			if (gridPath[currentPos.x + 1, currentPos.y + 1] < gridPath[currentPos.x, currentPos.y] && gridPath[currentPos.x + 1, currentPos.y + 1] >= 0)
			{
				currentPos.x += 1;
				currentPos.y += 1;
				dist += 1.414f;
			}
			else if (gridPath[currentPos.x + 1, currentPos.y - 1] < gridPath[currentPos.x, currentPos.y] && gridPath[currentPos.x + 1, currentPos.y - 1] >= 0)
			{
				currentPos.x += 1;
				currentPos.y -= 1;
				dist += 1.414f;
			}
			else if (gridPath[currentPos.x - 1, currentPos.y + 1] < gridPath[currentPos.x, currentPos.y] && gridPath[currentPos.x - 1, currentPos.y + 1] >= 0)
			{
				currentPos.x -= 1;
				currentPos.y += 1;
				dist += 1.414f;
			}
			else if (gridPath[currentPos.x - 1, currentPos.y - 1] < gridPath[currentPos.x, currentPos.y] && gridPath[currentPos.x - 1, currentPos.y - 1] >= 0)
			{
				dist += 1.414f;
				currentPos.x -= 1;
				currentPos.y -= 1;
			}
			else if (gridPath[currentPos.x + 1, currentPos.y] < gridPath[currentPos.x, currentPos.y] && gridPath[currentPos.x + 1, currentPos.y] >= 0)
			{
				dist += 1;
				currentPos.x += 1;
			}
			else if (gridPath[currentPos.x - 1, currentPos.y] < gridPath[currentPos.x, currentPos.y] && gridPath[currentPos.x - 1, currentPos.y] >= 0)
			{
				currentPos.x -= 1;
				dist += 1;

			}
			else if (gridPath[currentPos.x, currentPos.y + 1] < gridPath[currentPos.x, currentPos.y] && gridPath[currentPos.x, currentPos.y + 1] >= 0)
			{
				currentPos.y += 1;
				dist += 1;

			}
			else if (gridPath[currentPos.x, currentPos.y - 1] < gridPath[currentPos.x, currentPos.y] && gridPath[currentPos.x, currentPos.y - 1] >= 0)
			{
				currentPos.y -= 1;
				dist += 1;

			}


			if (currentPos.x == start.x && currentPos.y == start.y) { found = true; }
			path.Add(currentPos);
		}

		int pathSize = path.Count;

		for (int i = 0; i < pathSize; i++)
		{
			path.RemoveAt(path.Count - 1);
		}
		return true;
	}
}
