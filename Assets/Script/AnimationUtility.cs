using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public static class AnimationUtility 
{
    public static async Task FadeIn(VisualElement element, float duration = 1f)
    {
        if (element.style.display == DisplayStyle.None)
        {
            element.style.display = DisplayStyle.Flex;
        }
        var initialOpacity = element.style.opacity.value;
        var initialTime = Time.time;
        var position = (Time.time - initialTime) / duration;
        while (position<1)
        {
            element.style.opacity = Mathf.Lerp(initialOpacity, 1f, position);
            await DelayAsync(.03f);
            position = (Time.time - initialTime) / duration;
        }
    }
    public static async Task FadeOut(VisualElement element, float duration = 1f)
    {
        var initialOpacity = element.style.opacity.value;
        var initialTime = Time.time;
        var position = (Time.time - initialTime) / duration;
        while (position < 1)
        {
            element.style.opacity = Mathf.Lerp(initialOpacity, 0f, position);
            await DelayAsync(.03f);
            position = (Time.time - initialTime) / duration;
        }
        if (element.style.display == DisplayStyle.Flex)
        {
            element.style.display = DisplayStyle.None;
        }
    }
    public static async Task FadeIn(Material material, float duration = 1f)
    {
        var initialColor = material.color;
        var initialTime = Time.time;
        var position = (Time.time - initialTime) / duration;
        while (position < 1)
        {
            material.color = new Color(initialColor.r, initialColor.g, initialColor.b,
                Mathf.Lerp(initialColor.a, 1f, position)
                );
            await DelayAsync(.03f);
            position = (Time.time - initialTime) / duration;
        }
        material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1);
    }
    public static async Task FadeOut(Material material, float duration = 1f)
    {
        var initialColor = material.color;
        var initialTime = Time.time;
        var position = (Time.time - initialTime) / duration;
        while (position < 1)
        {
            material.color = new Color(initialColor.r,initialColor.g,initialColor.b,
                Mathf.Lerp(initialColor.a, 0f, position)
                ) ;
            await DelayAsync(.03f);
            position = (Time.time - initialTime) / duration;
        }
        material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

    }
    public static async Task FadeIn(TMP_FontAsset font, float duration = 1f)
    {
        var material = font.material;
        var initialFaceColor = material.GetColor("_FaceColor");
        var initialOutlineColor = material.GetColor("_OutlineColor");
        var initialTime = Time.time;
        var position = (Time.time - initialTime) / duration;
        while (position < 1)
        {

            material.SetColor("_FaceColor",
                new Color(initialFaceColor.r, initialFaceColor.g, initialFaceColor.b,
                Mathf.Lerp(initialFaceColor.a, 1f, position)
                ));
            material.SetColor("_OutlineColor",
                new Color(initialOutlineColor.r, initialOutlineColor.g, initialOutlineColor.b,
                Mathf.Lerp(initialOutlineColor.a, 1f, position)
                ));
            await DelayAsync(.03f);
            position = (Time.time - initialTime) / duration;
        }
        material.SetColor("_FaceColor", new Color(initialFaceColor.r, initialFaceColor.g, initialFaceColor.b, 1));
        material.SetColor("_OutlineColor", new Color(initialOutlineColor.r, initialOutlineColor.g, initialOutlineColor.b, 1));
    }
    public static async Task FadeOut(TMP_FontAsset font, float duration = 1f)
    {
        var material=font.material;
        var initialFaceColor = material.GetColor("_FaceColor");
        var initialOutlineColor = material.GetColor("_OutlineColor");
        var initialTime = Time.time;
        var position = (Time.time - initialTime) / duration;
        while (position < 1)
        {
            
            material.SetColor("_FaceColor", 
                new Color(initialFaceColor.r, initialFaceColor.g, initialFaceColor.b,
                Mathf.Lerp(initialFaceColor.a, 0f, position) 
                ));
            material.SetColor("_OutlineColor",
                new Color(initialOutlineColor.r, initialOutlineColor.g, initialOutlineColor.b,
                Mathf.Lerp(initialOutlineColor.a, 0f, position)
                ));
            await DelayAsync(.03f);
            position = (Time.time - initialTime) / duration;
        }
        material.SetColor("_FaceColor", new Color(initialFaceColor.r, initialFaceColor.g, initialFaceColor.b, 0));
        material.SetColor("_OutlineColor", new Color(initialOutlineColor.r, initialOutlineColor.g, initialOutlineColor.b, 0));
    }
    public static async Task DelayAsync(float secondsDelay)
    {
        float startTime = Time.time;
        while (Time.time < startTime + secondsDelay) await Task.Yield();
    }
}
