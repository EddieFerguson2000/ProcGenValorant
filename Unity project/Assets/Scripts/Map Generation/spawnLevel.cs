using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


//This actually spawns all the stuff that is planned by MapGenerator
public class spawnLevel : MonoBehaviour
{

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject teleportPrefabA;
    public GameObject teleportPrefabB;
    public GameObject[] nestPrefab = new GameObject[4];
    public GameObject doorPrefab;
    public GameObject pyramidPrefab;
    public GameObject[] bindBoxPrefab = new GameObject[3];
    public GameObject coverPrefab;
    public GameObject shortCoverPrefab;
    public GameObject plantArea;
    public GameObject bombPrefab;

    public Material[] Mats = new Material[10];

    MapGenerator mapGen;
    FitnessTest fitTest;
    public int sizeOfMap;
    private GameObject[,] walls = new GameObject[200, 200];
    private GameObject[,] floors = new GameObject[200, 200];
    private char[,] mapLayout = new char[200, 200];
    private char[,] mapHeight = new char[200, 200];
    private GameObject[] enemies = new GameObject[20];
    int enemyCounter = 0;

    public int seed;


    // Start is called before the first frame update
    void Start()
    {
        if (seed > 0) //Preset seeds are on the number keys across the top of the keyboard
        {
            Random.seed = seed;
        }

        //Initialising a 2D array of gameobjects
        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < 200; j++)
            {
                walls[i, j] = null;
            }
        }

        //Keeps generating map layouts until they meet the fitness test requirements
        bool mapTest = false;
        while (!mapTest)
        {
            mapGen = new MapGenerator();
            fitTest = new FitnessTest();
            mapGen.Create(150);
            mapTest = fitTest.Test(mapGen);
        }
        mapLayout = mapGen.GetMapLayout();
        mapHeight = mapGen.GetHeightMap();

        //Once the requirements are met, the level is spawned
        PresentLevel();
        CreateGround();
        SpawnPlayer();
    }

    void PresentLevel() //Goes through the entire 2D array of characters and spawns/modifies different objects depending on what the character is
    {
        for (int i = 0; i < sizeOfMap; i++)
        {
            for (int j = 0; j < sizeOfMap; j++)
            {
                //Walls
                if (string.Compare(mapLayout[i,j].ToString(), "1") == 0 || string.Compare(mapLayout[i, j].ToString(), "H") == 0) 
                {
                    if (j > 0)
                    {
                        if (walls[i, j - 1] != null && !(string.Compare(mapLayout[i, j-1].ToString(), "/") == 0) && !(string.Compare(mapLayout[i, j-1].ToString(), "?") == 0))
                        {
                            walls[i, j] = walls[i, j - 1];
                            Vector3 scale = walls[i, j].transform.localScale;
                            scale.z += 1f;
                            walls[i, j].transform.localScale = scale;

                            Vector3 pos = walls[i, j].transform.position;
                            pos.z += 0.5f;
                            walls[i, j].transform.position = pos;
                        }
                        else
                        {
                            walls[i, j] = Instantiate(wallPrefab);
                            walls[i, j].transform.position = new Vector3(i, 0f, j);
                        }
                    }
                    else
                    {
                        walls[i, j] = Instantiate(wallPrefab);
                        walls[i, j].transform.position = new Vector3(i, 0f, j);
                    }

                }
                //ground of different colours
                else if (string.Compare(mapLayout[i, j].ToString(), "2") == 0 || string.Compare(mapLayout[i, j].ToString(), "3") == 0 || string.Compare(mapLayout[i, j].ToString(), "4") == 0 || string.Compare(mapLayout[i, j].ToString(), "5") == 0 || string.Compare(mapLayout[i, j].ToString(), "6") == 0 || string.Compare(mapLayout[i, j].ToString(), "7") == 0 || string.Compare(mapLayout[i, j].ToString(), "8") == 0)
                {
                    int current = (int)char.GetNumericValue(mapLayout[i, j]);

                    if (j > 0)
                    {
                        if (floors[i, j - 1] != null && (string.Compare(mapLayout[i, j - 1].ToString(), mapLayout[i, j].ToString()) == 0) && (string.Compare(mapHeight[i, j - 1].ToString(), mapHeight[i, j].ToString()) == 0))
                        {
                            floors[i, j] = floors[i, j - 1];
                            Vector3 scale = floors[i, j].transform.localScale;
                            scale.z += 1f;
                            floors[i, j].transform.localScale = scale;

                            Vector3 pos = floors[i, j].transform.position;
                            pos.z += 0.5f;
                            floors[i, j].transform.position = pos;
                        }
                        else
                        {
                            float currentHeight = (float)char.GetNumericValue(mapHeight[i, j]);
                            floors[i, j] = Instantiate(floorPrefab);
                            floors[i, j].transform.position = new Vector3(i, -0.09f, j);
                            floors[i, j].transform.localScale = new Vector3(1f, 0.2f + 0.7f * currentHeight, 1f);
                            floors[i, j].GetComponent<MeshRenderer>().material = Mats[current];
                        }
                    }
                    else
                    {
                        float currentHeight = (float)char.GetNumericValue(mapHeight[i, j]);

                        floors[i, j] = Instantiate(floorPrefab);
                        floors[i, j].transform.position = new Vector3(i, -0.09f, j);
                        floors[i, j].transform.localScale = new Vector3(1f, 0.2f + 1f * currentHeight, 1f);

                        floors[i, j].GetComponent<MeshRenderer>().material = Mats[current];

                    }

                }
                // spawns
                else if (string.Compare(mapLayout[i, j].ToString(), "s") == 0) 
                {
                    enemies[enemyCounter] = Instantiate(enemyPrefab);
                    enemies[enemyCounter].transform.position = new Vector3(i, 0.1f, j);
                    enemyCounter++;
                }
                // bomb
                else if (string.Compare(mapLayout[i, j].ToString(), "b") == 0) 
                {
                    GameObject bomb = Instantiate(bombPrefab);
                    bomb.transform.position = new Vector3(i, 0.5f, j);
                }
                //Slanted wall
                else if (string.Compare(mapLayout[i, j].ToString(), "/") == 0)
                {
                    if (string.Compare(mapLayout[i-1, j+1].ToString(), "/") == 0)
                    {
                        walls[i, j] = walls[i - 1, j + 1];
                        Vector3 scale = walls[i, j].transform.localScale;
                        scale.x += 1.41421f;
                        walls[i, j].transform.localScale = scale;

                        Vector3 pos = walls[i, j].transform.position;
                        pos.z -= 0.5f;
                        pos.x += 0.5f;
                        walls[i, j].transform.position = pos;
                    }
                    else
                    {
                        walls[i, j] = Instantiate(wallPrefab);
                        walls[i, j].transform.position = new Vector3(i, 0f, j);
                        Quaternion newRot = new Quaternion();
                        newRot.eulerAngles = new Vector3(0, 45, 0);
                        walls[i, j].transform.rotation = newRot;
                        walls[i, j].transform.localScale = new Vector3(2.5f, walls[i, j].transform.localScale.y, 1);
                    }
                }
                //Slanted wall (other direction)
                else if (string.Compare(mapLayout[i, j].ToString(), "?") == 0)
                {
                    if (string.Compare(mapLayout[i-1, j-1].ToString(), "?") == 0)
                    {
                        walls[i, j] = walls[i - 1, j - 1];
                        Vector3 scale = walls[i, j].transform.localScale;
                        scale.z += 1.41421f;
                        walls[i, j].transform.localScale = scale;

                        Vector3 pos = walls[i, j].transform.position;
                        pos.z += 0.5f;
                        pos.x += 0.5f;
                        walls[i, j].transform.position = pos;
                    }
                    else
                    {
                        walls[i, j] = Instantiate(wallPrefab);
                        walls[i, j].transform.position = new Vector3(i, 0f, j);
                        Quaternion newRot = new Quaternion();
                        newRot.eulerAngles = new Vector3(0, 45, 0);
                        walls[i, j].transform.rotation = newRot;
                        walls[i, j].transform.localScale = new Vector3(1, walls[i, j].transform.localScale.y, 2.5f);
                    }


                }
                // Pyramid (unique cover type)
                else if (string.Compare(mapLayout[i, j].ToString(), "^") == 0)
                {
                    GameObject pyramid = Instantiate(pyramidPrefab);
                    pyramid.transform.position = new Vector3(i, 0f, j);
                }
                // Plant area
                else if (string.Compare(mapLayout[i, j].ToString(), "p") == 0)
                {
                    GameObject pyramid = Instantiate(plantArea);
                    pyramid.transform.position = new Vector3(i, -0.09f, j);
                }
                // Nest (unique cover type)
                else if (string.Compare(mapLayout[i, j].ToString(), "N") == 0)
                {
                    GameObject nest = Instantiate(nestPrefab[Random.Range(0,4)]);
                    nest.transform.position = new Vector3(i, 0f, j);

                    if (Random.Range(0, 1) == 0)
                    {
                        nest.transform.eulerAngles = new Vector3(nest.transform.eulerAngles.x, 90f, nest.transform.eulerAngles.z);
                    }
                }
                // Bind Box (unique cover type)
                else if (string.Compare(mapLayout[i, j].ToString(), "n") == 0)
                {
                    GameObject bindBox;
                    bindBox = Instantiate(bindBoxPrefab[Random.Range(0, 3)]);
                    bindBox.transform.position = new Vector3(i, 0f, j);

                    if (Random.Range(0, 1) == 0)
                    {
                        bindBox.transform.eulerAngles = new Vector3(bindBox.transform.eulerAngles.x, 90f, bindBox.transform.eulerAngles.z);
                    }
                }
                // Tall cover
                else if (string.Compare(mapLayout[i, j].ToString(), "C") == 0) 
                {
                    GameObject cover = Instantiate(coverPrefab);
                    cover.transform.position = new Vector3(i, 5, j);
                }
                // short cover
                else if (string.Compare(mapLayout[i, j].ToString(), "c") == 0)
                {
                    GameObject cover = Instantiate(shortCoverPrefab);
                    cover.transform.position = new Vector3(i, 5, j);
                }
                //doors
                else if (string.Compare(mapLayout[i, j].ToString(), "D") == 0)
                {
                    GameObject door = Instantiate(doorPrefab);
                    door.transform.position = new Vector3(i, 0f, j);
                }
                // doors, but flipped 90 degrees
                else if (string.Compare(mapLayout[i, j].ToString(), "d") == 0) 
                {
                    GameObject door = Instantiate(doorPrefab);
                    door.transform.position = new Vector3(i, 0f, j);
                    Quaternion newRot = new Quaternion();
                    newRot.eulerAngles = new Vector3(0, 90, 0);
                    door.transform.rotation = newRot;
                }
                //More walls
                else
                {
                    walls[i, j] = null;
                }
            }
        }
    }

    void CreateGround() //Creates a plane beneath most of the map in case of any holes
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(sizeOfMap / 10, 1, sizeOfMap / 10);
        plane.GetComponent<Renderer>().material = Mats[0];
        plane.transform.position = new Vector3(sizeOfMap / 2, 0f, sizeOfMap / 2);
        plane.gameObject.layer = LayerMask.NameToLayer("MiniMapIgnore");
    }

    void SpawnPlayer() //Spawns the player controlled character in the place of any of the spawned characters
    {
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = enemies[0].transform.position;
        player.transform.position = new Vector3(player.transform.position.x, 2, player.transform.position.z);
        Destroy(enemies[0]);
    }




}


