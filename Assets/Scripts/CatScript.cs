using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages cat ball

public class CatScript : MonoBehaviour
{

    public float StartForce;
    private Vector2 StartPos;
    public float maxVelocityTarget = 8;
    private float maxVelocity=8;
    public bool CatAlive = true;

    public GameObject Shadow;
    public GameObject Splash;
    public GameObject CatSprite;
    public GameObject CatSpriteDying;

    public AudioClip sndSplash;

    public GameObject[] CatLives;

    void Start()
    {
        if (StaticClass.Lives==0)
            StaticClass.Lives = 9;
        StartPos = transform.position;
        RemoveDeadCats();
        
    }
    
    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(GetComponent<Rigidbody2D>().velocity, maxVelocityTarget);
        if (maxVelocityTarget > maxVelocity)
            maxVelocityTarget -= .5f;


        if (GetComponent<Rigidbody2D>().velocity.y < -8)
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, -8);

        Shadow.transform.position = new Vector2(transform.position.x, Shadow.transform.position.y);

        // bounce off camera bounds
        if (transform.position.x > 7.53f && GetComponent<Rigidbody2D>().velocity.x>0)
            GetComponent<Rigidbody2D>().velocity = new Vector2(-GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y);
        if (transform.position.x < -7.53f && GetComponent<Rigidbody2D>().velocity.x < 0)
            GetComponent<Rigidbody2D>().velocity = new Vector2(-GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y);

        // change to dying image if he gets low
        if (transform.position.y < Shadow.transform.position.y+2.2f && CatAlive && !GameEngineScript.instance.endLevel)
        {
            CatSpriteDying.SetActive(true);
            CatSprite.SetActive(false);
        }
        else
        {
            if (CatAlive)
            {
                if (!GameEngineScript.instance.endLevel)
                {
                    CatSpriteDying.SetActive(false);
                    CatSprite.SetActive(true);
                }
            }
        }

            // check for splash
            if (transform.position.y<Shadow.transform.position.y-.1f && CatAlive)
        {
            CatAlive = false;
            CatSprite.SetActive(false);
            CatSpriteDying.SetActive(false);
            Shadow.GetComponent<SpriteRenderer>().enabled=false;
            Splash.transform.localPosition = new Vector2(.5f, .2f);
            Splash.SetActive(true);
            Splash.GetComponent<Animator>().Play("Splash");
            GetComponent<AudioSource>().clip = sndSplash;
            GetComponent<AudioSource>().Play();
            StartCoroutine(LoseLife());
        }
    }

    IEnumerator LoseLife()
    {
        StaticClass.Lives--;
        Debug.Log("Static lives:" + StaticClass.Lives);
        //Debug.Log("kill:" + Lives + " "+CatLives[Lives].GetComponent<CatLifeScript>().AngryTail.name);

        StartCoroutine(CatLives[StaticClass.Lives].GetComponent<CatLifeScript>().Dead());

        yield return new WaitForSeconds(2);


        if (StaticClass.Lives > 0)
            Reset();
        else
            GameEngineScript.instance.GameOver();
        
    }

    private void RemoveDeadCats()
    {
        Debug.Log("lives:" + StaticClass.Lives);
        if (StaticClass.Lives < 9)
        {
            for (int c = StaticClass.Lives ;c<=8;c++)
            {
                Debug.Log("remove cat: " + c);
                CatLives[c].SetActive(false);
            }
        }
    }

    // reset catball after every lost life
    public void Reset()
    {
        transform.position = StartPos;
        CatAlive = true;
        CatSprite.SetActive(true);

        Shadow.GetComponent<SpriteRenderer>().enabled = true;
        Splash.SetActive(false);

        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * StartForce, ForceMode2D.Impulse);
    }

    public void StartVelocity()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * StartForce, ForceMode2D.Impulse);
    }

}
