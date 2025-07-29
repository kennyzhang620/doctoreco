using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class APIWorkflow : MonoBehaviour
{
    [SerializeField] private string prompt;
    [SerializeField] private string placesSearchTerm; //this is a stand in

    private string geminiURL = "https://script.google.com/macros/s/AKfycbzTUcx1vJBLsPVtKbyQk4sya396lEDL8r7RiAa8Zzbbs7W9NsWJ3mdAIsCl8TNsMl8Psg/exec";
    private string placesGetNearbyURL = "https://script.google.com/macros/s/AKfycbx5ayZNlCFvQU1TMrhsp7L8D0b9jKN9gTVlJHIUAmk7ejlDsAVPN1_2s60ooLt9O98/exec";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log(prompt);
            StartCoroutine(AskGemini());
            Debug.Log(placesSearchTerm);
            StartCoroutine(FetchPlaces(placesSearchTerm));
        }
    }

    //the real input should just be the symptoms, so we might have to append further context onto it
    //ex: "given my symptoms, what type of specialist should i see?"
    //how should we approach the post-processing (trimming gemini's potentially wordy answers into
    //  viable search keywords)?
    private IEnumerator AskGemini() {
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt);

        UnityWebRequest www = UnityWebRequest.Post(geminiURL, form);
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
        }
        else {
            Debug.LogError(request.error);
        }
    }

    void Start () {
    }
}
