using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

// Player input, movement and collision
public class PlayerScript : MonoBehaviour
{
    // Input keys
    private bool pressLeft;
    private bool pressRight;
    private bool pressUp;
    private bool pressOverhand;
    private bool releaseOverhand;
    private bool pressUnderhand;
    private bool releaseUnderhand;

    private bool movementEnabled=true;
    public bool jumpEnabled = true;

    private int jumpForce = 18;
    private float JumpDelay = .20f;
    private float HitPower=0;
    public float HitMultiplier;
    public int jumpCount = 0;

    private float LeftBound = -1000;
    private float RightBound = 7.5f;
    private Vector3 playerScale;
    private int speed=5;

    public bool isWalking = false;
    public bool isIdling = true;
    public bool isJumping = false;
    public bool onGround = true;
    public bool UnderIsReleased=false;
    public bool OverIsReleased = true;
    private bool isHitting = false;

    public GameObject Spine;
    public GameObject NewBear;
    public Rigidbody2D playerRB;
    public GameObject Log;
    public GameObject Cat;
    public GameObject OverhandCol;
    public GameObject UnderhandCol;
    public GameObject Glow;
    public GameObject RacketMotion;

    public AudioClip sndBearLand;
    public GameObject objSwingSound;
    public AudioClip sndChargeIntro;
    public AudioClip sndChargeLoop;
    public AudioClip[] sndHitCat;

    private int rndSound;

    Vector3 startPos;

    void Start()
    {
        NewBear.GetComponent<Animator>().Play("idle");
        startPos = transform.position;
    }

    void Update()
    {
        if (Time.timeScale != 0)
        {
            playerScale = transform.localScale;
            if (movementEnabled)
                GetKeys();
            Movement();
            Animation();
            transform.localScale = playerScale;
            // update log
            if (Log != null)
            {
                if (Log.GetComponent<LogScript>().followPlayer)
                {
                    Log.transform.position = new Vector2(transform.position.x + .2f, Log.transform.position.y);
                }
            }
        }
    }

    private void GetKeys()
    {
        pressLeft = Input.GetKey(KeyCode.LeftArrow);//|| Input.GetKey("a");
        pressRight = Input.GetKey(KeyCode.RightArrow);//|| Input.GetKey("d");
        pressUp = Input.GetKeyDown(KeyCode.UpArrow);//|| Input.GetKeyDown("w"); 
        pressOverhand = Input.GetKeyDown("/");//|| Input.GetKeyDown("e");
        releaseOverhand = Input.GetKeyUp("/");//|| Input.GetKeyDown("e");
        pressUnderhand = Input.GetKeyDown(".");//|| Input.GetKeyDown("s"); 
        releaseUnderhand = Input.GetKeyUp(".");//|| Input.GetKeyUp("s"); 

        // exit game
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private void Movement()
    {
        if (pressLeft && movementEnabled && transform.position.x > LeftBound) // move left
        {
            playerScale.x = -1f;

            transform.position += Vector3.right * -speed * Time.deltaTime;

            if (transform.position.x < -7.56f)
                transform.position = new Vector2(-7.56f, transform.position.y);
            if (transform.position.x > 6.5f)
                transform.position = new Vector2(6.5f, transform.position.y);
        }

        if (pressRight && movementEnabled && transform.position.x < RightBound) // move right
        {
            playerScale.x = 1f;

            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        // Jump; checks if key was pressed and if player is on ground.
        if (pressUp && jumpEnabled && !isJumping && onGround)
        {
            //Debug.Log("isJumping:"+ isJumping+" OnGround:" + onGround);

            //StartCoroutine(JumpAnim());
            StartCoroutine(Jump());
        }

        if (pressOverhand && !isJumping && onGround && !isHitting) // charge swing
        {
            StartCoroutine(ChargeSwing());
        }

        if (releaseOverhand) // release overhand
        {
            OverIsReleased = true;
            Debug.Log("OH Released");
            RacketMotion.GetComponent<RacketMotionScript>().alpha = .9f;
            StartCoroutine(OverhandSwing());
            jumpCount++;

            objSwingSound.GetComponent<AudioSource>().Play();

            if (isJumping && jumpCount <= 1)
            {
                playerRB.AddForce(Vector2.up * 8, ForceMode2D.Impulse);
            }
        }

        if (releaseUnderhand) // release underhand
        { 
            UnderIsReleased = true;
            Debug.Log("Released under");
            RacketMotion.GetComponent<RacketMotionScript>().alpha = .9f;
            StartCoroutine(UnderhandSwing());
            jumpCount++;

            objSwingSound.GetComponent<AudioSource>().Play();

            if (isJumping && jumpCount<=1)
            {
                playerRB.AddForce(Vector2.up * 8, ForceMode2D.Impulse);
            }

        }


        // if you die, reappear at top
        if (transform.position.y<-7)
        {
            if (Log == null) // if no log then position over cliff
                transform.position = new Vector3(GameObject.Find("LevelCliff").transform.position.x, 3.22f, 0);
            else
                transform.position = new Vector3(Log.transform.position.x, 3.22f, 0);
        }


    }

    IEnumerator Jump()
    {
        playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(JumpDelay);
        //Debug.Log("Add Force");

        jumpEnabled = false;
        isJumping = true;
        isWalking = false;
        isIdling = false;
        onGround = false;

        Glow.SetActive(false); ;

        // delay to prevent double jumps
        yield return new WaitForSeconds(.7f);

        jumpEnabled = true;
        //movementEnabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Log" || collision.collider.tag=="Ground")
        {
            if (isJumping)
            {

                GetComponent<AudioSource>().clip = sndBearLand;
                GetComponent<AudioSource>().Play();

                // Lands on ground
                Debug.Log("Land on ground");
                StartCoroutine(Land());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Bird" )
        {
            col.GetComponent<BirdScript>().Explode();
            col.GetComponent<AudioSource>().clip = col.GetComponent<BirdScript>().BirdHit;
            col.GetComponent<AudioSource>().Play();
            col.GetComponent<BoxCollider2D>().enabled = false;

            if (transform.position.y > col.transform.position.y)
            {
                // stomp bird
                playerRB.AddForce(Vector2.up * (jumpForce)*2, ForceMode2D.Impulse);
                StartCoroutine(col.GetComponent<BirdScript>().KillBird());
                GetComponent<AudioSource>().clip = sndBearLand;
                GetComponent<AudioSource>().Play();
            }
            else
            { // bird hits player
                StartCoroutine(Fall());
            }

        }
    }

    IEnumerator Fall()
    {
        NewBear.GetComponent<Animator>().Play("falling");
        Debug.Log("falling");
        movementEnabled = false;
        yield return new WaitForSeconds(1.2f);
        movementEnabled = true;
        NewBear.GetComponent<Animator>().Play("idle");
        Debug.Log("idle after falling");
    }

    private void Animation()
    {
        if (pressLeft && onGround && transform.position.x > LeftBound && movementEnabled)
        {
            //Spine.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "run", true);
            NewBear.GetComponent<Animator>().Play("walk");
            //Debug.Log("walk");
            isWalking = true;
            isIdling = false;
        }

        if (pressRight && onGround && movementEnabled && transform.position.x < RightBound)
        {
            //Spine.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "run", true);
            NewBear.GetComponent<Animator>().Play("walk");
            //Debug.Log("walk");
            isWalking = true;
            isIdling = false;
        }

        if (!pressRight && !pressLeft) 
        {
            isWalking = false;
        }

        if (!isWalking && !isJumping && !isIdling && movementEnabled) // idle
        {
            //Spine.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "idle", true);
            NewBear.GetComponent<Animator>().Play("idle");
            //Debug.Log("idle2");
            isIdling = true;
        }
    }

    IEnumerator JumpAnim()
    {
        Debug.Log("Jump anim");
        yield return new WaitForSeconds(.20f);
    }

    IEnumerator Land()
    {
        Debug.Log("Land");
        onGround = true;
        isJumping = false;
        isIdling = false;
        jumpCount = 0;
        yield return null;
    }

    IEnumerator PlayChargeSound()
    {
        GetComponent<AudioSource>().clip = sndChargeIntro;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(sndChargeIntro.length);

        while (Input.GetKey("/"))
        {
            GetComponent<AudioSource>().clip = sndChargeLoop;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(sndChargeIntro.length);
        }
    }

    // coroutine to gradually charge swing and play charge animation and sound
    IEnumerator ChargeSwing()
    {
        Debug.Log("charge");
        StartCoroutine(PlayChargeSound());
        NewBear.GetComponent<Animator>().Play("charge-windup");
        Debug.Log("chargewindup");
        HitPower = 5.5f;
        yield return new WaitForSeconds(.34f);


        if (Input.GetKey("/") == false) // exit charging if you have released key
        {
            Glow.SetActive(false);
            NewBear.GetComponent<Animator>().Play("idle");
            Debug.Log("force exit charge ");
            yield break;
        }

        HitPower = 7;
        NewBear.GetComponent<Animator>().Play("charge01");

        Debug.Log("charge01");

        Glow.SetActive(true);

        while (Input.GetKey("/"))
        {
            HitPower += .34f;
            if (HitPower > 17f)
                HitPower = 17f;
            float alpha = (HitPower-7)/10;
            //Debug.Log("alpha:" + alpha);
            foreach(Transform gl in Glow.transform)
            {
                gl.GetComponent<SpriteRenderer>().color = new Color(gl.GetComponent<SpriteRenderer>().color.r, gl.GetComponent<SpriteRenderer>().color.g, gl.GetComponent<SpriteRenderer>().color.b, alpha);
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow))
            {
                Glow.SetActive(false);
                NewBear.GetComponent<Animator>().Play("idle");
                yield break;
            }

            yield return null;
        }
        Glow.SetActive(false);
        NewBear.GetComponent<Animator>().Play("idle");
        //Debug.Log("idle3");
    }


    IEnumerator OverhandSwing()
    {
        Glow.SetActive(false);
        if (HitPower == 0)
            HitPower = 3.5f;


        Debug.Log("overhand swing");
        pressOverhand = false;
        NewBear.GetComponent<Animator>().Play("overhand01",0);
        

        // when animation is over, play idle anim
        for (float iter=0;iter<0.15;iter+=Time.deltaTime)
            {
                // check for collision with cat
                if (OverhandCol.GetComponent<Collider2D>().IsTouching(Cat.GetComponent<Collider2D>()) && OverIsReleased)
                {
                    OverIsReleased = false;
                    Debug.Log("Hit: " + HitPower);

                Cat.GetComponent<CatScript>().maxVelocityTarget = 16;

                    StartCoroutine(Hit(Cat.GetComponent<Collider2D>()));
                    // check for forward or backward
                    if (Cat.transform.position.x > transform.position.x )
                    { // forward
                        Cat.GetComponent<Rigidbody2D>().AddForce((Vector2.up * HitPower*13.8f), ForceMode2D.Impulse);
                        Cat.GetComponent<Rigidbody2D>().AddForce(Vector2.right * (( HitPower * 5) + .4f), ForceMode2D.Impulse);
                    Cat.GetComponent<Rigidbody2D>().AddTorque(HitPower/ -32.5f, ForceMode2D.Impulse);
                        Debug.Log("for");
                    }
                    else
                    {
                    // backward
                    Cat.GetComponent<Rigidbody2D>().AddForce((Vector2.up * HitPower * 13.8f), ForceMode2D.Impulse);
                        Cat.GetComponent<Rigidbody2D>().AddForce(Vector2.left * ((HitPower * 5) + .4f), ForceMode2D.Impulse);
                        Cat.GetComponent<Rigidbody2D>().AddTorque(HitPower / 32.5f, ForceMode2D.Impulse);
                        Debug.Log("back");
                    }

                }
                yield return null;
            }
            NewBear.GetComponent<Animator>().Play("idle");
        OverIsReleased = false;
    }

        IEnumerator UnderhandSwing()
        {
        Glow.SetActive(false);
        Debug.Log("hit power:" + HitPower);
        if (HitPower == 0)
            HitPower = 4.5f;
            NewBear.GetComponent<Animator>().Play("underhand");

            // when animation is over, play idle anim
            for (float iter = 0; iter < 0.15; iter += Time.deltaTime)
            {
                // check for collision with cat
                if (UnderhandCol.GetComponent<Collider2D>().IsTouching(Cat.GetComponent<Collider2D>()) && UnderIsReleased)
                {
                    UnderIsReleased = false;
                    Debug.Log("Hit: " + HitPower);
                    StartCoroutine(Hit(Cat.GetComponent<Collider2D>()));

                    // check for forward or backward
                    if (Cat.transform.position.x>transform.position.x+1)
                { // forward
                    Cat.GetComponent<Rigidbody2D>().AddForce(Vector2.right * ((Cat.transform.position.x - transform.position.x - 1) / 4 + .4f), ForceMode2D.Impulse);
                    Cat.GetComponent<Rigidbody2D>().AddTorque(HitPower / 12.5f, ForceMode2D.Impulse);
                    Debug.Log("for");
                }
                else
                {
                    // backward
                    Cat.GetComponent<Rigidbody2D>().AddForce(Vector2.right * ((transform.position.x - Cat.transform.position.x + 1) / 4 + .4f), ForceMode2D.Impulse);
                    Cat.GetComponent<Rigidbody2D>().AddTorque(-HitPower / 12.5f, ForceMode2D.Impulse);
                    Debug.Log("back");
                }
            }
                yield return null;
            }
            NewBear.GetComponent<Animator>().Play("idle");
        UnderIsReleased = false;
        }   

    // player hits cat with overhand or underhand swing
    IEnumerator Hit(Collider2D other)
    {
        // play hit sound
        int snd = Random.Range(0, sndHitCat.Length);
        GetComponent<AudioSource>().clip = sndHitCat[snd];
        GetComponent<AudioSource>().Play();
        isHitting = true;
        Debug.Log("impulse");
        other.GetComponent<Rigidbody2D>().AddForce(Vector2.up * HitPower* HitMultiplier, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
        isHitting = false;
        HitPower = 0;

    }




}
