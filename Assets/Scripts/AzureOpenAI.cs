using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine;
using System.Text;
using System.IO;
using TMPro;

// https://learn.microsoft.com/ja-jp/azure/ai-services/openai/reference

public class AzureOpenAI : MonoBehaviour
{
    string jsonBody;
    string urlImage;

    // API key for Azure OpenAI Service
    public string ApiKeyAzure;
    // URL for Azure OpenAI Service
    public string urlGPTAzure;
    public string urlDALLEAzure;

    // API key for OpenAI
    public string ApiKeyOpenAI;
    // URL for OpenAI
    public string urlGPTOpenAI = "https://api.openai.com/v1/chat/completions";
    public string urlDALLEOpenAI = "https://api.openai.com/v1/images/generations";

    // parameters for GPT
    [SerializeField] public int MaxTokens = 80;
    [SerializeField] public int MemorablePairs = 0;
    List<Dictionary<string, string>> PreviousMessage;

    // for UI
    ButtonManipulator ButtonManipulator;    // for getting the source of GPT & DALLE and handling the rotation of Emoji
    public TextMeshProUGUI textGPT;         // present the output of GPT
    public Image imageDALLE;                // present the output of DALLE

    public void Awake()
    {
        PreviousMessage = new List<Dictionary<string, string>>();

        try
        {
            string[] SettingTextForPreprocessing;
            string SettingText;
            // get the setting text from the .txt file
            using (StreamReader reader = new StreamReader("./AgentSettings.txt", Encoding.UTF8))
            {
                SettingTextForPreprocessing = reader.ReadToEnd().Split('\n');
            }
            SettingText = string.Join("", SettingTextForPreprocessing);

            PreviousMessage.Add(new Dictionary<string, string>  // add the agent's role to jsonBody
            {
                {"role", "system"},
                {"content", SettingText}
            });
        }
        catch
        {
            Debug.Log("Failed to read the setting text...");
        }

        // get the instance of ButtonManipulator.cs
        ButtonManipulator = GameObject.Find("EventSystem").GetComponent<ButtonManipulator>();
    }

    // connect API and get the response -----------------------------------------------------------------
    public async Task<JObject> ConnectToOpenAPI(string url, string jsonBody)
    {
        ButtonManipulator.CommandToEmoji();     // start rotating the Emoji (optional)

        Dictionary<string, string> headers = new Dictionary<string, string>();
        if(ButtonManipulator.source == "Azure") headers.Add("Authorization", $"Bearer {ApiKeyAzure}");
        else headers.Add("Authorization", $"Bearer {ApiKeyOpenAI}");
        headers.Add("accept", "application/json");
        JObject response = await HTTPRequest.Link(url, headers, jsonBody);

        Debug.Log(response);

        ButtonManipulator.CommandToEmoji();     // stop rotating the Emoji (optional)

        // {
        // "statusCode": 401,
        // "message": "Unauthorized. Access token is missing, invalid, audience is incorrect (https://cognitiveservices.azure.com), or have expired."
        // }

        return response;
    }

    // prepare the input json body for GPT -------------------------------------------------------------
    public async Task GPTCompletion(string text) 
    {
        PreviousMessage.Add(new Dictionary<string, string>  // add the user's text to jsonBody
        {
            {"role", "user"},
            {"content", text}
        });

        var stop = new List<string> {"]","。"};             // for Japanese(optional)

        // for Azure ++++++++++++++++++++++++++
        if(ButtonManipulator.source == "Azure")
        {
            var request = new       // add parameters to jsonBody
            {
                max_tokens = MaxTokens,
                stop = stop,
                stream = true,
                prompt = PreviousMessage
            };

            jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var result = await ConnectToOpenAPI(urlGPTAzure, jsonBody);   // get response from GPT
            textGPT.text = result["choices"][0]["text"].ToString();       // get the text from the response
        }
        // for OpenAI +++++++++++++++++++++++++++++++
        else if(ButtonManipulator.source == "OpenAI")
        {
            var request = new        // add parameters to jsonBody
            {
                model = "gpt-3.5-turbo",
                max_tokens = MaxTokens,
                stop = stop,
                messages = PreviousMessage
            };

            jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var result = await ConnectToOpenAPI(urlGPTOpenAI, jsonBody);            // get response from GPT
            textGPT.text = result["choices"][0]["message"]["content"].ToString();   // get the text from the response
        }

        if(MemorablePairs != 0)     // if memorize the past(history) messages
        {
            Dictionary<string, string> ResponseMessage = new Dictionary<string, string>
            {
                {"role", "assistant"},
                {"content", textGPT.text}
            };

            PreviousMessage.Add(ResponseMessage);

            if (PreviousMessage.Count >= 1 + MemorablePairs * 2)     // if the number of previous messages is over the limit
            {
                PreviousMessage.RemoveAt(1);    //remove the oldest message (user text)
                PreviousMessage.RemoveAt(1);    //remove the oldest message (agent text)
            }
        }else{
            PreviousMessage.RemoveAt(1);        //remove the message (user text)
        }
    }

    // prepare the input json body for DALLE -------------------------------------------------------------
    public async Task DALLECompletion(string text) 
    {
        // for Azure ++++++++++++++++++++++++++
        if(ButtonManipulator.source == "Azure")
        {
            var request = new
            {
                n = 1,
                prompt = text
            };

            jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var result = await ConnectToOpenAPI(urlDALLEAzure, jsonBody);   // get response from DALLE
            urlImage = result["result"][0]["data"][0]["url"].ToString();    // get the url from the response
        }
        // for OpenAI +++++++++++++++++++++++++++++++
        else if(ButtonManipulator.source == "OpenAI")
        {
            var request = new
            {
                n = 1,
                prompt = text
            };

            jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var result = await ConnectToOpenAPI(urlDALLEOpenAI, jsonBody);   // get response from DALLE
            urlImage = result["data"][0]["url"].ToString();                  // get the url from the response
        }

        var image = await HTTPRequest.GetImage(urlImage);   // get the image from the url
        imageDALLE.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);  // set the image to the UI
    }
}