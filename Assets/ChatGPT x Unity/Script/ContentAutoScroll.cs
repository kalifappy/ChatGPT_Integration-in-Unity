using UnityEngine;
using UnityEngine.UI;

public class ContentAutoScroll : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private ScrollRect scrollRect;

    private void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        scrollRect = GetComponentInParent<ScrollRect>(); // Get the ScrollRect in the parent

        DiscussionManager.onMessageReceived += ScrollDown; // Scroll immediately on new message
    }

    private void OnDestroy()
    {
        DiscussionManager.onMessageReceived -= ScrollDown;
    }

    public void ScrollDown()
    {
        if (scrollRect == null) return;

        Canvas.ForceUpdateCanvases(); // Ensure UI updates before scrolling

        // Smoothly move scroll to the bottom
        scrollRect.verticalNormalizedPosition = 0;
    }
}
