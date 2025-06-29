using UnityEngine;

public enum TimeState { Present, Future, Both }

public class TimeStateObject : MonoBehaviour
{
    public TimeState state;
    public GameObject visualObject;

    private bool isInFutureZone = false;

    public void SetInFutureZone(bool inside)
    {
        isInFutureZone = inside;
        UpdateVisibility();
    }

    void Start()
    {
        UpdateVisibility(); // show the present state by default
    }

    void UpdateVisibility()
    {
        if (visualObject == null) return;

        switch (state)
        {
            case TimeState.Present:
                visualObject.SetActive(!isInFutureZone);
                break;
            case TimeState.Future:
                visualObject.SetActive(isInFutureZone);
                break;
            case TimeState.Both:
                visualObject.SetActive(true); // always visible
                break;
        }
    }

    void OnDisable()
    {
        isInFutureZone = false;
    }
}
