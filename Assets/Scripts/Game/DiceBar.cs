using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBar : MonoBehaviour
{
    public Dice dice;
    public GameObject barFill;

    void Update()
    {
        float timer = dice.timer;
        float cooldown = dice.cooldown;
        if (timer < cooldown) {
            barFill.transform.localScale = new Vector3(timer / cooldown, 1, 1);
            barFill.GetComponent<Renderer>().material.color = new Color(71f / 255f, 158f / 255f, 241f / 255f);
        }
        else {
            barFill.transform.localScale = new Vector3(1, 1, 1);
            barFill.GetComponent<Renderer>().material.color = new Color(45f / 255f, 222f / 255f, 63f / 255f);
        }
    }
}
