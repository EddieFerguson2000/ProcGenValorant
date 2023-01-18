using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION

public class FireEffect : MonoBehaviour
{
    private float lerper;
    public float fadeTime;
    private Material mat;
    public GameObject lt;
    public float lightRange;
    private bool fired = false;
    private bool shrunk = false;
    private float randy;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (lerper < 1)
        {
            lerper += fadeTime * Time.deltaTime;

        }

        if (lerper >= 0.5f && lerper < 1)
        {
            if (shrunk == false && fired == true)
            {
                transform.Rotate(0f, 0f, Random.Range(0, 360));
                transform.localScale = new Vector3(0.3f + randy, 0.2f+ randy, 1f);
                shrunk = true;
            }
            
        }
        else if (lerper >= 1)
        {
            mat.color = (new Color(mat.color.r, mat.color.g, mat.color.b, 0f));
            fired = false;
        }
        lt.GetComponent<Light>().range = lightRange - (lightRange * lerper);


    }

    public void Fired()
    {
        lerper = 0;
        transform.Rotate(0f, 0f, Random.Range(0, 360));
        randy = Random.Range(0f, 0.3f);
        transform.localScale = new Vector3(0.7f + randy, 0.4f + randy, 1f);
        fired = true;
        shrunk = false;

        mat.color = (new Color(mat.color.r, mat.color.g, mat.color.b, 1f));
        lt.GetComponent<Light>().range = lightRange;

    }
}
