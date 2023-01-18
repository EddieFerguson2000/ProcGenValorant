using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class Clock : MonoBehaviour
{
    private Text text;

    public Vector3 setupTime;
    public Vector3 roundStartTime;
    public Vector3 roundOverTime;

    public GameObject roundManager;

    private float timeTillSecond;

    private int minutes;
    private int tenSecs;
    private int seconds;

    private bool timeUp;

    // Start is called before the first frame update
    void Start()
    {
        StartClock(1);
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (roundManager == null)
        {
            roundManager = GameObject.Find("RoundManager");
        }
        if (timeTillSecond < 1 )
        {
            if (timeUp == false)
            {
                timeTillSecond += Time.deltaTime;
            }
        }
        else
        {
            seconds--;
            timeTillSecond = timeTillSecond - 1f;
        }

        if (seconds < 0 && tenSecs == 0 && minutes == 0)
        {
            timeUp = true;
            if (roundManager)
            {
                roundManager.GetComponent<RoundManager>().ChangePhase();
            }

        }

        if (seconds < 0)
        {
            seconds = 9;
            tenSecs--;
        }
        if (tenSecs < 0)
        {
            tenSecs = 5;
            minutes--;
        }

        string clock = minutes + ":" + tenSecs + seconds;
        text.text = clock;

    }

    public void StartClock(int which)
    {
        timeUp = false;
        if (which == 0)
        {
            minutes = 0;
            tenSecs = 0;
            seconds = 0;
        }
        else if (which == 1)
        {
            minutes = (int)setupTime.x;
            tenSecs = (int)setupTime.y;
            seconds = (int)setupTime.z;
        }
        else if (which == 2)
        {
            minutes = (int)roundStartTime.x;
            tenSecs = (int)roundStartTime.y;
            seconds = (int)roundStartTime.z;
        }
        else if (which == 3)
        {
            minutes = (int)roundOverTime.x;
            tenSecs = (int)roundOverTime.y;
            seconds = (int)roundOverTime.z;
        }
    }
}
