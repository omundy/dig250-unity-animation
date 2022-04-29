using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 *  Change color of material on trigger
 *  1. Attach to all sprites that will be changed
 *  2. Add a 2D collider (set to trigger) to each colorchange sprite
 *  3. Set the new color only
 *  4. Add tags: PowerUp and ColorChanger
 */

public class SpriteColorChanger : MonoBehaviour
{
    [Header("Set")]

    public SpriteRenderer spriteRenderer;
    public Color originalColor;
    public Color newColor;

    [Tooltip("Number of power ups required to change this sprite")]
    public int powerUpsRequired = 1;

    private void Awake()
    {
        // store original color
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void ChangeColor()
    {
        // change the color on the sprite
        spriteRenderer.color = newColor;
    }


}
