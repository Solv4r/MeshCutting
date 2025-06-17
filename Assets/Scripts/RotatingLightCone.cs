using UnityEngine;

public class RotatingLightCone : MonoBehaviour
{
    public Transform targetA; // The direction to look at for position A
    public Transform targetB; // The direction to look at for position B
    public float rotationSpeed = 2f;

    private bool isNear = false;
    private bool lookingAtA = true;
    private Transform player;

    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Toggling direction.");
            ToggleDirection();
        }

        RotateTowardsTarget();
    }

    void ToggleDirection()
    {
        lookingAtA = !lookingAtA;
    }

    void RotateTowardsTarget()
    {
        Transform target = lookingAtA ? targetA : targetB;
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger area.");
            isNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger area.");
            isNear = false;
        }
    }
}