using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class VideoSpeedToggle : MonoBehaviour
{
    [SerializeField] private VideoPlayer wordVideoPlayer;
    [SerializeField] private VideoPlayer definitionVideoPlayer;
    private bool isSlow;
    [SerializeField] private float slowSpeed = 0.75f;
    private Image buttonImg;
    private TextMeshProUGUI buttonText;
    private Color defaultColor;
    [SerializeField] private Color slowColor;

    private void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonImg = GetComponent<Image>();
        defaultColor = buttonImg.color;
        isSlow = false;
    }

    public void ToggleSpeed()
    {
        ChangeButtonStyle(!isSlow);
        ChangeVideoSpeed(isSlow ? 1f : slowSpeed);
        isSlow = !isSlow;
    }

    private void ChangeButtonStyle(bool changeToSlowStyle)
    {
        if (changeToSlowStyle)
        {
            buttonImg.color = slowColor;
            buttonText.text = "Slow";
        } else
        {
            buttonImg.color = defaultColor;
            buttonText.text = "Default";
        }
    }

    private void ChangeVideoSpeed(float videoSpeed)
    {
        wordVideoPlayer.playbackSpeed = videoSpeed;
        definitionVideoPlayer.playbackSpeed = videoSpeed;
    }
}
