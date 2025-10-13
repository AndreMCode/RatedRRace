using UnityEngine;

public class ScreenResize : MonoBehaviour
{
    public Canvas canvas;

    private void Update()
    {
        if(canvas != null)
        {
            RectTransform size = canvas.GetComponent<RectTransform>();
            size.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        }
    }
}
