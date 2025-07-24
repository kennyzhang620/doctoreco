using UnityEngine;

public class OrientNorth : MonoBehaviour
{
    public PointGenerator p;
    public GameObject head;
    public EventTrigger_C events;
    public void Load()
    {
        if (Input.compass.trueHeading < 10.0f || Input.compass.trueHeading >= 350.0f)
        {
            var v = Instantiate(head, this.transform);

            // Orient an object to point northward.
            v.transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
            p.Origin = v.transform;

            events.Trigger();
            enabled = false;
        }

        _td = 5;
    }

    float _td = 5;
    private void Update()
    {
        if (!enabled) return;

        if (_td > 0)
        {
            _td -= Time.deltaTime;
            return;
        }

        Load();
    }

}
