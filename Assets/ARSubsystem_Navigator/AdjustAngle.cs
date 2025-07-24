using UnityEngine;

public class AdjustAngle : MonoBehaviour
{
    public Animator anim;

    public void AdjustAngl(float a)
    {
        if (a < 0)
            a = 360 - a;

        a = a % 360;

        anim.Play(0, 0, (a / 360.0f));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    float _td = 3.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (enabled)
        {
            AdjustAngl(Input.compass.trueHeading);

        }
    }
}
