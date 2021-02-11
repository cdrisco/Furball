using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages a single cat life in the GUI canvas
public class CatLifeScript : MonoBehaviour
{
    public GameObject Head;
    public GameObject Tail;
    public GameObject BlinkHead;
    public GameObject AngryTail;
    public GameObject AngryHead;
    private float tailSpeed;
    public bool isDead = false;

    void Start()
    {
        tailSpeed = Random.Range(.4f, 1.5f);
        StartCoroutine(TailWag());
    }

    IEnumerator TailWag()
    {
        while (true)
        {
            for (int wags = 1; wags < Random.Range(1, 3); wags++)
            {
                for (float angle = -32; angle < 33; angle += tailSpeed)
                {
                    Tail.transform.rotation = Quaternion.Euler(0, 0, angle);
                    yield return null;
                }

                for (float angle = 32; angle > -33; angle -= tailSpeed)
                {
                    Tail.transform.rotation = Quaternion.Euler(0, 0, angle);
                    yield return null;
                }

                if (!isDead)
                    StartCoroutine(Blink());
                yield return null;
            }
        }

    }

    IEnumerator Blink()
    {
        //Debug.Log("Blink");
        BlinkHead.SetActive(true);
        Head.SetActive(false);
        yield return new WaitForSeconds(.1f);
        BlinkHead.SetActive(false);
        Head.SetActive(true);

    }

    public IEnumerator Dead()
    {
        isDead = true;
        Head.SetActive(false);
        Tail.SetActive(false);
        AngryHead.SetActive(true);
        AngryTail.SetActive(true);

        for (float alpha=1;alpha>0;alpha-=.009f)
        {
            AngryHead.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
            AngryTail.GetComponentInChildren<Image>().color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        AngryHead.SetActive(false);
        AngryTail.SetActive(false);
        Head.SetActive(false);
        Tail.SetActive(false);

    }


}
