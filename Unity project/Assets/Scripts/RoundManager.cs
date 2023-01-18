using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class RoundManager : MonoBehaviour
{
    enum Phase { BuyPhase, RoundStart, RoundOver };
    Phase phase;


    public GameObject clock;

    // Start is called before the first frame update
    void Start()
    {
        phase = Phase.BuyPhase;
        if (clock != null)
        {
            clock.GetComponent<Clock>().StartClock(1);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (clock == null)
        {
            clock = GameObject.Find("Clock");
        }
    }

    public void ChangePhase()
    {
        if (clock)
        {
            if (phase == Phase.BuyPhase)
            {
                clock.gameObject.SetActive(true);
                phase = Phase.RoundStart;
                clock.GetComponent<Clock>().StartClock(2);

            }
            else if (phase == Phase.RoundStart)
            {
                clock.gameObject.SetActive(true);
                phase = Phase.RoundOver;
                clock.GetComponent<Clock>().StartClock(3);
            }
            else if (phase == Phase.RoundOver)
            {
                clock.gameObject.SetActive(true);
                RestartRound();
                phase = Phase.BuyPhase;
                clock.GetComponent<Clock>().StartClock(1);
            }
        }
       
    }

    private void RestartRound()
    {

    }
}
