using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TinyJson;
using System.Collections.Generic;

public class NavigationStep
{
    public double lat;
    public double lon;
    public string instruction;
}

public class NavigationData
{
    public NavigationStep[] coords;
}


public class JSONGetter : MonoBehaviour
{
    [Header("Set the URL of the JSON endpoint")]
    public string jsonUrl = "https://testfh.novanode-test.cc/test/";
    public PointGenerator pg;

    double lat, longt;

    public Vector2 testSource;
    public Vector2 dest;
    public bool test = false;
    void Start()
    {

        lat = Input.location.lastData.latitude;
        longt = Input.location.lastData.longitude;

        if (test)
        {
            lat = testSource.x; longt = testSource.y;
        }

        jsonUrl = "https://testfh.novanode-test.cc/route/?origin_lat=" + lat.ToString() + "&origin_lon=" + longt.ToString() +"&destination_lat=" + dest.x.ToString() + "&destination_lon=" + dest.y.ToString() + "&skey=o98765redfghu";

        StartCoroutine(GetJsonFromUrl(jsonUrl));
    }

    void ParseJSON()
    {

    }

    IEnumerator GetJsonFromUrl(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Optional: Set request headers here, if needed
            // request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching JSON: {request.error}");
            }
            else
            {
                string jsonText = request.downloadHandler.text;
                Debug.Log("Received JSON:\n" + jsonText);

                NavigationData b = JSONParser.FromJson<NavigationData>(jsonText);

                foreach (var x in b.coords)
                {
                    print("Item=> "+ x.lat);
                    pg.paths.Add(new PointCoords(new Vector2((float)x.lat, (float)x.lon), x.instruction));
                }
                // Optional: parse JSON here using JsonUtility or another JSON library
                // Example: MyData data = JsonUtility.FromJson<MyData>(jsonText);

                pg.enabled = true;
            }
        }
    }
}
