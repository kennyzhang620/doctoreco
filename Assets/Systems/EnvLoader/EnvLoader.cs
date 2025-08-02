using UnityEngine;

public class EnvLoader : MonoBehaviour
{
    [SerializeField] 
    public TextAsset envJson;

    public static EnvLoader Instance;

    public static ENV env;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        env = JsonUtility.FromJson<ENV>(envJson.text);

    }


}

[System.Serializable]
public class ENV
{
    public string GOOGLE_API_URL;

}