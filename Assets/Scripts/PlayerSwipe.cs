using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerSwipe : MonoBehaviour
{
    
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private Vector2 originalVelocity;

    public float forceMultiplier = 2f;
    public float maxFlickMagnitude = 16f;
    public float stoneBounceForceMagnitude = 2f;
    public float bouncyBounceForceMagnitude = 16f;
    public float lavaBounceForceMagnitude = 8f;
    public float stickyFlickMagnitude = 10f;
    public float secondFlickMagnitude = 10f;

    public GameObject slimeSprite;
    public Sprite idle;
    public Sprite jump;
    private HudScript hudScript;
    private Rigidbody2D rb;
    private Animator animator;

    float currentTimer = 0.0f;
    float untouchableTimer = 0.0f;
    float untouchableDuration = 3.0f;
    float timerDuration = 3.0f;

    string whichBlock = "Platform";
    bool wrongWay = false;
    bool alreadyJumped = true;
    bool jumpedAgain = false;
    bool untouchable = false;
    float angle;
    public int playersHealth = 3;

    float angleDegrees;
    float flickAngleDegrees;

    bool gameUnpaused = false;
    bool rbStored = false;
    public bool playerWon = false;

    [SerializeField] GameObject canvas;

    public GameObject tutorialBoards;

    Save save;
    int sceneIndex;
    int currentLevel;

    void Start()
    {
        save = GetComponent<Save>();
        hudScript = canvas.GetComponent<HudScript>();
        rb = GetComponent<Rigidbody2D>();
        animator = slimeSprite.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (hudScript.isPaused)
        {
            if (!rbStored)
            {
                originalVelocity = rb.velocity;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;
                rbStored = true;
            }
            gameUnpaused = true;
            return;
        }

        if (gameUnpaused)
        {
            rbStored = false;
            rb.velocity = originalVelocity;
            rb.gravityScale = 2.5f;
            gameUnpaused = false;
        }

        currentTimer += Time.deltaTime;
        untouchableTimer += Time.deltaTime;

        if(untouchableTimer > untouchableDuration)
        {
            untouchable = false;
        }

        if (currentTimer >= timerDuration && whichBlock != "Stick")
        {
            alreadyJumped = false;
            jumpedAgain = false;
            animator.SetBool("isJumped", false);
            currentTimer = 0.0f;
        }



        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 flickVector = (touchEndPos - touchStartPos) * forceMultiplier;
            Vector2 flickVectorC = Vector2.ClampMagnitude(flickVector, maxFlickMagnitude);

            float flickAngleRadians = Mathf.Atan2(flickVectorC.y, flickVectorC.x);
            flickAngleDegrees = flickAngleRadians * Mathf.Rad2Deg;
            if (flickAngleDegrees < 0) flickAngleDegrees = flickAngleDegrees + 360f;


            if (!alreadyJumped)
            {
                CheckIsJumpingWayTrue(flickVectorC);
            }

            if (alreadyJumped && !jumpedAgain)
            {
                rb.gravityScale = 2.5f;
                rb.velocity = new Vector2(0, 0);

                if(whichBlock != "Stick")
                {
                    flickVectorC = Vector2.ClampMagnitude(flickVector, secondFlickMagnitude);
                }
                
                rb.AddForce(flickVectorC, ForceMode2D.Impulse);
                animator.SetBool("isJumpedAgain", true);
                jumpedAgain = true;
            }

            if (!alreadyJumped && !wrongWay)
            {
                rb.gravityScale = 2.5f;

                if (whichBlock != "Sticky")
                {
                    rb.AddForce(flickVectorC, ForceMode2D.Impulse);
                }
                else if(whichBlock == "Sticky")
                {
                    flickVectorC = Vector2.ClampMagnitude(flickVector, stickyFlickMagnitude);
                    rb.AddForce(flickVectorC, ForceMode2D.Impulse);
                }
                animator.SetBool("isJumped", true);
                alreadyJumped = true;
            }


        }
    }

    void CheckIsJumpingWayTrue(Vector2 flickVectorC)
    {
        float upAngle = angleDegrees + 90f;
        float downAngle = angleDegrees - 90f;

        if (upAngle > 360) upAngle = upAngle - 360;
        if (downAngle <= 0) downAngle = downAngle + 360;

        if (upAngle <= 180)
        {
            if (upAngle < flickAngleDegrees && flickAngleDegrees < downAngle)
            {
                wrongWay = false;
            }
            else
            {
                wrongWay = true;
            }
        }
        else if(upAngle > 180f && upAngle < 270f)
        {
            if (upAngle < flickAngleDegrees)
            {
                
                wrongWay = false;
            }
            else if(downAngle - 90f < flickAngleDegrees && flickAngleDegrees < downAngle)
            {
                wrongWay = false;
            }
            else
            {
                wrongWay = true;
            }
        }
    }

    void FindWhichWayToTurn(Collision2D collision)
    {
        animator.SetBool("isJumpedAgain", false);
        animator.SetBool("isJumped", false);
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;

        float angleRadians = Mathf.Atan2(normal.y, normal.x);
        angleDegrees = angleRadians * Mathf.Rad2Deg;
        angleDegrees = angleDegrees + 180f;
        if (angleDegrees < 0) angleDegrees = angleDegrees + 360f;
        slimeSprite.transform.rotation = Quaternion.Euler(0, 0, angleDegrees + 90f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            whichBlock = "Platform";
            FindWhichWayToTurn(collision);
            alreadyJumped = false;
            jumpedAgain = false;
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
        }

        if (collision.gameObject.CompareTag("Stone"))
        {
            Debug.Log(angleDegrees);
            whichBlock = "Stone";
            ContactPoint2D bounceContact = collision.GetContact(0);
            Vector2 bounceDirection = bounceContact.normal;
            FindWhichWayToTurn(collision);
            slimeSprite.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (angleDegrees >= 195 && angleDegrees <= 345)
            {
                
                animator.SetBool("isJumpedAgain", false);
                animator.SetBool("isJumped", false);
                alreadyJumped = false;
                jumpedAgain = false;
                rb.velocity = new Vector2(0, 0);
                rb.gravityScale = 0;
            }
            else
            {
                Vector2 bounceForce = bounceDirection * stoneBounceForceMagnitude;
                rb.AddForce(bounceForce, ForceMode2D.Impulse);
            }

        }

        if (collision.gameObject.CompareTag("Bouncy"))
        {
            whichBlock = "Bouncy";
            ContactPoint2D bounceContact = collision.GetContact(0);
            Vector2 bounceDirection = bounceContact.normal;
            Vector2 bounceForce = bounceDirection * bouncyBounceForceMagnitude;
            rb.AddForce(bounceForce, ForceMode2D.Impulse);
            rb.gravityScale = 2.5f;

            animator.SetBool("isJumpedAgain", false);
            animator.SetBool("isJumped", true); 
            alreadyJumped = true;
            jumpedAgain = false;
            angle = 0; 
            slimeSprite.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (collision.gameObject.CompareTag("Sticky"))
        {
            whichBlock = "Sticky";
            FindWhichWayToTurn(collision);
            alreadyJumped = false;
            jumpedAgain = false;
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;

        }

        if (collision.gameObject.CompareTag("Lava"))
        {
            whichBlock = "Lava";
            ContactPoint2D bounceContact = collision.GetContact(0);
            Vector2 bounceDirection = bounceContact.normal;

            animator.SetTrigger("isDamaged");

            if (!untouchable)
            {
                playersHealth--;
                untouchable = true;
            }

            Vector2 bounceForce = bounceDirection * lavaBounceForceMagnitude;
            rb.AddForce(bounceForce, ForceMode2D.Impulse);

            animator.SetBool("isJumpedAgain", false);
            animator.SetBool("isJumped", true);
            alreadyJumped = true;
            jumpedAgain = false;
            angle = 0;
            slimeSprite.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (collision.gameObject.CompareTag("Kill"))
        {
            playersHealth = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Stick"))
        {
            whichBlock = "Stick";
            animator.SetBool("isJumpedAgain", false);
            animator.SetBool("isJumped", true);
            alreadyJumped = true;
            jumpedAgain = false;
            angle = 0;
            slimeSprite.transform.rotation = Quaternion.Euler(0, 0, angle);
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
        }

        if (collision.gameObject.CompareTag("Victory"))
        {
            currentLevel = save.LoadPlayerProgress();
            sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if(currentLevel < sceneIndex - 1)
            {
                save.SavePlayerProgress(sceneIndex - 1);
            }
            playerWon = true;
        }

        if (collision.gameObject.CompareTag("Heal"))
        {
            if(playersHealth < 3)
            {
                Destroy(collision.gameObject);
                playersHealth++;
            }
        }

        if (collision.gameObject.CompareTag("Sign"))
        {
            tutorialBoards.SetActive(!tutorialBoards.activeSelf);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sign"))
        {
            tutorialBoards.SetActive(!tutorialBoards.activeSelf);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        currentTimer = 0.0f;

    }
}
