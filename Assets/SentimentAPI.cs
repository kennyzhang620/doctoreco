using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class SentimentAPI
{
    public static async Task<string> PostRequest(string url, string jsonPayload)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                return $"Error: {request.error}";
            }
            else
            {
                return request.downloadHandler.text;
            }
        }
    }
}
