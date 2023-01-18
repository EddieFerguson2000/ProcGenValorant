using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class just has generic tools for checking that values exist or changing elements of 2D arrays
public class ManipTools : MonoBehaviour
{
    public bool FillElementIfItExists(char[,] map, int xPos, int yPos, char fill)
    {
        if (IsValidElement(xPos, yPos, map.Length))
        {
            map[xPos, yPos] = fill;
            return true;
        }
        return false;
    }
    
    public bool FillElementIfItExists(List<List<char>> map, int xPos, int yPos, char fill)
    {
        if (IsValidElement(xPos, yPos, map.Count, map[0].Count))
        {
            map[xPos][yPos] = fill;
            return true;
        }
        return false;
    }

    public bool IsValidElement(int x, int y, int mapSize)
    {
        if (x >= 0 && x < mapSize && y >= 0 && y < mapSize)
        {
            return true;
        }
        else return false;
    }
    
    public bool IsValidElement(int x, int y, int mapSizeX, int mapSizeY)
    {
        if (x >= 0 && x < mapSizeX && y >= 0 && y < mapSizeY)
        {
            return true;
        }
        else return false;
    }

    public void CreateSquare(List<List<char>> contents, Vector2Int squareSize, Vector2Int pointSize, Vector2Int pos, char fill) //Creates a square of a given character in a given 2D list of characters
    {
        
        for (int i = 0; i < squareSize.x; i++)
        {
            int currX = pos.x - (squareSize.x / 2) + i;
            if (contents.Count > currX) //If element exists
            {
                for (int j = 0; j < squareSize.y; j++)
                {
                    int currY = pos.y - (squareSize.y / 2) + j;
                    if (currX < contents.Count && currX >= 0) //If element exists
                    {
                        if (currY < contents.Count && currY >= 0) //If element exists
                        {
                            if (contents[currX].Count > currY && contents[currX][currY] != 'H')
                            {
                                if (contents[currX][currY] == '1' && (fill == 'c' || fill == 'C')) { }
                                else { contents[currX][currY] = fill; }
                            }
                        }
                    }
                }
            }

        }
    }

    public void CreateCircle(List<List<char>> contents, float rad, Vector2Int size, Vector2Int pos, char fill) //Creates a circle of a given character in a given 2D list of characters
    {
        for (int j = 0; j < (size.y); j++)
        {
            for (int i = 0; i < (size.x); i++)
            {
                float dist = Mathf.Sqrt(((i - pos.x) * (i - pos.x)) + ((j - pos.y) * (j - pos.y)));

                if (dist < rad)
                {
                    FillElementIfItExists(contents, i, j, fill);
                }
            }
        }
    }

}
