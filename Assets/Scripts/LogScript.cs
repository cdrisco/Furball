using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogScript : MonoBehaviour
{
    private int dir = 1;
    public float bobSpeed;
    public float StartY;
    public float EndY;
    public bool sinking = false;
    public bool followPlayer=true; // follow code is in PlayerScript
    private GameObject Player;

    void Start()
    {
        StartY = transform.position.y;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (dir == 1)
            transform.position = new Vector2(transform.position.x, transform.position.y - bobSpeed);
        if (dir == -1)
            transform.position = new Vector2(transform.position.x, transform.position.y + bobSpeed);

        if (transform.position.y < EndY)
            dir = -1;
        if (transform.position.y > StartY)
            dir = 1;
        //Debug.Log("Dir:" + dir);
        if (sinking)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!followPlayer)
        {
            if (collision.collider.tag == "Player") // Player is on log
            {
                if (!sinking)
                {
                    Debug.Log("followPlayer:" +name);
                    followPlayer = true;
                    if (GetComponent<BackgroundScript>() != null)
                        GetComponent<BackgroundScript>().enabled = false;
                    Player.GetComponent<PlayerScript>().Log = gameObject;

                    transform.Find("Log_mask").gameObject.SetActive(true); // enable log mask

                    // enable log shadow
                    transform.Find("Shadow").GetComponent<SpriteRenderer>().enabled = true;

                    GameObject.Find("LevelCliff").GetComponent<BackgroundScript>().enabled = true;
                    GameObject.Find("LevelCliff").GetComponent<CliffScript>().cliffStopping = false;
                    GameObject.Find("LevelCliff").GetComponent<CliffScript>().HighCollider.SetActive(true);

                }
            }
        }
    }


}
