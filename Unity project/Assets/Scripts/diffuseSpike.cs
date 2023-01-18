using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class diffuseSpike : MonoBehaviour
{
    public GameObject timeIndicator;
    public GameObject rotator;
    public GameObject explosionPrefab;
    private GameObject deathClock;
    private GameObject clock;
    //private GameObject clock;
    public float timeTillDiffused;
    public Material indMat;
    public Slider slider;
    public bool halfway;

    public bool diffusing;
    public bool diffused;

    public float spinCap;
    public float spinMult;

    public float timeToExplode;
    public float timer;
    public bool exploded;

    private float deathClockTicker;
    private float deathClockSpeed = 0.5f;


    private Vector3 indicatorPos;

    // Start is called before the first frame update
    void Start()
    {
        clock = GameObject.Find("Clock");
        clock.gameObject.SetActive(false);
        deathClock = GameObject.Find("DeathClock");
        deathClock.GetComponent<Image>().enabled = true;
        indicatorPos = timeIndicator.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = timeTillDiffused / 7;

        CountDowns();

        if (diffusing == true)
        {
            slider.gameObject.SetActive(true);
            
        }
        else
        {
            if (diffused == true)
            {
                timeTillDiffused = 7f;
            }
            else if (halfway == true)
            {
                timeTillDiffused = 3.5f;
            }
            else
            {
                timeTillDiffused = 0f;
            }
        }

        rotator.transform.Rotate(new Vector3(0, timer / timeToExplode * spinMult, 0));
        timeIndicator.transform.position = new Vector3(indicatorPos.x, indicatorPos.y + (timeTillDiffused / 6.1f), indicatorPos.z);

        DeathClock();
        Explosion();


    }


    private void CountDowns()
    {
        if (diffusing == true)
        {
            if (diffused == false)
            {
                timeTillDiffused += Time.deltaTime;
            }
            else
            {
                timeTillDiffused = 7f;
            }
        }


        if (timeTillDiffused > 7f)
        {
            diffused = true;
        }
        else if (timeTillDiffused > 3.5f)
        {
            halfway = true;
        }

        if (timer < timeToExplode && diffused == false)
        {
            timer += Time.deltaTime;
        }
        else if (timer > timeToExplode)
        {
            timer = timeToExplode;
            exploded = true;
        }


        if (diffused == false)
        {
            if (spinMult < spinCap)
            {
                spinMult += Time.deltaTime / (timeToExplode / spinCap);
            }
            else
            {
                spinMult = spinCap;
            }
        }
        else
        {
            if (spinMult > 0)
            {
                spinMult -= Time.deltaTime;
            }
            else
            {
                spinMult = 0;
            }
        }
            


        if (deathClockTicker < 1)
        {
            deathClockTicker += Time.deltaTime * deathClockSpeed;
        }
        else
        {
            deathClockTicker = 0f;
        }

        if (deathClockSpeed < 3)
        {
            deathClockSpeed += Time.deltaTime / timeToExplode;
        }
    }

    private void Explosion()
    {
        if (exploded)
        {

            clock.gameObject.SetActive(true);
            clock.GetComponent<Clock>().StartClock(0);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            deathClock.GetComponent<Image>().enabled = false;
            Destroy(gameObject);
        }
    }

    private void DeathClock()
    {
        if (diffused == false)
        {
            indMat.color = new Color(1, 1 - (timer / timeToExplode) * (spinMult / spinCap), 1 - ((timer / timeToExplode) * (spinMult / spinCap)) / 2);
        }
        else
        {
            deathClock.GetComponent<Image>().enabled = false;
            indMat.color = new Color(0, 1, 1);

        }

        if (deathClockTicker > 0.8f)
        {
            deathClock.GetComponent<Image>().color = new Color(1, 0, 0);
        }
        else
        {
            deathClock.GetComponent<Image>().color = new Color(0.2f, 0, 0);
        }
    }

    public void beingDiffused(bool diffuseCheck)
    {
        diffusing = diffuseCheck;
    }

}
