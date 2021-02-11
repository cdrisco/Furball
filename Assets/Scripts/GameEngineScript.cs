using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

// This script will control most of the game systems
// such as Play/Pause, End Level, Game Over
public class GameEngineScript : MonoBehaviour
{
    public static GameEngineScript instance;
    public GameObject txtReady;
    bool GameStarted = false;
    public GameObject Cat;
    public float ScrollSpeed=0;
    public float distanceTravelled;
    public float distanceToGoal;
    public float distanceRemaining;
    public GameObject txtDistance;
    public GameObject particle1;
    public GameObject particle2;
    public GameObject endCliff;

    public GameObject Instructions;

    public AudioClip musSaveCat;
    public AudioClip musKillCat;
    public AudioClip sndBoing;

    public AudioLowPassFilter ALPF;
    public Image imgPause;
    public Image imgPlay;
    public bool endLevel;
    public bool isDead = false;

    public GameObject playAgainButton;
    public GameObject Credits;
    public AudioClip musCredits;
    private GameObject Player;
    public GameObject PPGO;

    Vector2 CatStart;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        // set goal distance for level
        if (SceneManager.GetActiveScene().name == "River")
            distanceToGoal = 2000;
        if (SceneManager.GetActiveScene().name == "Arctic")
            distanceToGoal = 3000;
        if (SceneManager.GetActiveScene().name == "Lava")
            distanceToGoal = 4000;

        Player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(GrowGUI(txtReady, .6f));
        CatStart = Cat.transform.position;
        //Time.timeScale = .2f;
        imgPause = GameObject.Find("Pause").GetComponent<Image>();
        imgPlay = GameObject.Find("Play").GetComponent<Image>();
        imgPlay.enabled = false;

        // only display instructions if on first level
        if (SceneManager.GetActiveScene().name == "River")
            StartCoroutine(DisplayInstructions());

    }

    // Update is called once per frame
    void Update()
    {
        // wait for key (left, right, jump, or swing
        if (!GameStarted)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(".") || Input.GetKeyDown("/") || Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))
            {
                GameStarted = true;
                StartCoroutine(ShrinkGUI(txtReady,1.0f));
                // start cat gravity
                Cat.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                Cat.GetComponent<CatScript>().StartVelocity();
            }
            Cat.transform.position = CatStart+ new Vector2(Random.Range(-0.08f, 0.08f), Random.Range(-0.08f, 0.08f));
        }
        else
        {
            if (endCliff.activeInHierarchy == false && Player.GetComponent<PlayerScript>().Log  !=null && Player.GetComponent<PlayerScript>().Log.GetComponent<LogScript>().sinking==false)
            {
                ScrollSpeed += .005f;
            }
            else
            {
                ScrollSpeed -= .005f;
                if (ScrollSpeed<0)
                {
                    ScrollSpeed = 0;

                }
            }

            if (ScrollSpeed > 1)
            {
                ScrollSpeed = 1;
                if (SceneManager.GetActiveScene().name == "River")
                {
                    particle1.SetActive(true);
                    particle2.SetActive(true);
                }
            }

            if (isDead)
                ScrollSpeed = 0;

            if (Time.timeScale != 0)
            {
                distanceTravelled += ScrollSpeed;

                distanceRemaining = distanceToGoal - distanceTravelled;
                if (distanceRemaining > 0)
                    txtDistance.GetComponent<Text>().text = (distanceRemaining).ToString("F0");
                else
                    txtDistance.GetComponent<Text>().text = "";

                if (distanceRemaining < 95 && endCliff.activeInHierarchy == false)
                {
                    // enable end clif
                    endCliff.transform.position = new Vector2(Cat.transform.position.x + 14, endCliff.transform.position.y);
                    endCliff.SetActive(true);
                }

                if (endCliff.activeInHierarchy == true && ScrollSpeed == 0 && !endLevel && !isDead)
                {
                    endLevel = true;
                    // play music
                    GetComponent<AudioSource>().clip = musSaveCat;
                    GetComponent<AudioSource>().Play();

                    StartCoroutine(EndLevel());
                }
            }


        }
    }

    // End of level sequence
    IEnumerator EndLevel()
    {
        SpriteRenderer[] SR = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in SR)
        {
            s.sortingOrder += 50;
        }

        Vector3 CatStartPos = Cat.transform.position;
        Vector3 CatEndPos = endCliff.transform.position + new Vector3(0, 10, 0);

        // grow cat
        for (float scale = 1; scale < 3; scale += 0.02f)
        {
            Cat.transform.localScale = new Vector3(scale, scale, 1);

            // move to top of cliff
            Cat.transform.position = Vector3.Lerp(CatStartPos, CatEndPos,scale/3);

            yield return null;
        }

        yield return new WaitForSeconds(4);

        // end of level
        if (SceneManager.GetActiveScene().name=="River")
            SceneManager.LoadScene("Arctic");
        if (SceneManager.GetActiveScene().name == "Arctic")
            SceneManager.LoadScene("Lava");
        if (SceneManager.GetActiveScene().name == "Lava")
            ShowCredits();
    }

    // Grow GUI item effect
    IEnumerator GrowGUI(GameObject OBJ, float seconds)
    {
        Vector2 startScale = OBJ.transform.localScale;
        OBJ.transform.localScale = new Vector3(0, 0, 0);
        float t;
        float NewScale;
        t = 0.0f;
        NewScale = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            NewScale = t;
            OBJ.transform.localScale = startScale*new Vector2(NewScale*1.2f,NewScale*1.2f);
            //Debug.Log("Newscale: " + NewScale);
            yield return null;
        }
        t = 0;
        seconds = seconds / 2;
        while (t <= .2f)
        {
            t += Time.deltaTime / seconds;
            NewScale = 1.2f-t;
            OBJ.transform.localScale = startScale * new Vector2(NewScale , NewScale );
            //Debug.Log("Newscale: " + NewScale);
            yield return null;
        }
    }


    // Shrinnk GUI item effect
    IEnumerator ShrinkGUI(GameObject OBJ, float speed)
    {
        Vector2 startScale = OBJ.transform.localScale;
        OBJ.transform.localScale = new Vector3(0, 0, 0);
        float t;
        float NewScale;
        t = 0.0f;
        NewScale = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / speed;
            NewScale = t;
            OBJ.transform.localScale = startScale * new Vector2(1-NewScale, 1-NewScale);
            //Debug.Log("Newscale: " + NewScale);
            yield return null;
        }
        OBJ.SetActive(false);
    }

    public void PlayPause(GameObject img)
    {
        if (imgPause.GetComponent<Image>().IsActive())
        {
            // Pause
            Debug.Log("Paused");
            imgPause.enabled = false;
            imgPlay.enabled = true;
            Time.timeScale = 0;
            GetComponent<AudioSource>().volume = .5f;
            // enable low pass filter on music
            ALPF.enabled = true;
        }
        else
        {
            // Play
            Debug.Log("Play");
            imgPause.enabled = true;
            imgPlay.enabled = false;
            Time.timeScale = 1;
            // disable low pass filter on music
            ALPF.enabled = false;
            GetComponent<AudioSource>().volume = 1;
        }
    }

    public void GameOver()
    {
        StartCoroutine(FadeScreen());
    }

    IEnumerator FadeScreen()
    {
        isDead = true;

        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().clip = musKillCat;
        GetComponent<AudioSource>().Play();

        PostProcessVolume volume = PPGO.GetComponent<PostProcessVolume>();
        ColorGrading colorGradingLayer = null;
        volume.profile.TryGetSettings(out colorGradingLayer);
        for (int sat=0;sat>-100;sat--)
        {
            //Debug.Log("sat:" + sat);
            colorGradingLayer.saturation.value = sat;
            yield return null;
        }

        playAgainButton.SetActive(true);
    }

    // reset game by loading first scene
    public void PlayAgain()
    {
        SceneManager.LoadScene("River");
    }

    public void ShowCredits()
    {
        Credits.SetActive(true);

        Time.timeScale = 0;

        isDead = true;
        // slow down log
        endCliff.SetActive(true);
        endCliff.transform.position = new Vector3(-500, 0, 0);

        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().clip = musCredits;
        GetComponent<AudioSource>().Play();
        Time.timeScale = 0;

    }

    IEnumerator DisplayInstructions()
    {
        Instructions.SetActive(true);
        yield return new WaitForSeconds(4);
        Instructions.SetActive(false);
    }

}
