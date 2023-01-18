using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class DecalFade : MonoBehaviour
{
    private float fadeTimer;
    public float timeToFade;
    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        fadeTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float alph = Mathf.Lerp(1f, 0f, fadeTimer);
        mat.color = (new Color(mat.color.r, mat.color.g, mat.color.b, alph));

        if (fadeTimer < 1)
        {
            fadeTimer += Time.deltaTime * timeToFade;
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
