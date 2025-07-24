using UnityEngine;

public class AdjustAngle : MonoBehaviour
{
    public Animator anim;

    public void AdjustAngl(float a)
    {
        anim.Play(0, 0, (a / 360.0f));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
