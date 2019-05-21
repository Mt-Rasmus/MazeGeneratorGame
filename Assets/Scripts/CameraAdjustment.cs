
using UnityEngine;

/*
 * Class that adjusts the orthographic camera so that the enire maze is always visible
 */

public class CameraAdjustment : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer square;
    [SerializeField]
    private int width, height;
    private float orthoSize;

    private bool IsOdd(int nr)
    {
        return (nr % 2 != 0);
    }

    // Method to change size of orthographic camera based on the dimensions of the newly create maze
    public void AdjustCamera()
    {
        if ((IsOdd(width) && width - height >= height + 1) || (!IsOdd(width) && width - height > height))
            orthoSize = width * square.bounds.size.x * Screen.height / Screen.width * 0.5f;
        else
            orthoSize = (height * square.bounds.size.x) * 0.5f;

        Camera.main.orthographicSize = orthoSize * (orthoSize / (orthoSize - 0.03775f));
    }

    // Method to change maze width, gets input from GUI slider
    public void ChangeWidth(float inputWidth)
    {
        width = (int)inputWidth;
    }

    // Method to change maze height, gets input from GUI slider
    public void ChangeHeight(float inputHeight)
    {
        height = (int)inputHeight;
    }

}
