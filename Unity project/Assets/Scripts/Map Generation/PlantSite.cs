using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSite : KeyPoint
{

	SiteGimmicks gimmicks = new SiteGimmicks();


	bool NeedsPlantArea = true;

    public override void InitialiseArea() //initialisation unique to the plant sites
    {
		SetExitsSite();

		attackerPoints = 0;
		defenderPoints = 0;
		ShouldAddCover = true;
		ImplementGimmick();
		AddCover(2, 4);

		//Creates area for attackers to plant inside of
		Vector2Int pos = new Vector2Int();
        pos.x = position.x + (size.x / 4) + Random.Range(0, (size.x / 2)-1);
        pos.y = position.y + (size.y / 4) + Random.Range(0, (size.y / 2)-1);
		if (NeedsPlantArea) 
        {
			manipTools.CreateSquare(contents, new Vector2Int(size.x / Random.Range(3, 5), size.y / Random.Range(3, 5)), size, pos, 'p');
		}
	}


	public void ImplementGimmick() //implements gimmick using the SiteGimmick
	{
		Maps maps = gimmicks.ImplementPointGimmick(contents, heightMap, exits, true, size, position, groundType.ToString().ToCharArray()[0], groundType2.ToString().ToCharArray()[0]);
		contents = maps.cont;
		heightMap = maps.height;
		exits = maps.exits;
		ShouldAddCover = maps.addMoreCover;
		NeedsPlantArea = maps.needsMorePlantSites;
		attackerPoints += maps.attPoints;
		defenderPoints += maps.defPoints;
	}

	protected void SetExitsSite()//plant sites need two extra exits, one to the east and one to the west
	{
		//srand(seed);
		Exit northExit = new Exit();
		northExit.exitMapChar = 'N';
		northExit.exitSize.x = 5 + Random.Range(0, 4);
		northExit.exitSize.y = 0;
		northExit.exitPositionRelative.x = (northExit.exitSize.x / 2) + Random.Range(0, (size.x / 2) - 1);
		northExit.exitPositionRelative.y = 0;

		northExit.exitPosition.x = position.x;
		northExit.exitPosition.y = position.y + (size.y / 2);
		exits.Add(northExit);


		Exit eastExit = new Exit();
		eastExit.exitMapChar = 'E';
		eastExit.exitSize.x = 0;
		eastExit.exitSize.y = 3 + Random.Range(0, 2);
		eastExit.exitPosition.x = position.x + (size.x / 2);
		eastExit.exitPosition.y = Random.Range(position.y, position.y + (size.y / 2));
		eastExit.exitPositionRelative.x = size.x - 1;
		eastExit.exitPositionRelative.y = eastExit.exitPosition.y - position.y + (size.y / 2);

		if (eastExit.exitPositionRelative.y <= eastExit.exitSize.y / 2)
		{
			eastExit.exitPositionRelative.y = eastExit.exitSize.y / 2;
		}
		else if (eastExit.exitPositionRelative.y >= size.y - (eastExit.exitSize.y / 2))
		{
			eastExit.exitPositionRelative.y = size.y - (eastExit.exitSize.y / 2);
		}
		exits.Add(eastExit);

		Exit northEastExit = new Exit();
		northEastExit.exitMapChar = 'E';

		northEastExit.exitSize.x = 0;
		northEastExit.exitSize.y = 3 + Random.Range(0, 2);
		northEastExit.exitPosition.x = position.x + (size.x / 2);
		northEastExit.exitPosition.y = Random.Range(position.y - (size.y / 2), position.y);
		northEastExit.exitPositionRelative.x = size.x - 1;
		northEastExit.exitPositionRelative.y = northEastExit.exitPosition.y - position.y + (size.y / 2);
		exits.Add(northEastExit);

		Exit southExit = new Exit();
		southExit.exitMapChar = 'S';
		southExit.exitSize.x = 5 + Random.Range(0, 4);
		southExit.exitSize.y = 0;
		southExit.exitPosition.x = position.x;
		southExit.exitPosition.y = position.y - (size.y / 2);
		southExit.exitPositionRelative.x = (southExit.exitSize.x / 2) + Random.Range(0, (size.x / 2) - 1);
		southExit.exitPositionRelative.y = size.y - 1;
		exits.Add(southExit);


		Exit westExit = new Exit();
		westExit.exitMapChar = 'W';
		westExit.exitSize.x = 0;
		westExit.exitSize.y = 5 + Random.Range(0, 4);
		westExit.exitPosition.x = position.x - (size.x / 2);
		westExit.exitPosition.y = Random.Range(position.y, position.y + (size.y / 2));
		westExit.exitPositionRelative.x = 0;
		westExit.exitPositionRelative.y = westExit.exitPosition.y - position.y + (size.y / 2);
		exits.Add(westExit);

		Exit northWestExit = new Exit();
		northWestExit.exitMapChar = 'W';
		northWestExit.exitSize.x = 0;
		northWestExit.exitSize.y = 3 + Random.Range(0, 2);
		northWestExit.exitPosition.x = position.x - (size.x / 2);
		northWestExit.exitPosition.y = Random.Range(position.y - (size.y / 2), position.y);
		northWestExit.exitPositionRelative.x = 0;
		northWestExit.exitPositionRelative.y = northWestExit.exitPosition.y - position.y + (size.y / 2);
		exits.Add(northWestExit);

	}


}
