using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Effects : MonoBehaviour
{
    public static Dictionary<string, Color> s_EffectColors = new Dictionary<string, Color>()
    {
        {"Red", new Color(255,0,0)},
        {"Green", new Color(0,255,0)},
        {"Blue", new Color(0,0,255) },
        {"Yellow", new Color(255,255,0) }
    };

    private static bool m_FlashIsPlaying;

    /// <summary>
    /// Screenshake effect
    /// </summary>
    /// <param name="duration">The duration of the shake</param>
    /// <param name="vibration">Vibration power of the shake</param>
    public static void s_Screenshake(float duration, int vibration)
    {
        Camera.main.transform.DOShakePosition(duration, 1, vibration, 30, false, true);
    }

    /// <summary>
    /// Flashes a image
    /// </summary>
    /// <param name="imageToFlash">The image that will be flashing</param>
    /// <param name="fadeInDuration">Duration of fade</param>
    /// <param name="loops">2 loops is 1 cycle</param>
    /// <param name="flashColor">Color of the flashing image</param>
    public static void s_ImageFlash(Image imageToFlash, float fadeDuration,int loops, Color flashColor)
    {
        if (!m_FlashIsPlaying) {
            m_FlashIsPlaying = true;
            imageToFlash.DOFade(1, fadeDuration).SetLoops(loops, LoopType.Yoyo).OnComplete(() => m_FlashIsPlaying = false);
        }        
    }

    /// <summary>
    /// Change the color of an image
    /// </summary>
    /// <param name="imageToChange">Image to change</param>
    /// <param name="targetColor">Color the image will change to</param>
    /// <param name="duration">The color fade duration</param>
    public static void ChangeImageColor(Image imageToChange, Color targetColor,float duration)
    {
        imageToChange.DOColor(targetColor, duration);
    }

    /// <summary>
    /// Change the color of a sprite
    /// </summary>
    /// <param name="spriteToChange">Sprite to change</param>
    /// <param name="targetColor">Color the image will change to</param>
    /// <param name="duration">The color fade duration</param>
    public static void ChangeSpriteColor(SpriteRenderer spriteToChange, Color targetColor, float duration)
    {
        spriteToChange.DOColor(targetColor, duration);
    }

    /// <summary>
    /// Yoyo's the color of a image between the target color and its original color
    /// </summary>
    /// <param name="imageToChange">Image to alter</param>
    /// <param name="targetColor">The color to change the sprite to</param>
    /// <param name="loops">Amount of times it should yoyo, 2 loops is 1 cycle</param>
    /// <param name="duration">duration of each loop</param>
    public static void YoyoImageColor(Image imageToChange, Color targetColor,int loops,float duration)
    {
        imageToChange.DOColor(targetColor,duration).SetLoops(loops, LoopType.Yoyo);
    }

    /// <summary>
    /// Yoyo's the color of a sprite between the target color and its original color
    /// </summary>
    /// <param name="spriteToChange">Sprite to alter</param>
    /// <param name="targetColor">The color to change the sprite to</param>
    /// <param name="loops">Amount of times it should yoyo, 2 loops is 1 cycle</param>
    /// <param name="duration">duration of each loop</param>
    public static void YoyoSpriteColor(SpriteRenderer spriteToChange, Color targetColor,int loops,float duration)
    {
        spriteToChange.DOColor(targetColor, duration).SetLoops(loops, LoopType.Yoyo);
    }
}