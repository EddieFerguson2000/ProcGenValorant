using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    enum MapTheme { Sandy, Forest, SnowyFacility, Town, SnowTown, Resort, Random };

    //Tools for altering the 2D arrays
    ManipTools ManipTools = new ManipTools();

    //Creating variables
    public int mapSize;
    public char[,] MapLayout = new char[200, 200];
    public char[,] MapHeight = new char[200, 200];

    public PlantSite aSite = new PlantSite();
    public PlantSite bSite = new PlantSite();
    public Spawn attackerSpawn = new Spawn();
    public Spawn defenderSpawn = new Spawn();
    public MidArea mid = new MidArea();
    KeyPoint aLobby = new KeyPoint();
    KeyPoint bLobby = new KeyPoint();

    LinkPoint[] AttackerASideLinks = new LinkPoint[6];
    LinkPoint[] AttackerBSideLinks = new LinkPoint[5];
    LinkPoint[] DefenderSideLinks = new LinkPoint[5];

    string aSiteLinkConfig;
    string bSiteLinkConfig;

    //ground order { 2 = Sand, 3 = Grass, 4 = Stone, 5 = Metal, 6 = Wood, 7 = Water, 8 = Snow, 9 = Mud, 10 = Tile };

    MapTheme mapTheme;
    //The numbers in these arrays correspond to different materials
    int[] sandyGrounds = { 2, 4, 5, 6 };
    int[] forestyGrounds = { 3, 4, 6, 7 };
    int[] townGrounds = { 4, 5, 6, 7 };
    int[] resortGrounds = { 2, 3, 4, 5, 6, 7 };
    int[] snowyGrounds = { 4, 5, 8 };
    int[] snowTownGrounds = { 4, 5, 6, 8 };
    int[] randomGrounds = { 2, 3, 4, 5, 6, 7, 8 };
    
    public char[,] GetMapLayout()
    {
        return MapLayout;
    }
    
    public char[,] GetHeightMap()
    {
        return MapHeight;
    }

    public void Create(int levSize)
    {
        //Init map Theme (basically just colour palette)
        mapTheme = Theme(Random.Range(0, 7));

        //initialising the entire map as flat, featureless ground
        mapSize = levSize;
        for (int j = 0; j < (mapSize); j++)
        {
            for (int i = 0; i < (mapSize); i++)
            {
                if (j == 0 || j == mapSize - 1 || i == 0 || i == mapSize - 1)
                {
                    MapLayout[i, j] = 'H';

                }
                else
                {
                    MapLayout[i, j] = '1';
                }

                MapHeight[i, j] = '0';
            }
        }

        //Creating all the points, giving them a size and two ground colours
        Vector2Int pSize = PointSizeGen(400, 30, 10, 10, 10);
        attackerSpawn.InitialisePoint('0', pSize, GetGround(), GetGround());

        pSize = PointSizeGen(1500, 25, 20, 25, 20);
        mid.InitialisePoint('0', pSize, GetGround(), GetGround());

        pSize = PointSizeGen(650, 20, 10, 20, 10);
        aSite.InitialisePoint('0', pSize, GetGround(), GetGround());

        pSize = PointSizeGen(150, 10, 5, 10, 5);
        aLobby.InitialisePoint('0', pSize, GetGround(), GetGround());

        pSize = PointSizeGen(650, 20, 10, 20, 10);
        bSite.InitialisePoint('0', pSize, GetGround(), GetGround());

        pSize = PointSizeGen(150, 10, 5, 10, 5);
        bLobby.InitialisePoint('0', pSize, GetGround(), GetGround());

        pSize = PointSizeGen(400, 30, 10, 10, 10);
        defenderSpawn.InitialisePoint('0', pSize, GetGround(), GetGround());

        for (int i = 0; i < 5; i++)
        {
            pSize = new Vector2Int(Random.Range(7, 15), Random.Range(7, 15));
            AttackerASideLinks[i] = new LinkPoint();
            AttackerASideLinks[i].InitialisePoint('0', pSize, GetGround(), GetGround());

            pSize = new Vector2Int(Random.Range(7, 15), Random.Range(7, 15));
            AttackerBSideLinks[i] = new LinkPoint();
            AttackerBSideLinks[i].InitialisePoint('0', pSize, GetGround(), GetGround());

            pSize = new Vector2Int(Random.Range(7, 15), Random.Range(7, 15));
            DefenderSideLinks[i] = new LinkPoint();
            DefenderSideLinks[i].InitialisePoint('0', pSize, GetGround(), GetGround());
        }

        pSize = new Vector2Int(Random.Range(7, 15), Random.Range(7, 15));
        AttackerASideLinks[5] = new LinkPoint();
        AttackerASideLinks[5].InitialisePoint('0', pSize, GetGround(), GetGround());

        //Position every point
        CreateLayout();

        //Connect the link points and the exits/entrances to points
        ConnectLinkPoints();

        //Attempt to balance the plant sites if they are not already deemed balanced
        BalancePoint(aSite);
        BalancePoint(bSite);

        //farve paths from each link point to all of their destinations
        for (int i = 0; i < 5; i++)
        {
            FormPath(AttackerASideLinks[i]);
            FormPath(AttackerBSideLinks[i]);
            FormPath(DefenderSideLinks[i]);
        }
        FormPath(AttackerASideLinks[5]);

        //Add the points to the overall map
        AddPoint(mid);
        AddPoint(aSite);
        AddPoint(bSite);
        AddPoint(aLobby);
        AddPoint(bLobby);

        //Adding spawn points and the bomb to the spawn areas
        defenderSpawn.AddSpawns(5);
        attackerSpawn.AddSpawns(5);
        attackerSpawn.AddBomb();

        //Adding cover where sightlines are too long
        AddCover();

        //Smoothing off jagged edges
        Finalise();

        //adding in the spawn areas
        AddPoint(defenderSpawn);
        AddPoint(attackerSpawn);


    }

    void AddPoint(KeyPoint point) //Adds the given points contents to the overall map
    {
        List<List<char>> pointContents = point.GetSite();
        List<List<char>> pointHeight = point.GetHeight();
        Vector2Int pos = point.GetPosition();
        Vector2Int size = point.GetSize();

        for (int j = (pos.y - (size.y / 2)); j < (pos.y + (size.y / 2)) - 1; j++)
        {
            for (int i = (pos.x - (size.x / 2)); i < (pos.x + (size.x / 2)) - 1; i++)
            {
                if (ManipTools.IsValidElement(i, j, mapSize))
                {
                    MapLayout[i, j] = pointContents[i - (pos.x - (size.x / 2))][j - (pos.y - (size.y / 2))];
                    MapHeight[i, j] = pointHeight[i - (pos.x - (size.x / 2))][j - (pos.y - (size.y / 2))];
                }
            }
        }
    }

    void CreateLayout() //Decides where each key point should be
    {
        int ranX, ranY;

        //Defender Spawn
        ranX = ((mapSize / 10) * 3) + Random.Range(0, ((mapSize / 10) * 4)-1);
        ranY = defenderSpawn.GetSize().y / 2 + Random.Range(0, (mapSize / 10)-1);
        defenderSpawn.SetPosition(new Vector2Int(ranX, ranY));

        //AttackerSpawn
        ranX = ((mapSize / 10) * 3) + Random.Range(0, ((mapSize / 10) * 4)-1);
        ranY = mapSize - Random.Range(0, (mapSize / 3)-1);
        while (ranY > mapSize - attackerSpawn.GetSize().y) { ranY = mapSize - Random.Range(0, (mapSize / 3)-1); }
        attackerSpawn.SetPosition(new Vector2Int(ranX, ranY));

        //Mid
        ranX = mapSize / 2;
        mid.SetPosition(new Vector2Int(ranX, ranX));

        //A Site
        ranX = Random.Range(15 + (aSite.GetSize().x / 2), mid.GetPosition().x - (mid.GetSize().x / 2) - 15);
        ranY = (defenderSpawn.GetPosition().y + defenderSpawn.GetSize().y + aSite.GetSize().y + Random.Range(0, (mapSize / 3)-1));
        while (ranY > mapSize / 2) { ranY = (defenderSpawn.GetPosition().y + defenderSpawn.GetSize().y + aSite.GetSize().y + Random.Range(0, (mapSize / 3)-1)); }
        aSite.SetPosition(new Vector2Int(ranX, ranY));

        //A Lobby
        ranX = ((mid.GetExitPosition(4).x + aSite.GetPosition().x) / 2);
        ranY = (attackerSpawn.GetExitPosition(0).y + aSite.GetExitPosition(3).y) / 2;
        aLobby.SetPosition(new Vector2Int(ranX, ranY));

        //A LINKS

        //Link Points A Side 0 (Connects to the side of A from link 1 or 2)
        ranX = (aSite.GetExitPosition(3).x - 10);
        if (aSite.GetExitPosition(4).y - aSite.GetExitPosition(5).y == 0) { ranY = aSite.GetExitPosition(5).y; }
        else { ranY = (aSite.GetExitPosition(5).y + Random.Range(0, (aSite.GetExitPosition(4).y - aSite.GetExitPosition(5).y)-1)); }
        AttackerASideLinks[0].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points A Side 2 (connects to the bottom of A, the left of A Lobby, and links 0/1)
        ranX = aSite.GetExitPosition(3).x;
        ranY = aSite.GetExitPosition(3).y + 10;
        AttackerASideLinks[2].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points A Side 1 (link from 0 to 2)
        ranX = AttackerASideLinks[0].GetPosition().x;
        ranY = AttackerASideLinks[2].GetPosition().y;
        AttackerASideLinks[1].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points A Side 3 (connects from A Lobby to the left side of Mid)
        ranX = (aLobby.GetExitPosition(1).x + mid.GetExitPosition(4).x) / 2;
        if (aLobby.GetExitPosition(1).y - mid.GetExitPosition(4).y == 0) { ranY = mid.GetExitPosition(4).y; }
        else { ranY = mid.GetExitPosition(4).y + Random.Range(0, (aLobby.GetExitPosition(1).y - mid.GetExitPosition(4).y)-1); }
        AttackerASideLinks[3].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points A Side 4 (connects Attack Spawn to A Lobby/bottom of mid)
        ranX = (attackerSpawn.GetPosition().x + aLobby.GetPosition().x) / 2;
        ranY = (attackerSpawn.GetExitPosition(0).y + aLobby.GetExitPosition(2).y) / 2;
        AttackerASideLinks[4].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points A Side 5 (connects attacker spawn to mid, as well as A Link 3/4 AND B Link 3/4)
        ranX = (attackerSpawn.GetPosition().x + mid.GetPosition().x) / 2;
        ranY = (attackerSpawn.GetExitPosition(0).y + mid.GetExitPosition(3).y) / 2;
        AttackerASideLinks[5].SetPosition(new Vector2Int(ranX, ranY));


        //B Site
        ranX = Random.Range(mid.GetPosition().x + (mid.GetSize().x / 2) + 15, mapSize - 15 - (bSite.GetSize().x / 2));
        ranY = (defenderSpawn.GetPosition().x + defenderSpawn.GetSize().y + aSite.GetSize().y + Random.Range(0, (mapSize / 3)-1));
        while (ranY > mapSize / 2) { ranY = (defenderSpawn.GetPosition().y + defenderSpawn.GetSize().y + aSite.GetSize().y + Random.Range(0, (mapSize / 3)-1)); }
        bSite.SetPosition(new Vector2Int(ranX, ranY));

        //B Lobby
        ranX = ((mid.GetExitPosition(2).x + bSite.GetPosition().x) / 2);
        ranY = (attackerSpawn.GetExitPosition(0).y + bSite.GetExitPosition(3).y) / 2;
        bLobby.SetPosition(new Vector2Int(ranX, ranY));


        //B LINKS

        //Link Points B Side 0 (Connects to the side of B from link 1 or 2)
        ranX = (bSite.GetExitPosition(2).x + 10);
        if (bSite.GetExitPosition(1).y - bSite.GetExitPosition(2).y == 0) { ranY = bSite.GetExitPosition(5).y; }
        else { ranY = bSite.GetExitPosition(5).y + Random.Range(0, (bSite.GetExitPosition(1).y - bSite.GetExitPosition(2).y)-1); }
        AttackerBSideLinks[0].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points B Side 2 (connects to the bottom of B, the right of B Lobby, and links 0/1)
        ranX = bSite.GetExitPosition(3).x;
        ranY = bSite.GetExitPosition(3).y + 10;
        AttackerBSideLinks[2].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points B Side 1 (link from 0 to 2)
        ranX = AttackerBSideLinks[0].GetPosition().x;
        ranY = AttackerBSideLinks[2].GetPosition().y;
        AttackerBSideLinks[1].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points B Side 3 (connects from B Lobby to the right side of Mid)
        ranX = (bLobby.GetExitPosition(3).x + mid.GetExitPosition(1).x) / 2;
        if (bLobby.GetExitPosition(3).y - mid.GetExitPosition(1).y == 0)
        { ranY = mid.GetExitPosition(1).y; }
        else
        { ranY = mid.GetExitPosition(1).y + Random.Range(0, (bLobby.GetExitPosition(3).y - mid.GetExitPosition(1).y)-1); }
        AttackerBSideLinks[3].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points B Side 4 (connects Attack Spawn to B Lobby/bottom of mid)
        ranX = (attackerSpawn.GetExitPosition(1).x + bLobby.GetPosition().x) / 2;
        ranY = (attackerSpawn.GetExitPosition(0).y + bLobby.GetExitPosition(2).y) / 2;
        AttackerBSideLinks[4].SetPosition(new Vector2Int(ranX, ranY));


        //DEFENDER LINKS

        //Link Points Defence 0 (above A Site)
        ranX = (aSite.GetExitPosition(0).x + defenderSpawn.GetExitPosition(3).x) / 2;
        ranY = (aSite.GetExitPosition(0).y + defenderSpawn.GetExitPosition(3).y) / 2;
        DefenderSideLinks[0].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points Defence 1 (links a site, mid and defender spawn)
        ranX = (aSite.GetExitPosition(1).x + mid.GetExitPosition(5).x) / 2;
        ranY = (aSite.GetExitPosition(1).y + mid.GetExitPosition(5).y) / 2;
        DefenderSideLinks[1].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points Defence 2 (links straight from defender spawn to mid)
        ranX = (defenderSpawn.GetExitPosition(2).x + mid.GetExitPosition(0).x) / 2;
        ranY = (defenderSpawn.GetExitPosition(2).y + mid.GetExitPosition(0).y) / 2;
        DefenderSideLinks[2].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points Defence 3 (links b site, mid and defender spawn)
        ranX = (bSite.GetExitPosition(5).x + mid.GetExitPosition(2).x) / 2;
        ranY = (bSite.GetExitPosition(5).y + mid.GetExitPosition(2).y) / 2;
        DefenderSideLinks[3].SetPosition(new Vector2Int(ranX, ranY));

        //Link Points Defence 4(above B Site)
        ranX = (bSite.GetExitPosition(0).x + defenderSpawn.GetExitPosition(1).x) / 2;
        ranY = (bSite.GetExitPosition(0).y + defenderSpawn.GetExitPosition(1).y) / 2;
        DefenderSideLinks[4].SetPosition(new Vector2Int(ranX, ranY));
    }

    void ConnectLinkPoints() //A series of random number generations are used to affect how link points connect to each other, leading to unique, varied approaches to the plant sites and the middle area
    {
        //A Side
        {
            //50/50 attackers entry to A site is left side vs bottom
            
            if (Random.value < 0.5 && AttackerASideLinks[0].GetPosition().x > 2 && AttackerASideLinks[1].GetPosition().x > 2) //Left side
            {
                aSiteLinkConfig = "Left";
                AttackerASideLinks[0].AddBranch(4, aSite);

                AttackerASideLinks[0].AddBranch( 10, AttackerASideLinks[1]);
                AttackerASideLinks[1].AddBranch( 10, AttackerASideLinks[2]);

                int randConn = Random.Range(0, 3);
                if (randConn == 0)
                {
                    AttackerASideLinks[1].AddBranch( 10, AttackerASideLinks[4]);
                }
                else if (randConn == 1)
                {
                    AttackerASideLinks[1].AddBranch( 3, attackerSpawn);
                }
            }
            else // bottom
            {
                aSiteLinkConfig = "Bottom";
                AttackerASideLinks[2].AddBranch( 3, aSite);
                int randConn = Random.Range(0, 4);
                if (randConn != 3)
                {
                    AttackerASideLinks[2].AddBranch(10, AttackerASideLinks[1]);
                    AttackerASideLinks[1].AddBranch(3, attackerSpawn);
                }
            }

            //Always attach A Lobby to A Link [2]
            AttackerASideLinks[2].AddBranch( 3, aLobby);

            //Attach ATTACKER SPAWN to A LOBBY
            AttackerASideLinks[4].AddBranch( 2, aLobby);
            AttackerASideLinks[4].AddBranch( 3, attackerSpawn);

            //attacker side mid Configuration has a few possible configurations
            int attackerMidConfig = Random.Range(0, 9);
            if (attackerMidConfig < 3)
            {
                AttackerASideLinks[3].AddBranch( 1, aLobby);
                AttackerASideLinks[3].AddBranch( 4, mid);
                AttackerBSideLinks[3].AddBranch( 3, bLobby);
                AttackerBSideLinks[3].AddBranch( 1, mid);

                attackerMidConfig = Random.Range(0, 8);
                if (attackerMidConfig < 3)
                {
                    AttackerASideLinks[5].AddBranch( 10, AttackerBSideLinks[3]);
                    AttackerASideLinks[5].AddBranch( 0, attackerSpawn);
                }
                else if (attackerMidConfig < 6)
                {
                    AttackerASideLinks[5].AddBranch( 10, AttackerASideLinks[3]);
                    AttackerASideLinks[5].AddBranch( 0, attackerSpawn);
                }
            }
            else
            {
                //Attach ATTACKER SPAWN to MID BOTTOM
                AttackerASideLinks[5].AddBranch( 3, mid);
                AttackerASideLinks[5].AddBranch( 0, attackerSpawn);

                if (attackerMidConfig >= 3 && attackerMidConfig < 6)
                {
                    AttackerASideLinks[3].AddBranch( 1, aLobby);
                    AttackerASideLinks[3].AddBranch( 4, mid);
                }
                else if (attackerMidConfig >= 6 && attackerMidConfig < 9)
                {
                    AttackerBSideLinks[3].AddBranch( 3, bLobby);
                    AttackerBSideLinks[3].AddBranch( 1, mid);
                }
            }
        }

        //B Side
        {
            //50/50 attackers entry to A site is left side vs bottom
            if (Random.value < 0.5 && AttackerBSideLinks[0].GetPosition().x < mapSize - 2) //Left side
            {
                bSiteLinkConfig = "Right";

                //Attach to upper left vs lower left
                //if (Random.Range(0, 1) == 0) { AttackerBSideLinks[0].AddBranch( 2, bSite); }
                //else {  }
                AttackerBSideLinks[0].AddBranch(1, bSite);

                AttackerBSideLinks[0].AddBranch( 10, AttackerBSideLinks[1]);
                AttackerBSideLinks[1].AddBranch( 10, AttackerBSideLinks[2]);

                int randConn = Random.Range(0, 3);
                if (randConn == 0)
                {
                    AttackerBSideLinks[1].AddBranch( 10, AttackerBSideLinks[4]);
                }
                else if (randConn == 1)
                {
                    AttackerBSideLinks[1].AddBranch( 1, attackerSpawn);
                }
            }
            else // bottom
            {
                bSiteLinkConfig = "Bottom";
                AttackerBSideLinks[2].AddBranch( 3, bSite);
                int randConn = Random.Range(0, 4);
                if (randConn != 3)
                {
                    AttackerBSideLinks[2].AddBranch(10, AttackerBSideLinks[1]);
                    AttackerBSideLinks[1].AddBranch(1, attackerSpawn);
                }
            }

            //Always attach A Lobby to A Link [2]
            AttackerBSideLinks[2].AddBranch( 3, bLobby);

            //Attach ATTACKER SPAWN to A LOBBY
            AttackerBSideLinks[4].AddBranch( 3, bLobby);
            AttackerBSideLinks[4].AddBranch( 1, attackerSpawn);


        }

        //Defender Side
        {
            DefenderSideLinks[0].AddBranch( 0, aSite);
            DefenderSideLinks[0].AddBranch( 3, defenderSpawn);
            DefenderSideLinks[4].AddBranch( 0, bSite);
            DefenderSideLinks[4].AddBranch( 1, defenderSpawn);

            //Link 2 (between a site and mid)
            {
                
                DefenderSideLinks[1].AddBranch( 2, aSite);

                if (Random.value < 0.5)
                {
                    DefenderSideLinks[1].AddBranch( 10, DefenderSideLinks[2]);
                }
                else
                {
                    if (Random.value < 0.5)
                    {
                        DefenderSideLinks[1].AddBranch( 2, defenderSpawn);
                    }
                    else
                    {
                        DefenderSideLinks[1].AddBranch( 3, defenderSpawn);
                    }
                }
            }

            //Link 4 (between b site and mid)
            {
                
                DefenderSideLinks[3].AddBranch( 5, bSite);

                if (Random.value < 0.5)
                {
                    DefenderSideLinks[3].AddBranch( 10, DefenderSideLinks[2]);
                }
                else
                {
                    if (Random.value < 0.5)
                    {
                        DefenderSideLinks[3].AddBranch( 2, defenderSpawn);
                    }
                    else
                    {
                        DefenderSideLinks[3].AddBranch( 1, defenderSpawn);
                    }
                }
            }

            //Defender Middle
            int defMidRand = Random.Range(0, 2);
            DefenderSideLinks[2].AddBranch( 2, defenderSpawn);

            if (defMidRand < 3)
            {
                DefenderSideLinks[1].AddBranch( 5, mid);
                DefenderSideLinks[3].AddBranch( 2, mid);

                if (Random.value < 0.5)
                {
                    DefenderSideLinks[1].AddBranch( 10, DefenderSideLinks[2]);
                }
                else
                {
                    DefenderSideLinks[3].AddBranch(10, DefenderSideLinks[2]);   
                }

            }
        }
    }

    void FormPath(LinkPoint link) //Creates the paths between points based on the position of "link points" and the exits of other points
    {
        //Creates a path for every branch that a link point has
        //A branch is a struct that contains the destination position, height and some other stuff
        List<Branch> branches = link.GetBranches();
        if (branches != null)
        {
            for (int i = 0; i < branches.Count; i++)
            {
                char mapChar = GetGround().ToString().ToCharArray()[0];

                Vector2Int currentPos = link.GetPosition();

                Vector2Int endPos = branches[i].endPos;

                float totalDist = Mathf.Sqrt(Mathf.Pow((endPos.x - currentPos.x), 2) + Mathf.Pow((endPos.y - currentPos.y), 2));
                int pathSize = Random.Range(1, 2);
                int infiniteLoopPrevention = 0;

                while ((currentPos.x != endPos.x || currentPos.y != endPos.y) && infiniteLoopPrevention < 3000)
                {
                    //Calculates what the height of the element should be based on the height of the destination, and how far from the destination this element is
                    float newDist = Mathf.Sqrt(Mathf.Pow((endPos.x - currentPos.x), 2) + Mathf.Pow((endPos.y - currentPos.y), 2));
                    int height = (int)(branches[i].endHeight * ((totalDist - newDist) / totalDist));
                    height = Mathf.Clamp(height, 0, 9);
                    char hght = height.ToString().ToCharArray()[0];

                    //Get closer to the destination
                    if (Mathf.Pow(currentPos.x - endPos.x, 2) > Mathf.Pow(currentPos.y - endPos.y, 2)) //gets closer in the axis that it is farthest in
                    {
                        if (currentPos.x < endPos.x)
                        {
                            currentPos.x += 1;
                        }
                        else if (currentPos.x > endPos.x)
                        {
                            currentPos.x -= 1;
                        }
                    }
                    else
                    {
                        if (currentPos.y < endPos.y)
                        {
                            currentPos.y += 1;
                        }
                        else if (currentPos.y > endPos.y)
                        {
                            currentPos.y -= 1;
                        }
                    }

                    //Fills that element accordingly
                    ManipTools.FillElementIfItExists(MapLayout, currentPos.x, currentPos.y, mapChar);
                    ManipTools.FillElementIfItExists(MapHeight, currentPos.x, currentPos.y, hght);


                    int counter = 1;
                    int innerInfiniteLoopPrevention = 0;
                    while (counter <= pathSize && innerInfiniteLoopPrevention < 3000) // fills an element and its surrounding elements of both the contents list and the height list with its new value
                    {
                        if (ManipTools.IsValidElement(currentPos.x, currentPos.y, mapSize))
                        {
                            
                                ManipTools.FillElementIfItExists(MapLayout, currentPos.x + counter, currentPos.y, mapChar);
                                ManipTools.FillElementIfItExists(MapHeight, currentPos.x + counter, currentPos.y, hght);
                                ManipTools.FillElementIfItExists(MapLayout, currentPos.x - counter, currentPos.y, mapChar);
                                ManipTools.FillElementIfItExists(MapHeight, currentPos.x - counter, currentPos.y, hght);
                                ManipTools.FillElementIfItExists(MapLayout, currentPos.x + counter, currentPos.y + counter, mapChar);
                                ManipTools.FillElementIfItExists(MapHeight, currentPos.x + counter, currentPos.y + counter, hght);
                                ManipTools.FillElementIfItExists(MapLayout, currentPos.x - counter, currentPos.y - counter, mapChar);
                                ManipTools.FillElementIfItExists(MapHeight, currentPos.x - counter, currentPos.y - counter, hght);
                                ManipTools.FillElementIfItExists(MapLayout, currentPos.x, currentPos.y + counter, mapChar);
                                ManipTools.FillElementIfItExists(MapHeight, currentPos.x, currentPos.y + counter, hght);
                                ManipTools.FillElementIfItExists(MapLayout, currentPos.x, currentPos.y - counter, mapChar);
                                ManipTools.FillElementIfItExists(MapHeight, currentPos.x, currentPos.y - counter, hght);
                                if (counter > 1)
                                {
                                    ManipTools.FillElementIfItExists(MapLayout, currentPos.x + counter, currentPos.y + (counter - 1), mapChar);
                                    ManipTools.FillElementIfItExists(MapHeight, currentPos.x + counter, currentPos.y + (counter - 1), hght);
                                    ManipTools.FillElementIfItExists(MapLayout, currentPos.x + counter, currentPos.y - (counter - 1), mapChar);
                                    ManipTools.FillElementIfItExists(MapHeight, currentPos.x + counter, currentPos.y - (counter - 1), hght);
                                    ManipTools.FillElementIfItExists(MapLayout, currentPos.x - counter, currentPos.y + (counter - 1), mapChar);
                                    ManipTools.FillElementIfItExists(MapHeight, currentPos.x - counter, currentPos.y + (counter - 1), hght);
                                    ManipTools.FillElementIfItExists(MapLayout, currentPos.x - counter, currentPos.y - (counter - 1), mapChar);
                                    ManipTools.FillElementIfItExists(MapHeight, currentPos.x - counter, currentPos.y - (counter - 1), hght);
                                }
                                counter++;
                        }
                        innerInfiniteLoopPrevention++;
                    }
                    infiniteLoopPrevention++;
                }
            }
        }
    }

    void AddCover() //Goes through every element of the map list and checks if it should be cover, and if it should be cover then it creates cover in that place
    {
        float max = 20f;
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (MapLayout[i, j] != '1')
                {
                    PlaceCover(i, j, max, max);
                }
            }
        }

    }

    bool PlaceCover(int x, int y, float maxSightLine, float minSightLine) // Checks if cover can or should be placed in a position
    {
        if (SpaceCanBeCover(x, y))
        {
            GetLongestLoS(x, y, maxSightLine, minSightLine);
        }
        return false;
    }

    bool SpaceCanBeCover(int x, int y) //Checks a co-ordinates surroundings to see that it will not be blocking a path if a piece of cover is placed there
    {
        if (MapLayout[x, y] != '1' && x > 0 && y > 0 && x < mapSize && y < mapSize)
        {

            int counterFree = 0;
            if (MapLayout[x + 1, y] == '1')
            {
                counterFree += 1;
            }
            if (MapLayout[x - 1, y] == '1')
            {
                counterFree += 1;
            }
            if (MapLayout[x, y + 1] == '1')
            {
                counterFree += 1;
            }
            if (MapLayout[x, y - 1] == '1')
            {
                counterFree += 1;
            }
            if (MapLayout[x + 1, y + 1] == '1')
            {
                counterFree += 1;
            }
            if (MapLayout[x + 1, y - 1] == '1')
            {
                counterFree += 1;
            }
            if (MapLayout[x - 1, y + 1] == '1')
            {
                counterFree += 1;
            }
            if (MapLayout[x - 1, y - 1] == '1')
            {
                counterFree += 1;
            }


            if (counterFree >= 2 && counterFree < 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    float GetLongestLoS(int x, int y, float maxSight, float minSight) //checks in 8 directions for the longest line of sight and creates a piece of cover if that line of sight is greater than the maximum line of sight
    {
        float[] LoSArray = new float[8];
        LoSArray[0] = LoSCalc(x, y, 1, 0);
        LoSArray[1] = LoSCalc(x, y, -1, 0);
        LoSArray[2] = LoSCalc(x, y, 0, 1);
        LoSArray[3] = LoSCalc(x, y, 0, -1);
        LoSArray[4] = LoSCalc(x, y, -1, -1);
        LoSArray[5] = LoSCalc(x, y, 1, 1);
        LoSArray[6] = LoSCalc(x, y, -1, 1);
        LoSArray[7] = LoSCalc(x, y, 1, -1);

        int longest = 0;
        for (int i = 1; i < 8; i++)
        {
            if (LoSArray[i] > LoSArray[longest])
            {
                longest = i;
            }
        }

        if (LoSArray[longest] > maxSight)
        {
            int xDir = 0;
            int yDir = 0;
            switch (longest)
            {
                case 0:
                    xDir = 1; yDir = 0; break;
                case 1:
                    xDir = -1; yDir = 0; break;
                case 2:
                    xDir = 0; yDir = 1; break;
                case 3:
                    xDir = 0; yDir = -1; break;
                case 4:
                    xDir = -1; yDir = -1; break;
                case 5:
                    xDir = 1; yDir = 1; break;
                case 6:
                    xDir = -1; yDir = 1; break;
                case 7:
                    xDir = 1; yDir = -1; break;
            }
            if(LoSCalc(x, y, -xDir, -yDir) > minSight)
            {
                ManipTools.FillElementIfItExists(MapLayout, x, y, 'C');
                return LoSArray[longest];
            }
        }
        return 0;
    }

    float LoSCalc(int x, int y, int xDir, int yDir)//returns the distance from any square on the map to the nearest wall or piece of cover in a given direction
    {
        int counter = 1;
        bool wallFound = false;
        while (wallFound == false && x + (counter * xDir) < mapSize && x + (counter * xDir) >= 0 && y + (counter * yDir) < mapSize && y + (counter * yDir) >= 0 && counter < 150)
        {
            if (MapLayout[x + (counter * xDir), y + (counter * yDir)] == '1' || MapLayout[x + (counter * xDir), y + (counter * yDir)] == 'C' || MapLayout[x + (counter * xDir), y + (counter * yDir)] == 'c' || MapLayout[x + (counter * xDir), y + (counter * yDir)] == 'H')
            {
                wallFound = true;
            }
            counter += 1;
        }

        if (wallFound)
        {
            if (xDir == 0 || yDir == 0)
            {
                return (float)counter;
            }
            else
            {
                return ((float)counter * 1.414f);
            }
        }
        else { return 0f; }

    }

    void Finalise() //Goes through the map and turns diagonal lines of '1's into '/' and '?', these are spawned as single diagonal rectangles instead of several squares. essentially makings slopes smooth
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (MapLayout[i,j] == '1')
                {
                    bool up, down, left, right, upRight, upLeft, downRight, downLeft;
                    downLeft = (MapLayout[i - 1,j - 1] == '1' || MapLayout[i - 1, j - 1] == '/' || MapLayout[i - 1, j - 1] == '?');
                    downRight = (MapLayout[i + 1, j - 1] == '1' || MapLayout[i + 1, j - 1] == '/' || MapLayout[i + 1, j - 1] == '?');
                    upLeft = (MapLayout[i - 1, j + 1] == '1' || MapLayout[i - 1, j + 1] == '/' || MapLayout[i - 1, j + 1] == '?');
                    upRight = (MapLayout[i + 1, j + 1] == '1' || MapLayout[i + 1, j + 1] == '/' || MapLayout[i + 1, j + 1] == '?');
                    down = (MapLayout[i, j - 1] == '1' || MapLayout[i, j - 1] == '/' || MapLayout[i, j - 1] == '?');
                    right = (MapLayout[i + 1, j] == '1' || MapLayout[i + 1, j] == '/' || MapLayout[i + 1, j] == '?');
                    left = (MapLayout[i - 1, j] == '1' || MapLayout[i - 1, j] == '/' || MapLayout[i - 1, j] == '?');
                    up = (MapLayout[i, j + 1] == '1' || MapLayout[i, j + 1] == '/' || MapLayout[i, j + 1] == '?');

                    if ((up && upRight && upLeft && left && downLeft && !down && !downRight && !right) || (down && downRight && downLeft && right && upRight && !left && !up && !upLeft))
                    {
                        MapLayout[i, j] = '?';
                    }
                    if ((up && upRight && upLeft && !left && !downLeft && !down && downRight && right) || (down && downRight && downLeft && !right && !upRight && left && !up && upLeft))
                    {
                        MapLayout[i, j] = '/';
                    }


                }
            }
        }
    }

    Vector2Int PointSizeGen(int maxArea, int minX, int randAddX, int minY, int randAddY) // Returns dimensions of a random rectangle based on minimum length and width and maximum area of the rectangle
    {
        int xSize = 1000, ySize = 1000;
        while (xSize * ySize > maxArea)
        {
            xSize = minX + Random.Range(0, randAddX - 1);
            ySize = minY + Random.Range(0, randAddY - 1);
        }
        Vector2Int sz = new Vector2Int(xSize, ySize);
        return sz;
    }
    
    MapTheme Theme(int randy) //Selects a random "Theme". Essentially changes the colour palette of the floor.
    {
        //enum class MapTheme { Sandy, Forest, SnowyFacility, Town, SnowTown, Resort, Random };
        MapTheme theme = MapTheme.Sandy;
        switch (randy)
        {
            case 0:
                theme = MapTheme.Sandy;
                break;
            case 1:
                theme = MapTheme.Forest;
                break;
            case 2:
                theme = MapTheme.SnowyFacility;
                break;
            case 3:
                theme = MapTheme.Town;
                break;
            case 4:
                theme = MapTheme.SnowTown;
                break;
            case 5:
                theme = MapTheme.Resort;
                break;
            case 6:
                theme = MapTheme.Random;
                break;
        }

        return theme;
    }

    int GetGround()
    {
        int grnd = 5;
        switch (mapTheme)
        {
            case MapTheme.Sandy:
                grnd = sandyGrounds[Random.Range(0,3)];
                break;
            case MapTheme.Forest:
                grnd = forestyGrounds[Random.Range(0, 3)];
                break;
            case MapTheme.SnowyFacility:
                grnd = snowyGrounds[Random.Range(0, 2)];
                break;
            case MapTheme.Town:
                grnd = townGrounds[Random.Range(0, 3)];
                break;
            case MapTheme.SnowTown:
                grnd = snowTownGrounds[Random.Range(0, 3)];
                break;
            case MapTheme.Resort:
                grnd = resortGrounds[Random.Range(0, 5)];
                break;
            case MapTheme.Random:
                grnd = randomGrounds[Random.Range(0, 6)];
                break;
        }

        return grnd;
    }

    void BalancePoint(PlantSite site) //This attempts to even the playing field when a gimmick or the cover layout makes a site too one sided.
    {
        bool isASite = (site.GetPosition().x < mapSize / 2);
        Vector2Int attDefPoints;
        if (isASite) { attDefPoints = aSite.GetAttackDefensePoints(); }
        else { attDefPoints = bSite.GetAttackDefensePoints(); }

        //Not really infinite prevention, just to stop the balance from getting to lost in a bunch of cover and gimmicks
        int infPrevention = 0;
        while (attDefPoints.x != attDefPoints.y && infPrevention < 5)
        {
            //When attackers have advantage, give boosts to defence
            if (attDefPoints.x > attDefPoints.y)
            {
                if (Random.value > 0.5f)
                {
                    //Adds 1 piece of cover to the site
                    site.AddCover(1, 1);
                    attDefPoints.y++;
                }
                else
                {
                    //Adds a gimmick to an attacker side entrance to site that makes it harder for attackers to push safely
                    if (site.GetExitStatus(3))
                    {
                        site.ExitGimmick(3, false);
                    }
                    else { site.ExitGimmick(4, false); }
                    attDefPoints.y++;
                }
            }
            //When defenders have advantage, give boosts to attackers
            else if (attDefPoints.y > attDefPoints.x)
            {
                int randBonus = Random.Range(0, 3);
                switch (randBonus)
                {
                    // Turn link points into rooms, this allows attackers to hide better if a defender is pushing, and peak site from more angles
                    case 0:
                        if (isASite)
                        {
                            if (AttackerASideLinks[0].GetBranches() != null) { AddPoint(AttackerASideLinks[0]); }
                            if (AttackerASideLinks[2].GetBranches() != null) { AddPoint(AttackerASideLinks[2]); }
                        }
                        else
                        {
                            if (AttackerBSideLinks[0].GetBranches() != null) { AddPoint(AttackerBSideLinks[0]); }
                            if (AttackerBSideLinks[2].GetBranches() != null) { AddPoint(AttackerBSideLinks[2]); }
                        }
                        attDefPoints.x += 1;

                        break;
                    //Adds another entrance onto the site
                    case 1:
                        if (isASite)
                        {
                            aSiteLinkConfig = "Both";
                            if (aSiteLinkConfig == "Left")
                            {
                                AttackerASideLinks[2].AddBranch(3, aSite);
                                int randConn = Random.Range(0, 4);
                                if (randConn != 3)
                                {
                                    AttackerASideLinks[2].AddBranch(10, AttackerASideLinks[1]);
                                    AttackerASideLinks[1].AddBranch(3, attackerSpawn);
                                }
                            }
                            else 
                            {
                                AttackerASideLinks[0].AddBranch(4, aSite);

                                AttackerASideLinks[0].AddBranch(10, AttackerASideLinks[1]);
                                AttackerASideLinks[1].AddBranch(10, AttackerASideLinks[2]);

                                int randConn = Random.Range(0, 3);
                                if (randConn == 0)
                                {
                                    AttackerASideLinks[1].AddBranch(10, AttackerASideLinks[4]);
                                }
                                else if (randConn == 1)
                                {
                                    AttackerASideLinks[1].AddBranch(3, attackerSpawn);
                                }
                            }
                        }
                        else
                        {
                            bSiteLinkConfig = "Both";
                            if (bSiteLinkConfig == "Right")
                            {
                                //Attach to upper left vs lower left
                                //if (Random.Range(0, 1) == 0) { AttackerBSideLinks[0].AddBranch( 2, bSite); }
                                //else {  }
                                AttackerBSideLinks[0].AddBranch(1, bSite);

                                AttackerBSideLinks[0].AddBranch(10, AttackerBSideLinks[1]);
                                AttackerBSideLinks[1].AddBranch(10, AttackerBSideLinks[2]);

                                int randConn = Random.Range(0, 3);
                                if (randConn == 0)
                                {
                                    AttackerBSideLinks[1].AddBranch(10, AttackerBSideLinks[4]);
                                }
                                else if (randConn == 1)
                                {
                                    AttackerBSideLinks[1].AddBranch(1, attackerSpawn);
                                }
                            }
                            else
                            {
                                AttackerBSideLinks[2].AddBranch(3, bSite);
                                int randConn = Random.Range(0, 4);
                                if (randConn != 3)
                                {
                                    AttackerBSideLinks[2].AddBranch(10, AttackerBSideLinks[1]);
                                    AttackerBSideLinks[1].AddBranch(1, attackerSpawn);
                                }
                            }
                        }
                        attDefPoints.x += 2;
                        break;
                    //Adds a gimmick to an attacker side entrance to site that makes it easier for attackers to push safely
                    case 2:
                        if (site.GetExitStatus(3))
                        {
                            site.ExitGimmick(3, true);
                        }
                        else
                        {
                            if (isASite)
                            {
                                site.ExitGimmick(4, true);
                            }
                            else
                            {
                                site.ExitGimmick(1, true);
                            }
                        }
                        attDefPoints.x++;
                        break;
                }
            }
            infPrevention++;
        }
        site.SetAttackDefensePoints(attDefPoints);

    }
    

}