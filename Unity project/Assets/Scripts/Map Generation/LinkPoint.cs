using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Branch //These are used to connect the link points to other key points on the map
{
    public Vector2Int endPos;
    public Vector2Int endSize;
    public float endHeight;
    public KeyPoint dest;
}

public class LinkPoint : KeyPoint
{
    List<Branch> branches;

    // Start is called before the first frame update
    void Start()
    {
        branches = new List<Branch>();
    }

    public void AddBranch(int entrySide, KeyPoint destination) // Adds a new branch that connects from this link point to the destination at the specified exit
    {
        Branch newBranch = new Branch();

        //if entryside is 10 that means its connected to a point that doesnt really have meaningful exits (such as another link point), and so simply sets the points centre as its destination
        if (entrySide == 10) 
        { 
            newBranch.endPos = destination.GetPosition();
            newBranch.endSize = destination.GetExitSize(Random.Range(0, 3)) ;
            newBranch.endHeight = 0;
        }
        else 
        {
            destination.SetExitStatus(entrySide, true);
            newBranch.endPos = destination.GetExitPosition(entrySide);
            newBranch.endSize = destination.GetExitSize(entrySide);
            newBranch.endHeight = destination.GetExitHeight(entrySide);
        }

        newBranch.dest = destination;

        if (branches == null)
        {
            //It's null - create it
            branches = new List<Branch>();
        }
        branches.Add(newBranch);
    }
    public List<Branch> GetBranches()
    {
        return branches;
    }
}
