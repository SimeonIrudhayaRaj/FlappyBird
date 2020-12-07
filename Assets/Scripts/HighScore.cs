﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]

public class HighScore : MonoBehaviour
{
    Text highScore;
    void OnEnable()
    {
        highScore = GetComponent<Text>();
        highScore.text = "HighScore: " + PlayerPrefs.GetInt("ColorHiScore").ToString(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
