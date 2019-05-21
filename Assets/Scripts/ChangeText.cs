
using UnityEngine;
using UnityEngine.UI;

/*
 * Class that updates text of the canvas elements
 */

public class ChangeText : MonoBehaviour
{
    [SerializeField]
    private GameObject widthTextGO, heightTextGO;
    private Text widthText, heightText; // Stores the text by the slider to change maze width and height respectively

    private void Start()
    {
        widthText = widthTextGO.GetComponent<Text>();
        heightText = heightTextGO.GetComponent<Text>();
    }

    // Method to update the UI text element showing maze width
    public void ChangeWidthText(float inputWidth)
    {
        if(widthText)
            widthText.text = inputWidth.ToString();
    }

    // Method to update the UI text element showing maze height
    public void ChangeHeightText(float inputHeight)
    {
        if(heightText)
            heightText.text = inputHeight.ToString();
    }
}
