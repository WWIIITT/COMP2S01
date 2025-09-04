using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public float duration = 0.5f;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    private bool isScrolling = false;
    private int currentIndex = 0; // Keep track of the current item index

    void Start()
    {
        // Initialize the ScrollRect to start without any automatic movement
        if (contentPanel.childCount > 0)
        {
            scrollRect.horizontalNormalizedPosition = 0f;  // Start at the beginning of the scroll view
        }

        // Optionally center on the first item at start
        if (contentPanel.childCount > 0)
        {
            CenterOnItem(currentIndex);
        }
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    public void MoveNext()
    {
        if (!isScrolling && currentIndex < contentPanel.childCount - 1)
        {
            currentIndex++;
            CenterOnItem(currentIndex);
        }
    }

    public void MovePrevious()
    {
        if (!isScrolling && currentIndex > 0)
        {
            currentIndex--;
            CenterOnItem(currentIndex);
        }
    }

    private void CenterOnItem(int index)
    {
        StartCoroutine(SmoothScrollTo(CalculateCenteredPosition(index)));
    }

    private float CalculateCenteredPosition(int index)
    {
        // Calculate the center position of the desired item
        RectTransform itemRect = (RectTransform)contentPanel.GetChild(index);
        float itemWidth = itemRect.rect.width;
        float itemCenterPosition = itemRect.anchoredPosition.x + itemWidth / 2 - 300;
        float scrollableWidth = contentPanel.rect.width - scrollRect.viewport.rect.width;

        if (scrollableWidth <= 0) return 0;  // Avoid division by zero or negative widths

        float centeredPosition = (itemCenterPosition - scrollRect.viewport.rect.width / 2) / scrollableWidth;
        return Mathf.Clamp01(centeredPosition);
    }

    private IEnumerator SmoothScrollTo(float targetPosition)
    {
        isScrolling = true;
        float timeElapsed = 0;
        float startValue = scrollRect.horizontalNormalizedPosition;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / duration;
            float interpolatedValue = curve.Evaluate(normalizedTime);
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startValue, targetPosition, interpolatedValue);
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = targetPosition;
        isScrolling = false;
        UpdateCurrentIndex();
    }

    private void OnScrollValueChanged(Vector2 scrollPosition)
    {
        if (!isScrolling)
        {
            UpdateCurrentIndex();
        }
    }

    private void UpdateCurrentIndex()
    {
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            float distance = Mathf.Abs(CalculateCenteredPosition(i) - scrollRect.horizontalNormalizedPosition);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                currentIndex = i;
            }
        }
    }
}
