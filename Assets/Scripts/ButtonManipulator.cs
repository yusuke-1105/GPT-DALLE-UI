using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ButtonManipulator : MonoBehaviour
{
    public Button button;                    // the button to choose the feature(GPT or DALLE)
    public TextMeshProUGUI buttonLeftText;   // the text of the button(GPT)
    public TextMeshProUGUI buttonRightText;  // the text of the button(DALLE)

    // カプセルの移動に関する変数
    public RectTransform capsule;
    float moveDistance = 100f;               // how far the capsule moves
    float moveDuration = 0.5f;               // how long the capsule moves
    float anchorNum = 0f;                    // the direction of the capsule movement

    [SerializeField]
    public string feature = "GPT";           // GPT or DALLE
    public GameObject gameObjectGPT;         // gameobject for presenting GPT output
    public GameObject gameObjectDALLE;       // gameobject for presenting DALLE output

    [SerializeField]
    public string source = "Azure";          // the source server of GPT and DALLE
    public GameObject gameObjEmojiAzure;     // Emoji gameobject for Azure
    public GameObject gameObjEmojiOpenAI;    // Emoji gameobject for OpenAI

    public TMP_InputField inputField;        // get the input text from the user

    AzureOpenAI AzureOpenAI;
    RotateEmoji RotateEmojiAzure;            // instance of RotateEmoji.cs for Azure Emoji
    RotateEmoji RotateEmojiOpenAI;           // instance of RotateEmoji.cs for OpenAI Emoji

    private void Start()
    {
        button.onClick.AddListener(() => StartCoroutine(SmoothMove()));
        gameObjectDALLE.SetActive(false);      // not shown at the beginning
        gameObjEmojiOpenAI.SetActive(false);   // not shown at the beginning

        AzureOpenAI = GetComponent<AzureOpenAI>();
        RotateEmojiAzure = gameObjEmojiAzure.transform.Find("Emoji").GetComponent<RotateEmoji>();
        RotateEmojiOpenAI = gameObjEmojiOpenAI.transform.Find("Emoji").GetComponent<RotateEmoji>();
    }

    // Smoothly move the capsule ---------------------------------------------------------------------------------
    private IEnumerator SmoothMove()
    {
        // the black capsule of FeatureSelection moves according to the color of text(GPT)
        // if the color of text(GPT) is white, then the capsule moves to the left. Otherwise, it moves to the right
        Color colorLeft = buttonLeftText.color;

        if(Mathf.Sign(colorLeft.r - 0.5f) < 0)
        {
            feature = "GPT";
            anchorNum = -1f;
            gameObjectGPT.SetActive(true);
            gameObjectDALLE.SetActive(false);
        }else{
            feature = "DALLE";
            anchorNum = 1f;
            gameObjectGPT.SetActive(false);
            gameObjectDALLE.SetActive(true);
        }

        Vector2 initialPosition = capsule.anchoredPosition;
        Vector2 targetPosition = initialPosition + new Vector2(anchorNum * moveDistance, 0);

        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            // the movement of the capsule is not linear but smooth(starting and stopping)
            t = t * t * (3f - 2f * t);

            capsule.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, t);

            // get the color of the text of buttonLeftText and change the color to the opposite color
            buttonLeftText.color = Color.Lerp(colorLeft, new Color(1 - colorLeft.r, 1 - colorLeft.g, 1 - colorLeft.b), t * Mathf.Sign(1 - colorLeft.r));
            // the color of "GPT" and "DALLE" are always opposite
            buttonRightText.color = Color.Lerp(new Color(1 - colorLeft.r, 1 - colorLeft.g, 1 - colorLeft.b), colorLeft, t * Mathf.Sign(colorLeft.r));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        capsule.anchoredPosition = targetPosition;
    }

    // Change the source server of GPT and DALLE ------------------------------------------------------------------
    public void ChangeSource()
    {
        if(source == "Azure")
        {
            gameObjEmojiAzure.SetActive(false);
            gameObjEmojiOpenAI.SetActive(true);
            source = "OpenAI";
        }else{
            gameObjEmojiAzure.SetActive(true);
            gameObjEmojiOpenAI.SetActive(false);
            source = "Azure";
        }
        Debug.Log(source);
    }

    // Send Command to one Emoji gameobject -----------------------------------------------------------------------
    public void CommandToEmoji()
    {
        if(source == "Azure") RotateEmojiAzure.RotationHandler();
        else if(source == "OpenAI") RotateEmojiOpenAI.RotationHandler();
    }

    // if the submit button is pressed, then call the completion function ----------------------------------------
    public void SubmitButtonPressed()
    {
        string text = inputField.text;
        if(feature == "GPT") AzureOpenAI.GPTCompletion(text);
        else if(feature == "DALLE") AzureOpenAI.DALLECompletion(text);
    }
}
