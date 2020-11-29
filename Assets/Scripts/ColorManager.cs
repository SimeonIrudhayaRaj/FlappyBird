using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;
    public void Awake() {
        ColorManager.instance = this;
    }
    public Color getRandomColor() {
        Random rnd = new Random();
        float f = Random.Range(0f, 3f);

        switch ((int)f)
            {
                case 0:
                    return Color.red;
                case 1:
                    return Color.yellow;
                case 2:
                    return Color.green;
                case 3:
                    return Color.blue;
                case 4:
                    return Color.magenta;
                default:
                    return Color.white;
            }
    }
}
