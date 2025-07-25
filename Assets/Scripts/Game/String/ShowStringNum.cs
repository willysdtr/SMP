using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ShowStringNum : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;

    public void UpdateDisplay(List<int> list)
    {
        textDisplay.text = string.Join(", ", list);
    }
}
