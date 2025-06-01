using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool isFuture = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isFuture = !isFuture;
            Debug.Log("Switched to " + (isFuture ? "Future" : "Present"));
        }
    }
}
