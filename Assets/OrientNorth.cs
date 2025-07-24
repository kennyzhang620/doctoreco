using UnityEngine;

public class OrientNorth : MonoBehaviour
{
    public AdjustAngle AA;

    public void Load()
    {
        if (Input.compass.trueHeading < 10.0f || Input.compass.trueHeading >= 350.0f)
        {
            gameObject.SetActive(true);
            // Orient an object to point northward.
            transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        AA.AdjustAngl(Input.compass.trueHeading);
    }
}
