using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    // wait for key press to start game
    void Update()
    {
        if (Input.anyKey || Input.GetMouseButtonDown(0))
            SceneManager.LoadScene("River");
    }
}
