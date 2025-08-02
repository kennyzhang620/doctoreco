using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiHandler : MonoBehaviour
{

    public string currOutPut;
    [SerializeField]
    public string prompt;

    public bool isProcessing;

    public enum promptState
    {
        ok, 
        error
    }

    public promptState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currOutPut = "";
        state = promptState.ok;
        isProcessing = false;
    }

    public void MakePrompt(string contents)
    {
        StartCoroutine(QueryTransformer(contents));
    }


    private IEnumerator QueryTransformer(string contents)
    {
        isProcessing = true;
        WWWForm form = new WWWForm();
        form.AddField("prompt", prompt + ":" + contents);
        UnityWebRequest req = UnityWebRequest.Post(EnvLoader.env.GOOGLE_API_URL, form);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            currOutPut = req.downloadHandler.text;
            state = promptState.ok;
        } else
        {
            currOutPut = "";
            state = promptState.error;

        }
        isProcessing = false;
   
    }
}
