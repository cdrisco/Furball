using UnityEngine;
using System.Collections;

// Shadow script
// creates a shadow for the cat, and for the player shadow which is a child of log


	public class BlobShadow : MonoBehaviour 
{
	public GameObject shadowedObject;
    public float scale = 1;
    private float minFade = .4f; // lowest alpha for shadow fade
    private float maxScale = 2.5f; 

    private void Update()
    {
        float Dist = Vector2.Distance(transform.position, shadowedObject.transform.position) + 4;
        float alpha = (1 - ((Dist - 4) / 2.71f));
        if (alpha < minFade)
            alpha = minFade;
        //Debug.Log(Vector2.Distance(transform.position, shadowedObject.transform.position) + " alpha:" + alpha);
        float s = Dist * scale;
        if (s > maxScale)
            s = maxScale;
        transform.localScale = new Vector2(s, s);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
    }
}
