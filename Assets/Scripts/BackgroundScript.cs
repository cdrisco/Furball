using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages the paralax scrolling for background
public class BackgroundScript : MonoBehaviour
{
    public float scrollSpeed;
    public float leftBounds;
    public float start;
    public bool repeat = true;


    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0) // if not paused
        { 
            transform.position = new Vector2(transform.position.x - scrollSpeed * GameEngineScript.instance.ScrollSpeed*Time.deltaTime*60, transform.position.y);
            if (transform.position.x <= leftBounds && repeat)
            {
                transform.position += new Vector3(start * 2, 0, 0);
            }
        }
    }
}
