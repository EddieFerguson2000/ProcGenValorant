using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//These are essentially used as balancing tools for the plant sites
//They alter the exits/entrances onto site in order to make them more or less challenging for attackers to push onto site
public enum ExitGimmick { Drop, Doors, Window, StepUpTo, NoFiftyFifty, None };
public struct ImplementationReturn //This is just a struct used to return all the altered values
{
    public List<List<char>> point;
    public List<List<char>> heightMap;
    public KeyPoint.Exit exit;
}

public class ExitGimmicks : MonoBehaviour
{
    ManipTools manipTools = new ManipTools();

    public ImplementationReturn ImplementExitGimmick(List<List<char>> point, List<List<char>> height, KeyPoint.Exit exit, bool positive)
    {
        //There are different gimmicks depending on whether the change should be "positive" or not for the attackers
        if(!positive)
        {
            exit.gimmick = (ExitGimmick)Random.Range(0, 3);
        }
        else
        {
            exit.gimmick = (ExitGimmick)Random.Range(3, 5);
        }

        switch (exit.gimmick)
        {
           
            case ExitGimmick.Doors: //Adds a set of doors similar to Dust from Counter Strike
                char doorChar;
                if (exit.exitMapChar == 'W' || exit.exitMapChar == 'E')
                {
                    doorChar = 'd';
                }
                else
                {
                    doorChar = 'D';
                }
                point[exit.exitPositionRelative.x][exit.exitPositionRelative.y] = doorChar;
                break;
            case ExitGimmick.Window: //Adds a small hurdle that the attackers have to jump over which would alert the defending team and lower attackers accuracy for a moment
                if (exit.exitMapChar == 'W' || exit.exitMapChar == 'E')
                {
                    manipTools.CreateSquare(height, new Vector2Int(1, exit.exitSize.y + 5), new Vector2Int(height.Count, height[0].Count), exit.exitPositionRelative, '3');
                }
                else
                {
                    manipTools.CreateSquare(height, new Vector2Int(exit.exitSize.x + 5, 1), new Vector2Int(height.Count, height[0].Count), exit.exitPositionRelative, '3');
                }
                break;
            case ExitGimmick.Drop: //raises the exit so that attackers are forced to drop onto site, alerting defenders. Also means defenders can hide under drop, so attackers must consider a third dimension when peaking
                exit.height = Random.Range(6, 10);
                Vector2Int coverPos = new Vector2Int();
                //Also adds a small piece of cover next to the drop to give a way to get back up
                {
                    if (exit.exitMapChar == 'W' || exit.exitMapChar == 'E')
                    {
                        coverPos.x = exit.exitPositionRelative.x;
                        coverPos.y = exit.exitPositionRelative.y + 2;
                    }
                    else
                    {
                        coverPos.x = exit.exitPositionRelative.x + 2;
                        coverPos.y = exit.exitPositionRelative.y;
                    }
                    if (!manipTools.FillElementIfItExists(point, coverPos.x, coverPos.y, 'C'))
                    {
                        manipTools.FillElementIfItExists(point, exit.exitPositionRelative.x, exit.exitPositionRelative.y, 'C');
                    }
                    if (coverPos.x + 1 > point.Count)
                    {
                        manipTools.FillElementIfItExists(point, coverPos.x - 1, coverPos.y, 'c');
                    }
                    else
                    {
                        manipTools.FillElementIfItExists(point, coverPos.x + 1, coverPos.y, 'c');
                    }
                }
                break;
            case ExitGimmick.StepUpTo: // Adds a couple of steps that attackers can use to alter the height that they peak from. Means defenders don't have a set in stone head height to aim at
                if (exit.height == 0)
                {
                    //Also adds two blocks on either side of the entrance, this is simply to allow the steps to be on site without having defenders see the attackers as they climb up
                    if (exit.exitMapChar == 'S')
                    {
                        manipTools.CreateSquare(point, new Vector2Int(3, 5), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x - 3, point[0].Count - 2), '1');
                        manipTools.CreateSquare(point, new Vector2Int(3, 5), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x + 3, point[0].Count - 2), '1');
                        manipTools.FillElementIfItExists(point, exit.exitPositionRelative.x + 1, point[0].Count - 3, 'c');
                        manipTools.FillElementIfItExists(point, exit.exitPositionRelative.x + 1, point[0].Count - 4, 'C');
                    }
                    else if (exit.exitMapChar == 'W')
                    {
                        manipTools.CreateSquare(point, new Vector2Int(3, 3), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x + 1, exit.exitPositionRelative.y - 3), '1');
                        manipTools.CreateSquare(point, new Vector2Int(3, 3), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x + 1, exit.exitPositionRelative.y + 3), '1');
                        manipTools.FillElementIfItExists(point, exit.exitPositionRelative.x, exit.exitPositionRelative.y + 1, 'c');
                        manipTools.FillElementIfItExists(point, exit.exitPositionRelative.x + 1, exit.exitPositionRelative.y + 1, 'C');
                    }
                    else if (exit.exitMapChar == 'E')
                    {
                        manipTools.CreateSquare(point, new Vector2Int(3, 3), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x - 2, exit.exitPositionRelative.y - 3), '1');
                        manipTools.CreateSquare(point, new Vector2Int(3, 3), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x - 2, exit.exitPositionRelative.y + 3), '1');
                        manipTools.FillElementIfItExists(point, exit.exitPositionRelative.x - 2, exit.exitPositionRelative.y + 1, 'c');
                        manipTools.FillElementIfItExists(point, exit.exitPositionRelative.x - 3, exit.exitPositionRelative.y + 1, 'C');
                    }
                }
                break;

            case ExitGimmick.NoFiftyFifty: // Places a large cover on one side of the exit to reduce the number of angles defenders can defend the entrance from
                if (exit.height == 0)
                {
                    if (exit.exitMapChar == 'S')
                    {
                        if (manipTools.IsValidElement(exit.exitPositionRelative.x + 3, point[0].Count - 2, point.Count, point[0].Count))
                        {
                            manipTools.CreateSquare(point, new Vector2Int(4, 4), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x + 2, point[0].Count - 2), '1');
                        }
                        else
                        {
                            manipTools.CreateSquare(point, new Vector2Int(4, 4), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x - 2, point[0].Count - 2), '1');
                        }
                    }
                    else if (exit.exitMapChar == 'W')
                    {
                        if (manipTools.IsValidElement(exit.exitPositionRelative.x + 2, exit.exitPositionRelative.y - 2, point.Count, point[0].Count))
                        {
                            manipTools.CreateSquare(point, new Vector2Int(4, 4), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x + 2, exit.exitPositionRelative.y - 3), '1');
                        }
                        else
                        {
                            manipTools.CreateSquare(point, new Vector2Int(4, 4), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x + 2, exit.exitPositionRelative.y + 3), '1');
                        }
                    }
                    else if (exit.exitMapChar == 'E')
                    {
                        if (manipTools.IsValidElement(exit.exitPositionRelative.x + 2, exit.exitPositionRelative.y - 2, point.Count, point[0].Count))
                        {
                            manipTools.CreateSquare(point, new Vector2Int(4, 4), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x - 2, exit.exitPositionRelative.y - 3), '1');
                        }
                        else
                        {
                            manipTools.CreateSquare(point, new Vector2Int(4, 4), new Vector2Int(point.Count, point[0].Count), new Vector2Int(exit.exitPositionRelative.x - 2, exit.exitPositionRelative.y + 3), '1');
                        }
                    }
                }
                break;

        }


        ImplementationReturn impRet;
        impRet.point = point;
        impRet.heightMap = height;
        impRet.exit = exit;
        return impRet;
    }
}
