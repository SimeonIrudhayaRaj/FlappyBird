using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerScored;
    public static event PlayerDelegate OnPlayerDied;
    
    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    public AudioSource die,tap,scoreUp; 

    Rigidbody2D rigidbody;
    Quaternion downRotation, forwardRotation;

    GameManager game;
    Renderer renderer;
    public static int nextColor = 0;
    string currentColor = "redBar";
    void Start()
    {
        renderer = GetComponent<Renderer>();
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        renderer.material.SetColor("_Color", Color.red);
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0,0,-90);
        forwardRotation = Quaternion.Euler(0,0,35);
        game = GameManager.instance;
        rigidbody.simulated = false;
    }

    void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }
    void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted() {
        renderer.material.SetColor("_Color", Color.red);
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }

    void OnGameOverConfirmed() {
        renderer.material.SetColor("_Color", Color.red);
        transform.localPosition =  startPos;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (game.isGameOver()) {
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            // Time.timeScale += 1;
            tap.Play();
            rigidbody.velocity = Vector3.zero;
            transform.rotation = forwardRotation;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);

    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == currentColor) {
            OnPlayerScored();
            scoreUp.Play();
            Random rnd = new Random();
            switch (nextColor)
            {
                case 0:
                    renderer.material.SetColor("_Color", Color.red);
                    currentColor = "redBar";
                    break;
                case 1:
                    renderer.material.SetColor("_Color", Color.green);
                    currentColor = "greenBar";
                    break;
                case 2:
                    renderer.material.SetColor("_Color", Color.yellow);
                    currentColor = "yellowBar";
                    break;
                case 3:
                    currentColor = "magentaBar";
                    renderer.material.SetColor("_Color", Color.magenta);
                    break;
                default:
                    currentColor = "redBar";
                    renderer.material.SetColor("_Color", Color.red);
                    break;
            }
        } else {
            die.Play();
            rigidbody.simulated = false;
            OnPlayerDied();
        } 
    }

}

