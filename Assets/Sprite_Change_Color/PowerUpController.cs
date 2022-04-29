using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Check collissions for 
 *  1. PowerUp tag => collect/destroy
 *  2. ColorChanger tag => update the color using its SpriteColorChanger script settings * 
 */

public class PowerUpController : MonoBehaviour
{
    public int powerUpsCollected = 0;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.tag);

        // Check the tag on the collider
        if (collision.tag == "PowerUp")
        {
            powerUpsCollected++;
            Destroy(collision.gameObject);
            Debug.Log("PowerUp 1) collected and 2) Destroyed");
        }
        else if (collision.tag == "ColorChanger")
        {
            SpriteColorChanger spriteColorChanger = collision.GetComponent<SpriteColorChanger>();
            if (powerUpsCollected >= spriteColorChanger.powerUpsRequired)
            {
                spriteColorChanger.ChangeColor();
                Debug.Log("ColorChanger 1) new color set");
            }

        }
    }


}
