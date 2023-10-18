using UnityEngine;

using System.Net;
using System.IO;

public class http
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Request()
    {
        Debug.Log($"http Request 111");
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:40080/Default.aspx?cmd=2");
        httpWebRequest.ContentType = "text/json";
        httpWebRequest.Method = "POST";
        Debug.Log($"http Request 222");

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            Debug.Log($"http Request 333");
            string json = "{\"kakao_id\":\"1\",\"image_url\":\"http://teste11111.com\",\"public_profile\":\"Y\"}";

            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
            Debug.Log($"http Request 444");

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Debug.Log($"http Request 555");
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                //MessageBox.Show(result);
            }
        }
        Debug.Log($"http Request 111");
    }
}
