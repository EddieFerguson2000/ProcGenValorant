using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidArea : KeyPoint
{
	SiteGimmicks gimmicks = new SiteGimmicks();

    public override void InitialiseArea()//Mid specific initialisation
    {
		attackerPoints = 0;
		defenderPoints = 0;
		SetExitsMid();
		ImplementGimmick();

		AddCover(4, 6);
    }

	void ImplementGimmick() //implements gimmick using the SiteGimmick
	{
		Maps maps = gimmicks.ImplementPointGimmick(contents, heightMap, exits, false, size, position, groundType.ToString().ToCharArray()[0], groundType2.ToString().ToCharArray()[0]);
		contents = maps.cont;
		heightMap = maps.height;
		exits = maps.exits;
		ShouldAddCover = maps.addMoreCover;
		attackerPoints += maps.attPoints;
		defenderPoints += maps.defPoints;
	}

	protected void SetExitsMid()//Mid has two extra exits, one on the east and one one the west
	{

		//srand(seed);
		Exit northExit = new Exit();
		northExit.exitMapChar = 'N';
		northExit.exitSize.x = 5 + Random.Range(0, 4);
		northExit.exitSize.y = 0;
		northExit.exitPositionRelative.x = (northExit.exitSize.x / 2) + Random.Range(0, (size.x / 2) - 1);
		northExit.exitPositionRelative.y = 0;
		//northExit.height = 0;

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
		eastExit.exitPositionRelative.y = eastExit.exitPosition.y - position.y + (size.y/2);
		//eastExit.height = 0;

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
		//northEastExit.height = 0;
		
		northEastExit.exitSize.x = 0;
		northEastExit.exitSize.y = 3 + Random.Range(0, 2);
		northEastExit.exitPosition.x = position.x + (size.x / 2);
		northEastExit.exitPosition.y = Random.Range(position.y - (size.y / 2), position.y);
		northEastExit.exitPositionRelative.x = size.x - 1;
		northEastExit.exitPositionRelative.y = northEastExit.exitPosition.y - position.y + (size.y / 2);
		//if (northEastExit.exitPositionRelative.y <= northEastExit.exitSize.y / 2)
		//{
		//	northEastExit.exitPositionRelative.y = northEastExit.exitSize.y / 2;
		//}
		//else if (northEastExit.exitPositionRelative.y >= size.y - (northEastExit.exitSize.y / 2))
		//{
		//	northEastExit.exitPositionRelative.y = size.y - (northEastExit.exitSize.y / 2);
		//}
		exits.Add(northEastExit);

		Exit southExit = new Exit();
		southExit.exitMapChar = 'S';
		southExit.exitSize.x = 5 + Random.Range(0, 4);
		southExit.exitSize.y = 0;
		southExit.exitPosition.x = position.x;
		southExit.exitPosition.y = position.y - (size.y / 2);
		southExit.exitPositionRelative.x = (southExit.exitSize.x / 2) + Random.Range(0, (size.x / 2) - 1);
		southExit.exitPositionRelative.y = size.y - 1;
		//southExit.height = 0;

		exits.Add(southExit);


		Exit westExit = new Exit();
		westExit.exitMapChar = 'W';
		westExit.exitSize.x = 0;
		westExit.exitSize.y = 5 + Random.Range(0, 4);
		westExit.exitPosition.x = position.x - (size.x / 2);
		westExit.exitPosition.y = Random.Range(position.y, position.y + (size.y / 2));
		westExit.exitPositionRelative.x = 0;
		westExit.exitPositionRelative.y = westExit.exitPosition.y - position.y + (size.y / 2);
		//westExit.height = 0;

		//if (westExit.exitPositionRelative.y <= westExit.exitSize.y / 2)
		//{
		//	westExit.exitPositionRelative.y = westExit.exitSize.y / 2;
		//}
		//else if (westExit.exitPositionRelative.y >= size.y - (westExit.exitSize.y / 2))
		//{
		//	westExit.exitPositionRelative.y = size.y - (westExit.exitSize.y / 2);
		//}
		exits.Add(westExit);
		
		Exit northWestExit = new Exit();
		northWestExit.exitMapChar = 'W';
		northWestExit.exitSize.x = 0;
		northWestExit.exitSize.y = 3 + Random.Range(0, 2);
		northWestExit.exitPosition.x = position.x - (size.x / 2);
		northWestExit.exitPosition.y = Random.Range(position.y - (size.y / 2), position.y);
		northWestExit.exitPositionRelative.x = 0;
		northWestExit.exitPositionRelative.y = northWestExit.exitPosition.y - position.y + (size.y / 2);
		//if (northWestExit.exitPositionRelative.y <= northWestExit.exitSize.y / 2)
		//{
		//	northWestExit.exitPositionRelative.y = northWestExit.exitSize.y / 2;
		//}
		//else if (northWestExit.exitPositionRelative.y >= size.y - (northWestExit.exitSize.y / 2))
		//{
		//	northWestExit.exitPositionRelative.y = size.y - (northWestExit.exitSize.y / 2);
		//}
		exits.Add(northWestExit);

	}

}
