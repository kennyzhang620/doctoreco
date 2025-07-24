using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointGenerator : MonoBehaviour
{
    public Vector2 test_origin;
    public Vector2[] testPaths;


    List<GameObject> currSpawn = new List<GameObject>();
    public Transform Origin;
    public GameObject Point;
    public int MaxPoints = 3;

    public int MinDist = 2;

    public bool GeneratePaths;

    int curr_ind = 0;
    int max_ind = 5;

    public float radians(float deg)
    {
        return deg * Mathf.Deg2Rad;
    }

    public Vector2 LatLong_xy(Vector2 origin, Vector2 point)
    {
        float R = 6371000;

        float lat0 = radians(origin.x);
        float lon0 = radians(origin.y);

        var x = R * (radians(point.y) - lon0) * Mathf.Cos(lat0);
        var y = R * (radians(point.x) - lat0);

        return new Vector2(x, y);

    }

    float relerror(Vector2 i, Vector2 o)
    {
        return (i.magnitude - o.magnitude) / i.magnitude;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _testlatlon();
        max_ind = MaxPoints;
    }


    public void GeneratePOI()
    {
        int j = 0;
        for (int i = curr_ind; i < max_ind; i++)
        {
            var xy = LatLong_xy(test_origin, testPaths[i]);
            
            if (currSpawn.Count <= j)
                currSpawn.Add(Instantiate(Point, new Vector3(xy.x, 0, xy.y), Quaternion.identity, Origin));
            else 
                currSpawn[j].transform.position = new Vector3(xy.x, 0, xy.y);

            j++;
        }
    }

    void QueryCurrDist()
    {

        if (LatLong_xy(test_origin, testPaths[curr_ind]).magnitude < MinDist)
        {
            curr_ind++; max_ind++;
            GeneratePOI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        test_origin.x = Input.location.lastData.latitude;
        test_origin.y = Input.location.lastData.longitude;

        if (GeneratePaths)
        {
            GeneratePaths = false;
            GeneratePOI();
        }

        QueryCurrDist();
    }

    void _testlatlon()
    {
        Vector2 origin = new Vector2(49.18682308524388f, -123.1004400840039f);

        Vector2[] test = {
            new Vector2(49.188349635253466f, -123.10036454569875f),
            new Vector2(49.18833608913238f, -123.09827119896379f),
            new Vector2(49.185538270569005f, -123.10098217499468f),
            new Vector2(49.18680676574397f, -123.09977095617032f),
        };

        Vector2[] result = {
            new Vector2(5.489853032026322f, 169.74461633567407f),
            new Vector2(157.6267840406154f, 168.23835639449268f),
            new Vector2(-39.39722850902235f, -142.86487352425016f),
            new Vector2(48.62981051079299f, -1.814645595406894f)
        };

        print(Mathf.Acos(Vector2.Dot(result[3],Vector2.up)/(result[3].magnitude * 1)));
        for (int i=0;i<4;i++)
        {
            if (Mathf.Abs(relerror(LatLong_xy(origin, test[i]), result[i])) < 0.1f)
            {
                Debug.LogError("test failed" + i);
                print(Mathf.Abs(relerror(LatLong_xy(origin, test[i]), result[i])));
               // return;
            }
        }

    }
}
