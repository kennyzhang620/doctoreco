using System.Collections;
using UnityEngine;

public class HandlePatientQuery : MonoBehaviour
{

    public TMPro.TMP_InputField queryField;
    public ApiHandler handler;

    public void ProcessQuery()
    {
        if (queryField.text == "") { return; }
        StartCoroutine(ProcessQuerySubroutine());
    }

    IEnumerator ProcessQuerySubroutine()
    {
        string current = queryField.text;
        handler.MakePrompt(current);
        while (handler.isProcessing)
        {
            yield return new WaitForEndOfFrame();
        }

        if (handler.state == ApiHandler.promptState.ok) {

            UIStateHandler.UISingleton.SetMapView();
        } else
        {
            // show error
        }
        Debug.Log(handler.currOutPut);
    }
}
