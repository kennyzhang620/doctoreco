using UnityEngine;

public class StretchCubeBetweenPoints : MonoBehaviour
{
    public GameObject cubePrefab;     // Assign in Inspector
    public Vector3 pointA = Vector3.zero;
    public Vector3 pointB = new Vector3(0, 0, 10);
    public float YOffset = -0.56f;

    GameObject stretchedCube;

    void Start()
    {
        if (cubePrefab == null)
        {
            Debug.LogError("Cube prefab not assigned!");
            return;
        }

       // CreateStretchedCube(pointA, pointB);
    }

    public void CreateStretchedCube(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        Vector3 center = (start + end) / 2f;
        center.y = YOffset;
        float length = direction.magnitude;

        if (!stretchedCube)
            stretchedCube = Instantiate(cubePrefab, center, Quaternion.identity);
        else
            stretchedCube.transform.position = center;

        // Adjust scale: assume the cube is 1 unit in Z by default
        stretchedCube.transform.localScale = new Vector3(0.6f, 0.6f, length); // thin X and Y, stretched Z

        // Rotate to align with direction
        stretchedCube.transform.rotation = Quaternion.LookRotation(direction);
    }
}
