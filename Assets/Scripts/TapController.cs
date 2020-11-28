using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
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

    void Start()
    {
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
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }

    void OnGameOverConfirmed() {
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
        if (col.gameObject.tag == "scoreZone") {
            OnPlayerScored();
            scoreUp.Play();
        }
        if (col.gameObject.tag == "deadZone") {
            die.Play();
            rigidbody.simulated = false;
            OnPlayerDied();
        } 
    }

}
