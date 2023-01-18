using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This essentially takes a point (a plant site or the middle point) and gives it a gimmick for players to work around.
//the gimmicks can be pretty simple or small, such as a unique type of cover,
//or more complex, such as altering the height of areas to give high ground advantage


public enum PointGimmick { Long, Pillar, BindBox, Nest, Catwalk, BigBox, Pyramids, None, TwoPlantAreas, Heaven };

public class SiteGimmicks : MonoBehaviour
{
    ManipTools manipTools = new ManipTools();


    public PointGimmick SetPointGimmick(bool SiteOrMid) //True means Site. False means Mid
    {
		//The middle area and the plant sites don't share entirely the same gimmicks.
		//Mid cannot have two plant areas, or a covered raised area for defenders
		//While the plant sites cannot have a giant pillar in the middle of them

        // mid: Long, Pillar, BindBox, Nest, Catwalk, BigBox, Pyramids, None
        // site: BindBox, Nest, Catwalk, BigBox,  Pyramids, None, TwoPlantAreas, Heaven
        // Combined: 0:Long, 1: Pillar, 2:BindBox, 3:Nest, 4:Catwalk, 5:BigBox, 6:Pyramids, 7:None, 8:TwoPlantAreas, 9:Heaven
        int gim;

        if (SiteOrMid)
        {
            gim = Random.Range(2, 10);
        }
        else { gim = Random.Range(0, 8); }
        return (PointGimmick)gim;
    }

    public Maps ImplementPointGimmick(List<List<char>> contents, List<List<char>> heightMap, List<KeyPoint.Exit> exits, bool SiteOrMid, Vector2Int size, Vector2Int position, char ground1, char ground2)
    {
		//Init return value
        PointGimmick gimmick = SetPointGimmick(SiteOrMid);
		Maps maps = new Maps();
		maps.addMoreCover = true;
		maps.needsMorePlantSites = true;
		maps.attPoints = 0;
		maps.defPoints = 0;

		//long if statement for implementing each gimmick
		if (gimmick == PointGimmick.Long) //designed to immitate the middle area of split in Valorant. Essentially turns the middle area int an H on its side. Makes more of a choke point in the middle 
		{
			//Changing how long the narrow choke point will be based on where the entrances and exits lie
			int startPoint, endPoint;
			if (exits[1].exitPositionRelative.y > exits[4].exitPositionRelative.y)
			{
				exits[4].exitPositionRelative.y = exits[1].exitPositionRelative.y;
				exits[4].exitPosition.y = exits[1].exitPosition.y;
			}
			else
			{
				exits[1].exitPositionRelative.y = exits[4].exitPositionRelative.y;
				exits[1].exitPosition.y = exits[4].exitPosition.y;
			}
			startPoint = (int)exits[4].exitPositionRelative.y - 2;
			if (exits[2].exitPositionRelative.y < exits[5].exitPositionRelative.y)
			{
				exits[5].exitPositionRelative.y = exits[2].exitPositionRelative.y;
				exits[5].exitPosition.y = exits[2].exitPosition.y;
			}
			else
			{
				exits[2].exitPositionRelative.y = exits[5].exitPositionRelative.y;
				exits[2].exitPosition.y = exits[5].exitPosition.y;
			}
			endPoint = (int)exits[2].exitPositionRelative.y + 2;

			//Puts a big rectangle of cover into the centre of the point
			manipTools.CreateSquare(contents, new Vector2Int((int)size.x, startPoint - endPoint), size, new Vector2Int((int)size.x / 2, (int)size.y / 2), '1');
			//cuts a big square out of that cover so that people can pass through
			manipTools.CreateSquare(contents, new Vector2Int((int)size.x / 2, startPoint - endPoint), size, new Vector2Int((int)size.x / 2, (int)size.y / 2), ground1);

			//Repositioning the exits to ensure that they don't lead to a brick wall
			exits[5].exitPosition.y = position.y - (size.y / 2) + 2;
			exits[2].exitPosition.y = position.y - (size.y / 2) + 2;
			exits[1].exitPosition.y = position.y + (size.y / 2) - 2;
			exits[4].exitPosition.y = position.y + (size.y / 2) - 2;

			for (int i = 0; i < exits.Count; i++)
			{
				exits[i].exitPositionRelative.y = exits[i].exitPosition.y - position.y + (size.y / 2);
			}

		}
		else if (gimmick == PointGimmick.BindBox)
		{
			//Places a unique cover type somewhere near the middle of the point
			maps.defPoints += 1;
			int x, y;
			x = Random.Range(size.x / 4, (size.x / 4) + size.x / 2);
			y = Random.Range(size.y / 4, (size.y / 4) + size.y / 2);

			//removes cover within an area around the new unique cover
			manipTools.CreateSquare(contents, new Vector2Int(15, 15), size, new Vector2Int(x, y), ground1);

			contents[x][y] = 'n';

		}
		else if (gimmick == PointGimmick.Nest)//same as BindBox but the cover is elevated slightly
		{
			maps.defPoints += 1;

			int x, y;
			x = Random.Range(size.x / 4, (size.x / 4) + size.x / 2);
			y = Random.Range(size.y / 4, (size.y / 4) + size.y / 2);

			manipTools.CreateSquare(contents, new Vector2Int(15, 15), size, new Vector2Int(x, y), ground1);

			contents[x][y] = 'N';
		}
		else if (gimmick == PointGimmick.Pillar)//Puts a big circle of cover dead center
		{
			maps.defPoints += 1;
			manipTools.CreateCircle(contents, Random.Range(4, 7), size, new Vector2Int(size.x / 2, size.y / 2), '1');
			maps.addMoreCover = false;
		}
		else if (gimmick == PointGimmick.Catwalk)//Creates an elevated path that goes along the length or width of the point
		{
			int thickness = Random.Range(4, 7);
			int side = Random.Range(0, 3); //0 is north, 1 is east, 2 is west. cannot be on the south side (attacker side, would give to big of an advantage)
			int height = Random.Range(3, 8);
			switch (side) //All the cases are similar just swapping x/y or changing values in order to alter to position of the catwalk
			{
				case 0:
					exits[0].height = height;
					//Each case also ensures that each exit that leads to the catwalk is sufficiently raised
					if (exits[2].exitPositionRelative.y < thickness)
					{
						exits[2].height = height;
					}
					if (exits[5].exitPositionRelative.y < thickness)
					{
						exits[5].height = height;
					}
					for (int i = 0; i < size.x; i++)
					{
						for (int j = 0; j < thickness; j++)
						{
							heightMap[i][j] = height.ToString().ToCharArray()[0];
							contents[i][j] = ground2;
						}
					}
					int step0 = Random.Range(1, size.x - 1);
					for (int i = -1; i < 2; i++)
					{
						contents[step0 + i][thickness] = 'c';
					}
					break;
				case 1:
					exits[1].height = height;
					exits[2].height = height;
					if (exits[0].exitPositionRelative.x > size.x - thickness)
					{
						exits[0].height = height;
					}
					if (exits[3].exitPositionRelative.x > size.x - thickness)
					{
						exits[3].height = height;
					}
					for (int i = 0; i < size.y; i++)
					{
						for (int j = 0; j < thickness; j++)
						{
							heightMap[(int)size.x - 1 - j][i] = height.ToString().ToCharArray()[0];
							contents[(int)size.x - 1 - j][i] = ground2;

						}
					}
					int step1 = Random.Range(1, size.y - 1);
					for (int i = -1; i < 2; i++)
					{
						contents[thickness][step1 + i] = 'c';
					}
					break;
				case 2:
					exits[4].height = height;
					exits[5].height = height;
					if (exits[0].exitPositionRelative.x < thickness)
					{
						exits[0].height = height;
					}
					if (exits[3].exitPositionRelative.x < thickness)
					{
						exits[3].height = height;
					}
					for (int i = 0; i < size.y; i++)
					{
						for (int j = 0; j < thickness; j++)
						{
							heightMap[j][i] = height.ToString().ToCharArray()[0];
							contents[j][i] = ground2;

						}
					}
					int step2 = Random.Range(1, size.y - 1);
					for (int i = -1; i < 2; i++)
					{
						contents[thickness][step2 + i] = 'c';
					}
					break;
			}


		}
		else if (gimmick == PointGimmick.Pyramids) // puts one or two big square based pyramids in the middle of the point depending on the size of the point. They act as cover
		{
			maps.defPoints += 1;
			maps.addMoreCover = false;
			maps.needsMorePlantSites = false;
			if (size.x * size.y < 750)
            {
				if (SiteOrMid)
				{
					manipTools.CreateSquare(contents, new Vector2Int((size.x / 2), (size.y * 2) / 3), size, new Vector2Int((size.x / 2), (size.y / 2)), 'p');
				}
				int xRand = (size.x / 2);
				int yRand = (size.y / 2);
				contents[xRand][yRand] = '^';
			}
            else 
			{
				bool verticalOrHorizontal = false;

				verticalOrHorizontal = (Random.value < 0.5);

				if (verticalOrHorizontal)
				{
					if (SiteOrMid)
                    {
						manipTools.CreateSquare(contents, new Vector2Int((size.x / 2), (size.y * 2) / 3), size, new Vector2Int((size.x / 2), (size.y / 2)), 'p');
					}
					int xRand = (size.x / 2);
					int yRand = (size.y / 3);
					contents[xRand][yRand] = '^';

					yRand += (size.y / 3);
					contents[xRand][yRand] = '^';
				}
				else
				{
					if (SiteOrMid)
					{
						manipTools.CreateSquare(contents, new Vector2Int((size.x / 2) / 3, (size.y * 2)), size, new Vector2Int((size.x / 2), (size.y / 2)), 'p');
					}
					int xRand = (size.x / 3);
					int yRand = (size.y / 2);
					contents[xRand][yRand] = '^';

					xRand += (size.x / 3);
					contents[xRand][yRand] = '^';
				}
			}

			

		}
		else if (gimmick == PointGimmick.BigBox) // Puts a big box of cover in the centre of the point
		{
			maps.defPoints += 1;
			maps.needsMorePlantSites = false;

			if (SiteOrMid)
            {
				manipTools.CreateSquare(contents, new Vector2Int((size.x / 2) + 3, (size.y / 2) + 3), size, new Vector2Int(size.x / 2, size.y / 2), 'p');
			}
			manipTools.CreateSquare(contents, new Vector2Int(size.x / 2, size.y / 2), size, new Vector2Int((size.x / 2), (size.y / 2)), 'C');
		}
		else if (gimmick == PointGimmick.TwoPlantAreas) //Adds an additional area in the point in which attackers can plant the bomb
		{
			maps.attPoints += 2;
			Vector2Int pos = new Vector2Int();
			pos.x = position.x + (size.x / 4) + Random.Range(0, (size.x / 2) - 1);
			pos.y = position.y + (size.y / 4) + Random.Range(0, (size.y / 2) - 1);
			manipTools.CreateSquare(contents, new Vector2Int(size.x / Random.Range(3, 5), size.y / Random.Range(3, 5)), size, new Vector2Int(pos.x, pos.y), 'p');
		}
		else if (gimmick == PointGimmick.Heaven) //Similar to catwalk, it raises up an area of the site. This is a smaller platform compared to catwalk, and it has added cover 
		{
			maps.defPoints += 1;
			int height = Random.Range(5, 8);
			int width, length;
			width = Random.Range(6, size.x - 5);
			length = Random.Range(6, size.y / 3);
			exits[0].height = height;
			//First creates a rectangle of slightly raised ground as a step up to the actual heaven area
			manipTools.CreateSquare(heightMap, new Vector2Int(width + 2, length + 2), size, new Vector2Int(exits[0].exitPositionRelative.x, exits[0].exitPositionRelative.y), '2');
			//Then creates a rectangle of wall that can be used as cover
			manipTools.CreateSquare(contents, new Vector2Int(width + 1, length + 1), size, new Vector2Int(exits[0].exitPositionRelative.x, exits[0].exitPositionRelative.y), '1');
			//Then creates a small hole in the wall that faces onto site
			manipTools.CreateSquare(contents, new Vector2Int(width/2, length+1), size, new Vector2Int(exits[0].exitPositionRelative.x, exits[0].exitPositionRelative.y), ground2);
			//Then creates the actual heaven raised platform within the cover
			manipTools.CreateSquare(heightMap, new Vector2Int(width, length), size, new Vector2Int(exits[0].exitPositionRelative.x, exits[0].exitPositionRelative.y), height.ToString().ToCharArray()[0]);
			manipTools.CreateSquare(contents, new Vector2Int(width, length), size, new Vector2Int(exits[0].exitPositionRelative.x, exits[0].exitPositionRelative.y), ground2);
		}

		maps.cont = contents;
		maps.height = heightMap;
		maps.exits = exits;

		return maps;
    }



}
