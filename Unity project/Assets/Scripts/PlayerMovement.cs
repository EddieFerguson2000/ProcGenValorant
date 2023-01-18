using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    public GameObject spikePrefab;
    public GameObject plantPrefab;
    public Slider slider;

    private Rigidbody rb;
    public float moveSpeed;

    public float xMove, zMove;
    public Vector3 moveVec;
    public float fireRate;
    private float timeSinceShot;
    public float jumpforce;

    public bool carryingBomb = false;
    private bool canPickUpBomb = true;
    private float bombPickupTimer;

    public float bombPlantTimer;
    public bool canPlant;
    public bool canDiffuse;
    private bool planted;
    private bool plantingSpike;
    private bool diffusingSpike;

    private GameObject plantedSpike;





    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        foreach (Slider o in Object.FindObjectsOfType<Slider>())
        {
            slider = o;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (slider == null)
        {
            foreach (Slider o in Object.FindObjectsOfType<Slider>())
            {
                slider = o;
            }
        }
        Controls();
        Move();
        plantingBomb();
        diffuseBomb();
        lookForBomb();

        //transform.position = new Vector3()
    }

    private void Move()
    {
        if (plantingSpike == false && diffusingSpike == false)
        {
            moveVec = ((transform.forward * zMove) + (transform.right * xMove)) * moveSpeed;



            transform.position += moveVec * Time.deltaTime;

            if (bombPickupTimer > 0)
            {
                bombPickupTimer -= Time.deltaTime;
            }
            else
            {
                bombPickupTimer = 0;
                canPickUpBomb = true;
            }
        }
    }

    private void Controls()
    {
        int right, left, up, down;

        

        right = isKeyPressed(KeyCode.D);
        left = isKeyPressed(KeyCode.A);
        down = isKeyPressed(KeyCode.S);
        up = isKeyPressed(KeyCode.W);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0f, jumpforce, 0f));
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            tossBomb();
        }

        zMove = up - down;
        xMove = right - left;

        if (zMove != 0 && xMove != 0)
        {
            zMove *= 0.707f;
            xMove *= 0.707f;
        }



    }

    private int isKeyPressed(KeyCode code)
    {
        if (Input.GetKey(code))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }


    public void pickUpBomb(GameObject spike)
    {
        if (canPickUpBomb == true)
        {
            carryingBomb = true;
            Destroy(spike);
        }
        
    }

    private void tossBomb()
    {
        if (carryingBomb)
        {
            carryingBomb = false;
            canPickUpBomb = false;
            bombPickupTimer = 1f;
            GameObject spike = Instantiate(spikePrefab, transform.position + transform.forward, Quaternion.identity);
            spike.GetComponent<Rigidbody>().AddForce(transform.forward * 2);
        }

        
    }

    private void plantingBomb()
    {
        if (canPlant && carryingBomb && (planted == false))
        { 
            if (Input.GetKey(KeyCode.B))
            {
                plantingSpike = true;
                if (bombPlantTimer < 4)
                {
                    bombPlantTimer += Time.deltaTime;
                    slider.gameObject.SetActive(true);

                    slider.value = bombPlantTimer / 4;
                }
                else
                {
                    GameObject spikePlant = Instantiate(plantPrefab, transform.position + new Vector3(transform.forward.x * 2, -0.4f, transform.forward.z * 2), Quaternion.identity);
                    spikePlant.GetComponent<diffuseSpike>().slider = slider;
                    spikePlant.name = "PlantedSpike";
                    plantingSpike = false; 
                    slider.gameObject.SetActive(false);


                    planted = true;
                }
            }
            else
            {
                bombPlantTimer = 0f;
                plantingSpike = false;
                slider.gameObject.SetActive(false);

            }
        }
        else
        {
            //slider.gameObject.SetActive(false);
        }
    }

    private void diffuseBomb()
    {
        if (canDiffuse)
        {
            if (Input.GetKey(KeyCode.B))
            {
                diffusingSpike = true;
                if (plantedSpike != null)
                {
                    plantedSpike.GetComponent<diffuseSpike>().beingDiffused(true);
                }
            }
            else
            {
                diffusingSpike = false;
                if (plantedSpike != null)
                {
                    plantedSpike.GetComponent<diffuseSpike>().beingDiffused(false);
                }

            }
        }
        else
        {
            diffusingSpike = false;
            if (plantedSpike != null) { plantedSpike.GetComponent<diffuseSpike>().beingDiffused(false); }

        }
    }

    private void lookForBomb()
    {
        plantedSpike = GameObject.Find("PlantedSpike");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlantArea")
        {
            canPlant = true;
        }

        if (other.tag == "DiffuseArea")
        {
            canDiffuse = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlantArea")
        {
            canPlant = false;

        }

        if (other.tag == "DiffuseArea")
        {
            canDiffuse = false;

        }
    }


}
