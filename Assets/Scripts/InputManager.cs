using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool isDragging = false;
    private Vector2 startTouch, swipeDelta;

    private SwipeInputValues inputValues;

    //Properties
    public bool IsDragging { get { return isDragging; } }
    public SwipeInputValues SwipeInputValues { get { return inputValues; } }


    // Update is called once per frame
    void Update()
    {
        //inputValues = SwipeInputValues.Null;

        Inputs();
        CalculateSwipeDistance();
    }

    private void Inputs()       //PC and Touch inputs -- Dragging
    {
        #region PC Inputs
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SwipeEnd();
        }
        #endregion

        #region Touch Inputs
        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                isDragging = true;
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                SwipeEnd();
            }
        }
        #endregion
    }

    private void SwipeEnd()
    {
        if (inputValues != SwipeInputValues.Null)
            EventsService.Instance.InvokeOnInputDirectionEvent(inputValues);
        Reset();
    }

    private void Reset()        //Resetting the swipe distance
    {
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
        Debug.Log(inputValues);
        inputValues = SwipeInputValues.Null;

    }
    private void CalculateSwipeDistance()
    {
        swipeDelta = Vector2.zero;
        if (isDragging)
        {
            //calculating the distance of swipe performed
            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }

        //checking if swipe exceeded the deadzone(no input will be considered if swipe is less than the zone)
        if (swipeDelta.magnitude > 50)         //Till 50 pixels is considered as deadzone
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;
            if (Mathf.Abs(x) > Mathf.Abs(y))         //Movement in x axis
            {
                if (x < 0)
                    inputValues = SwipeInputValues.SwipeLeft;
                else
                    inputValues = SwipeInputValues.SwipeRight;
            }
            else                                   //Movement in y axis
            {
                if (y < 0)
                    inputValues = SwipeInputValues.SwipeDown;
                else
                    inputValues = SwipeInputValues.SwipeUp;

            }
        }

    }


}

public enum SwipeInputValues
{
    Null,
    SwipeLeft,
    SwipeRight,
    SwipeUp,
    SwipeDown
}