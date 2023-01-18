using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Maps
{
    public List<List<char>> cont;
    public List<List<char>> height;
    public List<KeyPoint.Exit> exits;
    public bool needsMorePlantSites;
    public bool addMoreCover;
    public int attPoints;
    public int defPoints;
}


public class KeyPoint : MonoBehaviour
{

    public class Exit //These dictate how paths form between key points.
    {
        public ExitGimmick gimmick;
        public Vector2Int exitPosition = new Vector2Int();
        public Vector2Int exitPositionRelative = new Vector2Int();
        public Vector2Int exitSize = new Vector2Int();
        public int height;
        public char exitMapChar = '0';
        public bool open = false;
    }

    protected ExitGimmicks exitGimmicks = new ExitGimmicks();
    protected ManipTools manipTools = new ManipTools();

    protected Vector2Int position, size;
    protected List<List<char>> contents = new List<List<char>>();
    protected List<List<char>> heightMap = new List<List<char>>();
    protected List<Exit> exits = new List<Exit>();
    protected char groundType;
    protected char groundType2;

    protected bool ShouldAddCover = true;

    protected int attackerPoints;
    protected int defenderPoints;

    public void InitialisePoint(char mapChar, Vector2Int pSize, int ground1, int ground2)
    {
        SetSize(pSize);

        groundType = ground1.ToString().ToCharArray()[0];
        groundType2 = ground2.ToString().ToCharArray()[0];
        
        //Sets the entire square site to a flat ground
        for (int i = 0; i < size.x; i++)
        {
            List<char> line = new List<char>();
            List<char> heightLine = new List<char>();

            for (int j = 0; j < size.y; j++)
            {
                line.Add(groundType);
                heightLine.Add('0');
            }
            contents.Add(line);
            heightMap.Add(heightLine);
        }

        //Initialises things unique to each type of point
        InitialiseArea();

    }

    public void SetPosition(Vector2Int newPos)
    {
        position = newPos;
    }

    public void SetSize(Vector2Int newSize)
    {
        size = newSize;
    }

    public List<List<char>> GetSite()
    {
        return contents;
    }

    public List<List<char>> GetHeight()
    {
        return heightMap;
    }

    public Vector2Int GetSize()
    {
        return size;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public void SetExits() //Initialises exits for a basic point.
    {
        Exit northExit = new Exit();
        northExit.exitMapChar = 'N';
        northExit.exitSize.x = 5 + Random.Range(0, 4);
        northExit.exitSize.y = 0;
        northExit.exitPositionRelative.x = (northExit.exitSize.x / 2) + Random.Range(0, (size.x / 2)-1);
        northExit.exitPositionRelative.y = 0;
        northExit.height = 0;

        northExit.exitPosition.x = position.x;
        northExit.exitPosition.y = position.y + (size.y / 2);
        exits.Add(northExit);


        Exit eastExit = new Exit();
        eastExit.exitMapChar = 'E';
        eastExit.exitSize.x = 0;
        eastExit.exitSize.y = 5 + Random.Range(0, 4);
        eastExit.exitPosition.x = position.x + (size.x / 2);
        eastExit.exitPosition.y = position.y;
        eastExit.exitPositionRelative.x = size.x - 1;
        eastExit.exitPositionRelative.y = Random.Range(0, size.y-1);
        eastExit.height = 0;

        if (eastExit.exitPositionRelative.y <= eastExit.exitSize.y / 2)
        {
            eastExit.exitPositionRelative.y = eastExit.exitSize.y / 2;
        }
        else if (eastExit.exitPositionRelative.y >= size.y - (eastExit.exitSize.y / 2))
        {
            eastExit.exitPositionRelative.y = size.y - (eastExit.exitSize.y / 2);
        }
        exits.Add(eastExit);


        Exit southExit = new Exit();
        southExit.exitMapChar = 'S';
        southExit.exitSize.x = 5 + Random.Range(0, 4);
        southExit.exitSize.y = 0;
        southExit.exitPosition.x = position.x;
        southExit.exitPosition.y = position.y - (size.y / 2);
        southExit.exitPositionRelative.x = (southExit.exitSize.x / 2) + Random.Range(0, (size.x / 2)-1);
        southExit.exitPositionRelative.y = size.y - 1;
        southExit.height = 0;

        exits.Add(southExit);


        Exit westExit = new Exit();
        westExit.exitMapChar = 'W';
        westExit.exitSize.x = 0;
        westExit.exitSize.y = 5 + Random.Range(0, 4);
        westExit.exitPosition.x = position.x - (size.x / 2);
        westExit.exitPosition.y = position.y;
        westExit.exitPositionRelative.x = 0;
        westExit.exitPositionRelative.y = Random.Range(0, size.y-1);
        westExit.height = 0;

        if (westExit.exitPositionRelative.y <= westExit.exitSize.y / 2)
        {
            westExit.exitPositionRelative.y = westExit.exitSize.y / 2;
        }
        else if (westExit.exitPositionRelative.y >= size.y - (westExit.exitSize.y / 2))
        {
            westExit.exitPositionRelative.y = size.y - (westExit.exitSize.y / 2);
        }
        exits.Add(westExit);

    }



    public Vector2Int GetExitPosition(int exit) //Returns the co-ordinate of any exit (not local coordinate)
    {
        Vector2Int exitPos = position;
        exitPos.x += exits[exit].exitPositionRelative.x - (size.x / 2);
        exitPos.y += exits[exit].exitPositionRelative.y - (size.y / 2);
        return exitPos;
    }

    public Vector2Int GetExitSize(int exit)
    {
        return exits[exit].exitSize;
    }

    public void AddCover(int min, int max)
    {
        if (ShouldAddCover)
        {
            int numCover; 
            if (min == max)
            {
                numCover = min;
            }
            else
            {
                numCover = Random.Range(min, max);
            }
            //More cover gives an advantage to the defender, therefor defender points are added to the balancing system that will help to even out the site later
            defenderPoints += numCover - min;
            for (int c = 0; c < numCover; c++)
            {
                char coverChar;

                //Cover position
                int x = Random.Range((size.x / 4), (size.x - (size.x / 4)) - 1);
                int y = Random.Range((size.y / 4), (size.y - (size.y / 4) - 1));

                //Always tries to block the sightline from attacker spawn to defender spawn
                if (c == 0)
                {
                    x = (exits[0].exitPositionRelative.x + exits[3].exitPositionRelative.x) / 2;
                    y = (exits[0].exitPositionRelative.y + exits[3].exitPositionRelative.y) / 2;
                }
                int coverType = Random.Range(0, 12);
                int shortBlocks;

                switch (coverType) //Implementing the cover. The "coverType" variable decides what shape the cover will be in
                {
                    case 0: // rectangle
                        manipTools.CreateSquare(contents, new Vector2Int(Random.Range(3, 5), Random.Range(4, 8)), size, new Vector2Int(x, y), 'C');
                        break;

                    case 1: // rectangle
                        manipTools.CreateSquare(contents, new Vector2Int(Random.Range(4, 8), Random.Range(3, 5)), size, new Vector2Int(x, y), 'C');
                        break;

                    case 2: // square
                        int coverSize = Random.Range(3, 4);
                        manipTools.CreateSquare(contents, new Vector2Int(coverSize, coverSize), size, new Vector2Int(x, y), 'C');
                        break;

                    case 3: // short square
                        shortBlocks = Random.Range(2, 4);
                        manipTools.CreateSquare(contents, new Vector2Int(shortBlocks, shortBlocks), size, new Vector2Int(x, y), 'c');
                        break;

                    case 4: // L-shape with the little part being shorter
                        shortBlocks = Random.Range(2, 4);
                        manipTools.CreateSquare(contents, new Vector2Int(shortBlocks, shortBlocks), size, new Vector2Int(x, y), 'c');
                        if (Random.value < 0.5)
                            manipTools.CreateSquare(contents, new Vector2Int(shortBlocks * 2, shortBlocks), size, new Vector2Int(x + shortBlocks, y - (shortBlocks / 2)), 'C');
                        else
                            manipTools.CreateSquare(contents, new Vector2Int(shortBlocks * 2, shortBlocks), size, new Vector2Int(x + shortBlocks, y + (shortBlocks / 2)), 'C');
                        break;

                    case 5: // L-shape in other direction
                        shortBlocks = Random.Range(2, 4);
                        manipTools.CreateSquare(contents, new Vector2Int(shortBlocks, shortBlocks), size, new Vector2Int(x, y), 'c');
                        if (Random.value < 0.5)
                            manipTools.CreateSquare(contents, new Vector2Int(shortBlocks * 2, shortBlocks), size, new Vector2Int(x - shortBlocks + 1, y - (shortBlocks / 2) - 1), 'C');
                        else
                            manipTools.CreateSquare(contents, new Vector2Int(shortBlocks * 2, shortBlocks), size, new Vector2Int(x - shortBlocks + 1, y + (shortBlocks / 2)), 'C');
                        break;

                    case 6: // Square of short cover with an inner square of tall cover
                        shortBlocks = Random.Range(2, 4);
                        manipTools.CreateSquare(contents, new Vector2Int(shortBlocks + 2, shortBlocks + 2), size, new Vector2Int(x, y), 'c');
                        manipTools.CreateSquare(contents, new Vector2Int(shortBlocks, shortBlocks), size, new Vector2Int(x, y), 'C');
                        break;

                    case 7: // single line of cover in X direction
                        if (Random.value < 0.5) // 50% chance that half the wall is short
                            manipTools.CreateSquare(contents, new Vector2Int(Random.Range(4, 8), 1), size, new Vector2Int(x, y), 'C');
                        else
                        {
                            shortBlocks = Random.Range(-2, 2);
                            manipTools.CreateSquare(contents, new Vector2Int(8, 1), size, new Vector2Int(x, y), 'c');
                            manipTools.CreateSquare(contents, new Vector2Int(4, 1), size, new Vector2Int(x + shortBlocks, y), 'C');
                        }
                        break;

                    case 8: // single line of cover in Y direction
                        if (Random.value < 0.5) // 50% chance that half the wall is short
                            manipTools.CreateSquare(contents, new Vector2Int(1, Random.Range(4, 8)), size, new Vector2Int(x, y), 'C');
                        else
                        {
                            shortBlocks = Random.Range(-2, 2);
                            manipTools.CreateSquare(contents, new Vector2Int(8, 1), size, new Vector2Int(x, y), 'C');
                            manipTools.CreateSquare(contents, new Vector2Int(4, 1), size, new Vector2Int(x, y + shortBlocks), 'c');
                        }
                        break;

                    case 9: // square with second square offset by (1,1) or (-1, 1)
                        manipTools.CreateSquare(contents, new Vector2Int(2, 2), size, new Vector2Int(x, y), 'C');
                        if (Random.value < 0.5) { manipTools.CreateSquare(contents, new Vector2Int(2, 2), size, new Vector2Int(x + 1, y + 1), 'C'); }
                        else { manipTools.CreateSquare(contents, new Vector2Int(2, 2), size, new Vector2Int(x - 1, y + 1), 'C'); }
                        break;

                    case 10: // T-Shape
                        if (Random.value < 0.5) { coverChar = 'C'; }
                        else { coverChar = 'c'; }

                        manipTools.CreateSquare(contents, new Vector2Int(6, 2), size, new Vector2Int(x, y), 'C');
                        if (Random.value < 0.5) { manipTools.CreateSquare(contents, new Vector2Int(2, 2), size, new Vector2Int(x, y + 2), coverChar); }
                        else { manipTools.CreateSquare(contents, new Vector2Int(2, 2), size, new Vector2Int(x, y - 2), coverChar); }
                        break;

                    case 11: // T-Shape flipped 90 degrees
                        if (Random.value < 0.5) { coverChar = 'C'; }
                        else { coverChar = 'c'; }

                        manipTools.CreateSquare(contents, new Vector2Int(2, 6), size, new Vector2Int(x, y), 'C');
                        if (Random.value < 0.5) { manipTools.CreateSquare(contents, new Vector2Int(2, 2), size, new Vector2Int(x + 2, y), coverChar); }
                        else { manipTools.CreateSquare(contents, new Vector2Int(2, 2), size, new Vector2Int(x - 2, y), coverChar); }
                        break;

                }
            }
        }
    }


    public void SetGroundType(int type, int firstSecond)
    {
        if (firstSecond == 1)
        {
            groundType = type.ToString().ToCharArray()[0];
        }
        else
        {
            groundType2 = type.ToString().ToCharArray()[0];
        }
    }

    public char GetGroundType()
    {
        return groundType.ToString().ToCharArray()[0];
    }

    public float GetExitHeight(int exit)
    {
        return exits[exit].height;
    }

    public void SetExitStatus(int exit, bool isOpen)
    {
        exits[exit].open = isOpen;
    }

    public bool GetExitStatus(int exit)
    {
        return exits[exit].open;
    }

    public virtual void InitialiseArea() //This initialises aspects that are unique to the type of point.
    {
        SetExits();
    }

    public void ExitGimmick(int exitNum, bool positive) //Adds a gimmick to an exit that makes it easier or harder for attackers to make it onto site
    {
        ImplementationReturn exitGim;

        exitGim = exitGimmicks.ImplementExitGimmick(contents, heightMap, exits[exitNum], positive);

        contents = exitGim.point;
        heightMap = exitGim.heightMap;
        exits[exitNum] = exitGim.exit;

    }

    public Vector2Int GetAttackDefensePoints()
    {
        return new Vector2Int(attackerPoints, defenderPoints);
    }
    
    public void SetAttackDefensePoints(Vector2Int newPoints)
    {
        attackerPoints = newPoints.x;
        defenderPoints = newPoints.y;
    }
}
