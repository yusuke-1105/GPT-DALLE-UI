using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using UnityEngine;
using System.Text;
using System;

public class HTTPRequest : MonoBehaviour
{
    static JObject jsonResponse = null;

    // connect API and get the response ------------------------------------------------------
    public static async Task<JObject> Link(string url, Dictionary<string, string> headers, string jsonBody)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);

        foreach (KeyValuePair<string, string> header in headers)
        {
            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);
        Debug.Log(response);
        var responseContents = await response.Content.ReadAsStringAsync();
        jsonResponse = JObject.Parse(responseContents);
        // Debug.Log(jsonResponse);

        return jsonResponse;
    }

    // get the image from the url -------------------------------------------------------------
    public static async Task<Texture2D> GetImage(string url)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);
        var contents = await response.Content.ReadAsByteArrayAsync();
        var texture = new Texture2D(1, 1);
        texture.LoadImage(contents);
        return texture;
    }
}

