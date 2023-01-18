using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class DoorAngles : MonoBehaviour
{
    public Transform leftHinge, rightHinge, leftTip, rightTip;
    public float minAng;


    // Start is called before the first frame update
    void Start()
    {
        RefreshAngles();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            RefreshAngles();
        }
    }

    public void RefreshAngles()
    {
        float leftAng, rightAng, distBetween;
        leftAng = Random.Range(70f, 90f);
        rightAng = Random.Range(70f, 90f);


        rightHinge.rotation = Quaternion.Euler(0, rightAng, 0);
        leftHinge.rotation = Quaternion.Euler(0, leftAng, 0);

        distBetween = Vector3.Magnitude(rightTip.position - leftTip.position);
        while (distBetween < minAng)
        {
            leftAng = Random.Range(-15f, 15f);
            leftHinge.rotation = Quaternion.Euler(0, leftAng, 0);
            distBetween = Vector3.Magnitude(rightTip.position - leftTip.position);

        }
    }
}
