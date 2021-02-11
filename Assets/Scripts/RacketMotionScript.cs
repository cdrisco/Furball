using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketMotionScript : MonoBehaviour
{
    public float alpha;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        alpha -= .2f;
        if (alpha < 0)
            alpha = 0;
    }

}
