using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceInv : MonoBehaviour
{
    // list of sprites
    public List<Sprite> faces = new List<Sprite>();
    public int index = -1;

    public void updateSlots(Dices dice)
    {
        for (int i = 0; i < dice.items.Length; i++)
        {
            // get the name of the face
            string faceName = dice.items[i];
            // get the sprite
            Sprite face = faces.Find(item => item.name == faceName);
            // get the child of the dice
            Transform slot = transform.GetChild(i);
            if (index == -1)
            {
                slot = slot.GetChild(0);
            }
            // get the image component
            Image slotImage = slot.GetComponent<Image>();
            // set the sprite
            slotImage.sprite = face;
        }
    }
}
