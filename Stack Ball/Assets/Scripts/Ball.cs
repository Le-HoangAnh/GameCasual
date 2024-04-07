using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private float currentTime;

    private bool smash, invincible;

    private int currentBrokenStacks, totalStacks;

    public GameObject invincibleObj;
    public Image invincibleFill;
    public GameObject fireEffect;

    public enum BallState
    {
        Prepare,
        Playing,
        Died,
        Finish
    }

    [HideInInspector]
    public BallState ballState = BallState.Prepare;

    public AudioClip bounceOffClip, deadClip, winClip, destroyClip, iDestroyClip;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentBrokenStacks = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        totalStacks = FindObjectsOfType<StackController>().Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (ballState == BallState.Playing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                smash = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                smash = false;
            }

            if (invincible)
            {
                currentTime -= Time.deltaTime * 0.35f;
                if (!fireEffect.activeInHierarchy)
                {
                    fireEffect.SetActive(true);
                }
            }
            else
            {
                if (fireEffect.activeInHierarchy)
                {
                    fireEffect.SetActive(false);
                }

                if (smash)
                {
                    currentTime += Time.deltaTime * 0.8f;
                }
                else
                {
                    currentTime -= Time.deltaTime * 0.5f;
                }
            }

            if (currentTime >= 0.3f || invincibleFill.color == Color.red)
            {
                invincibleObj.SetActive(true);
            }
            else
            {
                invincibleObj.SetActive(false);
            }

            if (currentTime >= 1)
            {
                currentTime = 1;
                invincible = true;
                invincibleFill.color = Color.red;
            }
            else if (currentTime <= 0)
            {
                currentTime = 0;
                invincible = false;
                invincibleFill.color = Color.white;
            }

            if (invincibleObj.activeInHierarchy)
            {
                invincibleFill.fillAmount = currentTime / 1;
            }
        }

        if (ballState == BallState.Finish)
        {
            if (Input.GetMouseButtonDown(0))
                FindObjectOfType<LevelSpawner>().nextLevel();
        }
    }

    void FixedUpdate()
    {
        if (ballState == BallState.Playing)
        {
            if (Input.GetMouseButton(0))
            {
                smash = true;
                rb.velocity = new Vector3(0, -100 * Time.fixedDeltaTime * 7, 0);
            }
        }
        
        if (rb.velocity.y > 5)
        {
            rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z);
        }
    }

    public void increaseBrokenStacks()
    {
        currentBrokenStacks++;

        if (!invincible)
        {
            ScoreManager.instance.addScore(1);
            SoundManager.instance.playSoundFX(destroyClip, 0.5f);
        }
        else
        {
            ScoreManager.instance.addScore(2);
            SoundManager.instance.playSoundFX(iDestroyClip, 0.5f);
        }
    }

    void OnCollisionEnter(Collision target)
    {
        if (!smash)
        {
            rb.velocity = new Vector3 (0, 50 * Time.deltaTime * 5, 0);
            SoundManager.instance.playSoundFX(bounceOffClip, 0.5f);
        }
        else
        {
            if (invincible)
            {
                if (target.gameObject.tag == "enemy" || target.gameObject.tag == "plane")
                {
                    target.transform.parent.GetComponent<StackController>().ShatterAllParts();
                }
            }
            else
            {
                if (target.gameObject.tag == "enemy")
                {
                    target.transform.parent.GetComponent<StackController>().ShatterAllParts();
                }

                if (target.gameObject.tag == "plane")
                {
                    print("Over");
                    ScoreManager.instance.resetScore();
                    SoundManager.instance.playSoundFX(deadClip, 0.5f);
                }
            }
        }

        FindObjectOfType<GameUI>().levelSliderFill(currentBrokenStacks / (float)totalStacks);

        if (target.gameObject.tag == "Finish" && ballState == BallState.Playing)
        {
            ballState = BallState.Finish;
            SoundManager.instance.playSoundFX(winClip, 0.7f);
        }
    }

    void OnCollisionStay(Collision target)
    {
        if (!smash || target.gameObject.tag == "Finish")
        {
            rb.velocity = new Vector3(0, 50 * Time.deltaTime * 5, 0);
        }
    }
}
