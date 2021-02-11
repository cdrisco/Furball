using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CliffScript : MonoBehaviour
{
    private GameObject Player;
    public int cliffFreq;
    public bool cliffActive = false;
    public bool cliffStopping = false;
    public GameObject HighCollider;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (SceneManager.GetActiveScene().name != "River")
            StartCoroutine(NewCliff());
    }

    // Update is called once per frame
    void Update()
    {

        if (cliffStopping)
        {
            GameEngineScript.instance.ScrollSpeed =.3f;
            if (GameEngineScript.instance.ScrollSpeed<0)
            {
                GameEngineScript.instance.ScrollSpeed = 0;
                GetComponent<BackgroundScript>().enabled = false;
                GameObject.Find("Log2").GetComponent<BackgroundScript>().enabled = false;

            }
        }

        // if reaches far left off screen
        if (transform.position.x<-12 && cliffActive)
        {
            cliffActive = false;
            StartCoroutine(NewCliff());
        }
    }

    IEnumerator NewCliff()
    {
        int wait_time = Random.Range(cliffFreq, cliffFreq*2);
        Debug.Log("wait time:" + wait_time);
        yield return new WaitForSeconds(wait_time);

        if (GameEngineScript.instance.distanceRemaining > 200 && !cliffActive)
        {
            GameObject L1 = GameObject.Find("Log");
            GameObject L2 = GameObject.Find("Log2");

            Debug.Log("New Cliff");

            HighCollider.SetActive(false);
            cliffActive = true;
            GetComponent<BackgroundScript>().enabled = true;
            transform.position = new Vector3(13, -1.57f, 0);
            if (Player.GetComponent<PlayerScript>().Log != null)
            {
                if (Player.GetComponent<PlayerScript>().Log.name == "Log2") // swap logs
                {
                    Player.GetComponent<PlayerScript>().Log.GetComponent<LogScript>().followPlayer = false;
                    L1.GetComponent<LogScript>().followPlayer = true;
                    L2.transform.Find("Log_mask").gameObject.SetActive(true); // enable log mask
                    L1.transform.Find("Log_mask").gameObject.SetActive(true); // enable log mask
                                                                              // enable shadow
                    L1.transform.Find("Shadow").GetComponent<SpriteRenderer>().enabled = true;
                    // disable old log shadow
                    L2.transform.Find("Shadow").GetComponent<SpriteRenderer>().enabled = false;
                    L2.GetComponent<LogScript>().sinking = false;
                    L1.GetComponent<LogScript>().sinking = false;
                    L1.transform.position = L2.transform.position;
                    Player.GetComponent<PlayerScript>().Log = L1;
                }
            }
            L2.transform.position = new Vector3(15.32f, -3.09f, 0);
            L2.GetComponent<BackgroundScript>().enabled = true;

            
        }



    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Log" && collision.collider.transform.position.x < transform.position.x && GameEngineScript.instance.isDead != true) // log hits cliff
        {
            collision.collider.GetComponent<LogScript>().followPlayer = false;
            collision.collider.transform.Find("Log_mask").gameObject.SetActive(false); // disable log mask
            cliffStopping = true;
            if (Player.GetComponent<PlayerScript>().Log == collision.gameObject)
            {
                Player.GetComponent<PlayerScript>().Log.GetComponent<BackgroundScript>().enabled = true;
                Player.GetComponent<PlayerScript>().Log = null;
                // disable shadow on first log
                collision.collider.transform.Find("Shadow").GetComponent<SpriteRenderer>().enabled = false;


                Debug.Log("log=null");
            }
            // sink log 
            collision.gameObject.GetComponent<LogScript>().sinking = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Log" && collision.collider.transform.position.x<transform.position.x) // log hits cliff
        {
            collision.collider.transform.position = new Vector3(collision.collider.transform.position.x - .1f, collision.collider.transform.position.y, collision.collider.transform.position.z);       
        }
    }


}
