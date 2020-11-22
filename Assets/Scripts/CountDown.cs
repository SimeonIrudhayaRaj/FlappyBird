﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountDown : MonoBehaviour
{
    public delegate void CountDownFinished();
    public static event CountDownFinished OnCountDownFinished;
    Text countdown;
    void OnEnable() {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
    }
    IEnumerator Countdown() {
        int count = 3;
        for (int i = 0; i< count; i++) {
            countdown.text = (count - i ).ToString();
            yield return new WaitForSeconds(1);
        }
        OnCountDownFinished();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}