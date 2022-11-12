using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image image;

    int currentSpriteIndex = 0;

    public void CycleIcons()
    {
        
        currentSpriteIndex++;
        currentSpriteIndex = currentSpriteIndex % sprites.Length;
        image.overrideSprite = sprites[currentSpriteIndex];
    }
}
