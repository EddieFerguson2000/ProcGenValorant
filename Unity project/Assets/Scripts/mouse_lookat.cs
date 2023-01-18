using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class mouse_lookat : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    
    public Vector3 cameraReel;
    public float recoilGlideMult;

    private Vector3 recoilAngle;
    private float recoilLerp;
    private float timeSinceGunShake = 1;


    float xRotation = 0f;
    //float yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xRotation = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        
        

        recoilAngle = Vector3.Lerp(new Vector3(recoilAngle.x, recoilAngle.y, 0), new Vector3(0f, 0f, 0f), timeSinceGunShake);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.localPosition = Vector3.Lerp(cameraReel, new Vector3(0, 0, 0), recoilLerp);
        Vector3 baseRot = (Vector3.up * mouseX);
        //Vector3 recoilRot = new Vector3(baseRot.x, baseRot.y + recoilAngle.y, baseRot.z);
        //playerBody.Rotate(Vector3.Lerp(recoilRot, baseRot, timeSinceGunShake));
        playerBody.Rotate(baseRot);

        if (timeSinceGunShake < 1)
        {
            timeSinceGunShake += Time.deltaTime/5;
        }
        else
        {
            timeSinceGunShake = 1;
        }

        if (recoilLerp < 1)
        {
            recoilLerp += Time.deltaTime * recoilGlideMult;
        }
        else
        {
            recoilLerp = 1;
        }
    }

    public void AddRecoil()
    {
        //recoilAngle = newRecoilAngle;
        //recoilAngle = recoilAng;
        timeSinceGunShake = 0;
        recoilLerp = 0;
    }
}