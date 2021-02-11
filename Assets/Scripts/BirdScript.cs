using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Bird logic
public class BirdScript : MonoBehaviour
{
    private float flyFrequency = 5; // how often bird appears - Higher number=longer wait between birds
    private float curTime;
    public float birdSpeed ;
    bool isFlying = false;

    public bool birdAlive;

    public GameObject featherExplode;
    public GameObject BirdSkele;

    public AudioClip BirdFly;
    public AudioClip BirdHit;

    void Start()
    {
        transform.position = new Vector3(10, Random.Range(-3.3f, -1.98f), transform.position.z);

        birdAlive = true;

        if (SceneManager.GetActiveScene().name == "River")
            birdSpeed = 7.5f;
        if (SceneManager.GetActiveScene().name == "Arctic")
            birdSpeed = 9.5f;
        if (SceneManager.GetActiveScene().name == "Lava")
            birdSpeed = 12;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameEngineScript.instance.ScrollSpeed > .8f && !isFlying && birdAlive)
            StartCoroutine(Fly());
    }

    // coroutine to initiate bird, animate bird, and move bird across screen 
    IEnumerator Fly()
    {
        isFlying = true;

        yield return new WaitForSeconds(flyFrequency+Random.Range(1,4));
        Debug.Log("Bird wait over");


        GetComponent<BoxCollider2D>().enabled = true;
        Debug.Log("enable birdskele");
        BirdSkele.SetActive(true);
        BirdSkele.transform.localPosition = new Vector3(0, 0, 0);

        GetComponent<AudioSource>().clip = BirdFly;

        GetComponent<AudioSource>().Play();
        transform.position = new Vector3(transform.position.x, Random.Range(-3.3f, -1.98f), transform.position.z);

        for (float x = 10; x > -10; x-= GameEngineScript.instance.ScrollSpeed/100*birdSpeed)
        {
            if (Time.timeScale != 0)
            {
                transform.position = new Vector3(x, transform.position.y, 0);
            }

            yield return null;
        }

        featherExplode.SetActive(false);

        Debug.Log("Call Fly coroutine");
        StartCoroutine(Fly());
    }

    // play feather explode animation
    public void Explode()
    {
        featherExplode.SetActive(true);
    }

    // Kill bird animation
    public IEnumerator KillBird()
    {
        birdAlive = false;

        // move up
        float startRotY = 0;
        float startEndY = -90;
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
        float seconds = .6f;
        float t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            BirdSkele.transform.localRotation = Quaternion.Euler(0, 0,Mathf.SmoothStep(startRotY, startEndY, t*2));
            transform.localPosition = Vector3.Lerp(startPos,endPos,t);
            yield return null;
        }

        // move down
        startRotY = -90;
        startEndY = 90;
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        endPos = new Vector3(transform.position.x, transform.position.y -12, transform.position.z);
        seconds = 1.6f;
        t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            BirdSkele.transform.localRotation = Quaternion.Euler(0, 0, Mathf.SmoothStep(startRotY, startEndY, t*4));
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        BirdSkele.SetActive(false);
        Debug.Log("Disable bird skele");
        BirdSkele.transform.localRotation = Quaternion.Euler(0, 0, 0);
        BirdSkele.transform.position = new Vector3(-0.116f, 0.776f, 0);
    }


}
