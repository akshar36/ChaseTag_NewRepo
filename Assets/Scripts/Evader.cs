using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Evader : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 8.0f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private ChaserAI chaserController;
    private TimerScript timerController;
    private int platformCount = 3;
    public GameObject floorprefab;
    private GameObject chaser;
    public Text GameText;
    public Text LedgeCount;
    public Text TimerTxt;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer chaserSpriteRenderer;
    public Sprite caughtSprite;
    public Sprite smilingSprite;
    private bool evaderMoved = false;

    void Start()
    {
        HideGameOverShowTimer();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject chaser = GameObject.Find("Chaser");
        GameObject timer = GameObject.Find("TimerTxt");
        chaserSpriteRenderer = chaser.GetComponent<SpriteRenderer>();
        chaserController = chaser.GetComponent<ChaserAI>();
        timerController = timer.GetComponent<TimerScript>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector2 moveDirection = new Vector2(moveInput, 0);
        if(rb.velocity.magnitude > 5f && !evaderMoved){
            Debug.Log("rb.velocity.magnitude "+ rb.velocity.magnitude);
            evaderMoved = true;
            StartRunning();
        }

        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        if(!isGrounded && Input.GetKeyDown(KeyCode.Space) && platformCount>0)
        {
            Instantiate(floorprefab, transform.position, Quaternion.identity);
            platformCount--;
            LedgeCount.text = "x " + platformCount;
            LedgeCount.gameObject.SetActive(true);
            isGrounded = true;
        }
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Chaser"))
        {
            ShowGameOverHideTimer();
        } else{
            Debug.Log("hit the ground");
            isGrounded = true;
        }

    }

    void ShowGameOverHideTimer()
    {
        GameText.text = "Game Over";
        GameText.gameObject.SetActive(true);
        TimerTxt.gameObject.SetActive(false);
        chaserSpriteRenderer.sprite = smilingSprite;
        spriteRenderer.sprite = caughtSprite;
        Time.timeScale = 0f;
    }

    void HideGameOverShowTimer()
    {
        GameText.gameObject.SetActive(false);
        TimerTxt.gameObject.SetActive(true);
    }

    public void StartRunning()
    {
        // Start the chaser's movement
        chaserController.StartChasing();
        timerController.StartTime();
    }
}
