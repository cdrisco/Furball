using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JitterScript : MonoBehaviour
{
    private Vector2 startPos;
    public float jitter;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition + new Vector3(-.08f, -.05f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = startPos + new Vector2(Random.Range(-jitter, jitter), Random.Range(-jitter, jitter));
    }

}
