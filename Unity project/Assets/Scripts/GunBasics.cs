using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//GAMEPLAY STUFF, NOT RELATED TO MAP GENERATION
public class GunBasics : MonoBehaviour
{
    public float fireRate;
    private float timeSinceShot;

    public float gunShakeMult;
    public float gunShakeTime;
    public float timeSinceGunShake = 1;

    public float innaccuracy;
    public float currentInnaccuracy;
    public float gunInnacuracy;
    public float innacuracyResetTimer;
    public float maxInnac;
    public float innacResetSpeed;

    private Vector3 gunShook;
    public Vector3 recoilAngle;
    public Vector3 originalAngle;

    public Vector3[] recoilAngles;
    public int currentBullet;
    private float bulletTimer;
    public float recoilTimeScale;

    public LayerMask decalIgnore;





    public GameObject fireEffect;
    public GameObject decalPrefab;
    public GameObject bloodPrefab;
    public Camera cam;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shooting();
        ShakeGun();
    }


    private void ShakeGun()
    {
        transform.localPosition = Vector3.Lerp(gunShook, new Vector3(0f, 0f, 0f), timeSinceGunShake);
    }

    private void Shooting()
    {
        if (timeSinceShot < 1 / fireRate)
        {
            timeSinceShot += Time.deltaTime;
        }

        recoilAngle = Vector3.Lerp(new Vector3(recoilAngle.x, recoilAngle.y, 0), new Vector3(0f, 0f, 0f), timeSinceShot);
        transform.localRotation = Quaternion.Euler(originalAngle + recoilAngle);


        if (bulletTimer > 0)
        {
            if (Input.GetMouseButton(0) == false)
            {
                if (bulletTimer > 4)
                {
                    bulletTimer = 4f;
                }
                bulletTimer -= Time.deltaTime * recoilTimeScale;
            }
        }
        else
        {
            bulletTimer = 0;
        }

        if (innacuracyResetTimer > 0)
        {
            innacuracyResetTimer -= innacResetSpeed * Time.deltaTime;
        }
        if (innacuracyResetTimer > maxInnac)
        {
            innacuracyResetTimer = maxInnac;
        }
        else if (innacuracyResetTimer < 0)
        {
            innacuracyResetTimer = 0;
        }

        if (timeSinceGunShake < 1)
        {
            timeSinceGunShake += fireRate * Time.deltaTime;
        }

        currentInnaccuracy = Mathf.Lerp(gunInnacuracy, innaccuracy, innacuracyResetTimer);

        if (Input.GetMouseButton(0))
        {
            if (timeSinceShot >= 1 / fireRate)
            {
                TakeShot();
            }
        }
    }

    private void TakeShot()
    {
        //fireEffect.transform.Rotate(new Vector3(0f, 0f, Random.Range(0, 360)));
        fireEffect.GetComponent<FireEffect>().Fired();
        gunShook = new Vector3(Random.Range(0.01f, 0.02f), Random.Range(0.01f, 0.02f), -0.1f);
        timeSinceShot = 0;
        timeSinceGunShake = 0;

        CalcBulletTraj();
        currentBullet = (int)bulletTimer;
        recoilAngle = recoilAngles[currentBullet];

        cam.GetComponent<mouse_lookat>().AddRecoil();
        innacuracyResetTimer += 1;
        if (currentBullet < recoilAngles.Length-1)
        {
            bulletTimer += 1f;
            

        }
        else
        {
            bulletTimer = 4f;
        }
    }

    private void CalcBulletTraj()
    {
        float ranInnacX = Random.Range(-currentInnaccuracy, currentInnaccuracy);
        float ranInnacY = Random.Range(-currentInnaccuracy, currentInnaccuracy);
        //Vector3 rayDir = new Vector3(cam.transform.forward.x + ranInnacX, cam.transform.forward.y + ranInnacY, cam.transform.forward.z);
        Vector3 rayDir = new Vector3(transform.forward.x + ranInnacX, transform.forward.y + ranInnacY, transform.forward.z);
        Ray ray = new Ray(cam.transform.position, rayDir);
        Vector3 endPos = new Vector3(cam.transform.position.x + ((transform.forward.x + ranInnacX) * 150), cam.transform.position.y + ((transform.forward.y + ranInnacY + recoilAngle.y) * 150), cam.transform.position.z + (cam.transform.forward.z * 150));
        Ray backRay = new Ray(endPos, -rayDir);
        
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, 150f, ~decalIgnore);
        RaycastHit[] backHits;
        backHits = Physics.RaycastAll(backRay, 150f, ~decalIgnore);

        RaycastHit[] currentRay = hits;
        bool hitBody = false;
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < currentRay.Length; i++)
            {
                string collTag = currentRay[i].collider.tag;
                if (collTag != "Attacker" && collTag != "Defender" && collTag != "PlayerEquipment" && collTag != "Arm" && collTag != "Leg" && collTag != "Torso"  && collTag != "Head")
                {
                    Vector3 decalPos = new Vector3(currentRay[i].point.x + (currentRay[i].normal.x * 0.01f), currentRay[i].point.y + (currentRay[i].normal.y * 0.01f), currentRay[i].point.z + (currentRay[i].normal.z * 0.01f));
                    GameObject decal = Instantiate(decalPrefab, decalPos, Quaternion.identity);
                    decal.transform.forward = currentRay[i].normal * -1f;
                    decal.transform.localEulerAngles = new Vector3(decal.transform.localEulerAngles.x, decal.transform.localEulerAngles.y, decal.transform.localEulerAngles.z + Random.Range(0, 360));
                }

                if (hitBody == false && (collTag == "Arm" || collTag == "Leg" || collTag == "Torso" || collTag == "Head"))
                {
                    Enemy person = currentRay[i].collider.transform.root.gameObject.GetComponent<Enemy>();
                    Vector3 bloodSize;
                    if (collTag == "Arm" || collTag == "Leg")
                    {
                        bloodSize = new Vector3(0.5f, 0.5f, 1);
                        person.takeDamage(27f);
                    }
                    else if (collTag == "Torso")
                    {
                        bloodSize = new Vector3(0.7f, 0.7f, 1);
                        person.takeDamage(40f);

                    }
                    else
                    {
                        bloodSize = new Vector3(1, 1, 1);
                        person.takeDamage(160f);

                    }
                    Vector3 bloodPos = new Vector3(currentRay[i].point.x + (currentRay[i].normal.x * 0.1f), currentRay[i].point.y + (currentRay[i].normal.y * 0.1f), currentRay[i].point.z + (currentRay[i].normal.z * 0.1f));
                    GameObject decal = Instantiate(bloodPrefab, bloodPos, Quaternion.identity);
                    decal.transform.forward = cam.transform.forward;
                    decal.transform.localEulerAngles = new Vector3(decal.transform.localEulerAngles.x, decal.transform.localEulerAngles.y, decal.transform.localEulerAngles.z + Random.Range(0, 360));
                    decal.transform.localScale = bloodSize;
                    hitBody = true;
                }

            }

            currentRay = backHits;
        }
        
    }

}
