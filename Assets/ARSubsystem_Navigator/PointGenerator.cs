using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PointCoords
{
    public Vector2 coords;
    public string instruction;

    public PointCoords(Vector2 xy, string instr)
    {
        coords = xy;
        instruction = instr;
    }
}

public class PointGenerator : MonoBehaviour
{
    public Vector2 test_origin;
    public Vector2[] testPaths;
    public List<PointCoords> paths = new List<PointCoords>();
    public AdjustAngle AA;
    public StretchCubeBetweenPoints linegen;
    public Text textG;

    public bool test = false;
    List<GameObject> currSpawn = new List<GameObject>();
    public Transform Origin;
    public GameObject Point;
    public int MaxPoints = 3;

    public float MinDist = 2;

    public bool GeneratePaths;

    int curr_ind = 0;
    int max_ind = 3;

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

        test_origin.x = Input.location.lastData.latitude;
        test_origin.y = Input.location.lastData.longitude;
        GeneratePOI();
    }


    public void GeneratePOI()
    {
        int j = 0;
        for (int i = curr_ind; i < max_ind; i++)
        {
            print(i);
            var xy = paths[i].coords;
            if (i+1 < paths.Count)
                textG.text = paths[i+1].instruction;

            if (test)
                xy = LatLong_xy(test_origin, testPaths[i]);
            
            if (currSpawn.Count <= j)
                currSpawn.Add(Instantiate(Point, new Vector3(xy.x, 0, xy.y), Quaternion.identity, Origin));
            else 
                currSpawn[j].transform.position = new Vector3(xy.x, 0, xy.y);

            j++;
        }
    }

    void QueryCurrDist()
    {

        var rs = currSpawn[0].transform.position - Camera.main.transform.position;
        linegen.CreateStretchedCube(Camera.main.transform.position, currSpawn[0].transform.position);
        // print(rs.magnitude);
        if (rs.magnitude < MinDist)
        {
            if (max_ind + 1 < paths.Count)
            {
                curr_ind++; max_ind++;
                GeneratePOI();
            }
            else
            {
                textG.text = "Arrived at destination";
            }
        }

        var thet = Mathf.Atan(rs.z / rs.x) * Mathf.Rad2Deg;
        print(thet);//# print(thet2);

        if (rs.x > 0 && rs.z > 0)
        {
            thet = 90 - thet;
        }

        if (rs.x > 0 && rs.z < 0)
        {
            thet *= -1;
            thet += 90;
        }

        if (rs.x < 0 && rs.z < 0)
        {
            thet = 270 - thet;
        }

        if (rs.x < 0 && rs.z > 0)
        {
            thet *= -1;
            thet += 270;
        }

        thet -= Input.compass.trueHeading;

        AA.AdjustAngl(thet);
    }

    // Update is called once per frame
    void Update()
    {
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
