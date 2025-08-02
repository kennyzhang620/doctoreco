using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


//INSTRUCTIONS TO RUN:
//can run AskGemini and FetchPlaces independently, just comment out the
//call to FetchPlaces at the end of AskGemini and input the prompt + placesSearchTerm
//fields. Current setup runs FetchPlaces off of the result to AskGemini
//Note: if you get a result from Gemini that ISN'T a viable search term,
//let me know and i'll fix the promptAddOn.
public class APIWorkflow : MonoBehaviour
{
    [SerializeField] private string prompt;
    [SerializeField] private string placesSearchTerm; //this is a stand in
    [SerializeField] private string clinicName;
    [SerializeField] private string cityName;
    [SerializeField] private string rateMDSlug;

    private string geminiURL = "https://script.google.com/macros/s/AKfycbzTUcx1vJBLsPVtKbyQk4sya396lEDL8r7RiAa8Zzbbs7W9NsWJ3mdAIsCl8TNsMl8Psg/exec";
    private string placesGetNearbyURL = "https://script.google.com/macros/s/AKfycbx5ayZNlCFvQU1TMrhsp7L8D0b9jKN9gTVlJHIUAmk7ejlDsAVPN1_2s60ooLt9O98/exec";

    private string mapsSearchTerm = "";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            //Debug.Log(prompt);
            StartCoroutine(AskGemini());
            //Debug.Log(placesSearchTerm);
            //StartCoroutine(FetchPlaces(placesSearchTerm));
            //StartCoroutine(GetZembraSlug(clinicName, cityName));
            //StartCoroutine(CreateZembraListingJob(rateMDSlug));
            //StartCoroutine(GetZembraReviews(rateMDSlug));
        }
    }

    private string promptAddOn = "I'd like to see a medical specialist for these symptoms. Could you give me a single search term that can be put into google maps to look for a doctor in a one-word response?";
    private IEnumerator AskGemini() {
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt + promptAddOn);

        UnityWebRequest www = UnityWebRequest.Post(geminiURL, form);
        yield return www.SendWebRequest();

        string response = "";

        if (www.result == UnityWebRequest.Result.Success) {
            response = www.downloadHandler.text;
            mapsSearchTerm = response;
        }
        else {
            response = www.error;
            //display some sort of text that says there was an error?
        }

        Debug.Log(response);

        FetchPlaces(mapsSearchTerm);
    }

    private float latitude = 0;
    private float longitude = 0;
    private int radiusMeters = 3000;

    private IEnumerator FetchPlaces(string keyword)
    {
        float desiredAccuracyInMeters = 10f;
        float updateDistanceInMeters = 10f;

        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1) {
            Debug.Log("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed) {
            Debug.LogError("Unable to determine device location");
            yield break;
        } 
        else {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        Input.location.Stop();

        latitude = 49.229869f;
        longitude = -123.005902f; //dummy data for metrotown burnaby

        WWWForm form = new WWWForm();
        form.AddField("keyword", keyword);
        form.AddField("radius", radiusMeters);
        form.AddField("longitude", longitude.ToString());
        form.AddField("latitude", latitude.ToString());
        
        UnityWebRequest request = UnityWebRequest.Post(placesGetNearbyURL, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log(request.downloadHandler.text);
            //string text = request.downloadHandler.text;
            //GoogleMapsResponse mapresponse = JsonUtility.FromJson<GoogleMapsResponse>(text);
            //Debug.Log(mapresponse.results[0]);
        }
        else {
            Debug.LogError(request.error);
        }
    }

    private string slugURL = "https://script.google.com/macros/s/AKfycby6HBor67G2E8-eWjjVZVr7uS-4JYY_D_zaoVPIsmbKZVi9lKIGC3OWX5eHaQeDDxQq/exec";

    //currently not very functioning. 
    private IEnumerator GetZembraSlug(string placeName, string placeCity)
    {
        WWWForm form = new WWWForm();
        form.AddField("businessName", placeName);
        form.AddField("city", placeCity);

        UnityWebRequest www = UnityWebRequest.Post(slugURL, form);
        yield return www.SendWebRequest();

        string response = "";

        if (www.result == UnityWebRequest.Result.Success) {
            response = www.downloadHandler.text;
        }
        else {
            response = www.error;
        }

        Debug.Log(response);

        CreateZembraListingJob(response);//which should be the slug
    }

    //should be called before GetZembraReviews on the same slug, retrieved from GetZembraSlug
    private string createJobURL = "https://script.google.com/macros/s/AKfycbzmxoQwYN2UC1STXZvrGJsspQmrQDNfKxFO2PYzGkWSQhy9pDEYB34QrtmTBa7qJzn3hw/exec";
    private IEnumerator CreateZembraListingJob(string slug)
    {
        WWWForm form = new WWWForm();
        form.AddField("slug", slug);

        UnityWebRequest www = UnityWebRequest.Post(createJobURL, form);
        yield return www.SendWebRequest();

        string response = "";

        if (www.result == UnityWebRequest.Result.Success) {
            response = www.downloadHandler.text;
        }
        else {
            response = www.error;
        }

        Debug.Log(response);
    }

    private string getReviewsURL = "https://script.google.com/macros/s/AKfycbxlmfHGsVStJCyL526pteOumvqSCRtjB03CwGTs5VSC5Q1Mbs3a9wUHdiQDd640-Lfz9A/exec";
    private IEnumerator GetZembraReviews(string slug)
    {
        WWWForm form = new WWWForm();
        form.AddField("slug", slug);
        form.AddField("jobId", "");
        form.AddField("uid", "");

        UnityWebRequest www = UnityWebRequest.Post(getReviewsURL, form);
        yield return www.SendWebRequest();

        string response = "";

        if (www.result == UnityWebRequest.Result.Success) {
            response = www.downloadHandler.text;
        }
        else {
            response = www.error;
        }

        Debug.Log(response);
    }

    void Start () {
    }
}

public class GoogleMapsResponse
{
    public List<string> html_attributions;
    public List<PlaceInfo> results;
}

public class PlaceInfo
{
    public string business_status;
    public PlaceCoordinates geometry;
    public string icon;
    public string icon_background_color;
    public string icon_mask_base_uri;
    public string name;
    public string place_id;
    public PlusCode plus_code;
    public double rating;
    public string reference;
    public string scope;
    public List<string> types;
    public int user_ratings_total;
    public string vicinity;
}

public class PlusCode
{
    public string compound_code;
    public string global_code;
}

public class PlaceCoordinates
{
    public CoordPair location;
    public ViewPort viewport;
}

public class ViewPort
{
    public CoordPair northeast;
    public CoordPair southwest;
}

public class CoordPair
{
    public double lat;
    public double lng;
}