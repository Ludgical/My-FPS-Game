using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmoothScrollRect : ScrollRect
{
    private bool scrolled;
    private float expectedY = 1;
    private float targetY = 1;
    private float menuVelocity;

    public override void OnScroll(PointerEventData eventData)
    {
        //When using the scroll wheel, move the target position of the menu
        targetY += eventData.scrollDelta.y * 0.01f * scrollSensitivity;
        targetY = Mathf.Clamp01(targetY);
        scrolled = true;
    }
    
    private void Update()
    {
        //If the player didn't use the scroll wheel, don't scroll smoothly
        if (!scrolled)
        {
            targetY = verticalNormalizedPosition;
            expectedY = verticalNormalizedPosition;
            return;
        }

        //If the player used the scroll wheel, but the expected y-position
        //isn't equal to the action y-position, the player started dragging
        //the menu, cancel the smooth scrolling
        if (expectedY != verticalNormalizedPosition)
        {
            scrolled = false;
            menuVelocity = 0;
            return;
        }
    
        //The player used the scroll wheel - scroll smoothly
        ScrollSmoothly();

        //The menu reached the target - cancel the smooth scrolling
        if (Mathf.Abs(verticalNormalizedPosition - targetY) < 0.001)
        {
            verticalNormalizedPosition = targetY;
            expectedY = targetY;
            menuVelocity = 0;
            scrolled = false;
        }
    }
    
    private void ScrollSmoothly()
    {
        //Move the menu towards the target position smoothly
        verticalNormalizedPosition = Mathf.SmoothDamp(
            verticalNormalizedPosition, targetY, ref menuVelocity, 0.08f);
        expectedY = verticalNormalizedPosition;
    }
}
