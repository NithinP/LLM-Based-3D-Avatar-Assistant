using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Langchain_control : MonoBehaviour
{
    TextManager tm;
    WakeWordController wc;
    private string lastProcessedIntent = "";

    private const string apiKey = "AIzaSyCMqtqqn68ltAl_10WvEkvTSq4YsUJKCv0";
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key=";

    void Start()
    {
        wc = FindObjectOfType<WakeWordController>();
        tm = FindObjectOfType<TextManager>();
        if (tm == null)
        {
            Debug.LogError("TextManager not found!");
        }
        else
        {
            Debug.Log("TextManager found successfully");
        }
    }

    void Update()
    {
        if (tm != null && !string.IsNullOrEmpty(tm.intent) && tm.intent != lastProcessedIntent)
        {
            Debug.Log($"Received new intent: {tm.intent}");

            // Process the intent
            switch (tm.intent.ToLower())
            {
                case "audio_volume_other":
                    Debug.Log("Handling audio_volume_other intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "play_music":
                    // TODO: Implement play_music intent handling
                    break;

                case "iot_hue_lighton":
                    Debug.Log("Handling iot_hue_lighton intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "general_greet":

                    string response = MenuItem_KODEZ_Class.KODEZ(tm.currentReceivedText);
                    if(response != null)
                    {
                        Debug.Log($"Response: {response}");
                        animation_identifier(tm.currentReceivedText);
                    }
                    break;

                case "calendar_set":
                    Debug.Log("Handling calendar_set intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "audio_volume_down":
                    Debug.Log("Handling audio_volume_down intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "social_query":
                    Debug.Log("Handling social_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "audio_volume_mute":
                    Debug.Log("Handling audio_volume_mute intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "iot_wemo_on":
                    Debug.Log("Handling iot_wemo_on intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "iot_hue_lightup":
                    Debug.Log("Handling iot_hue_lightup intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "audio_volume_up":
                    Debug.Log("Handling audio_volume_up intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "iot_coffee":
                    Debug.Log("Handling iot_coffee intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "takeaway_query":
                    Debug.Log("Handling takeaway_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "qa_maths":
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "play_game":
                    Debug.Log("Handling play_game intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "cooking_query":
                    Debug.Log("Handling cooking_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "iot_hue_lightdim":
                    Debug.Log("Handling iot_hue_lightdim intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "iot_wemo_off":
                    Debug.Log("Handling iot_wemo_off intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "music_settings":
                    Debug.Log("Handling music_settings intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "weather_query":
                    Debug.Log("Handling weather_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "news_query":
                    Debug.Log("Handling news_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "alarm_remove":
                    Debug.Log("Handling alarm_remove intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "social_post":
                    Debug.Log("Handling social_post intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "recommendation_events":
                    Debug.Log("Handling recommendation_events intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "transport_taxi":
                    Debug.Log("Handling transport_taxi intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "takeaway_order":
                    Debug.Log("Handling takeaway_order intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "music_query":
                    Debug.Log("Handling music_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "calendar_query":
                    Debug.Log("Handling calendar_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "lists_query":
                    Debug.Log("Handling lists_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "qa_currency":
                    Debug.Log("Handling qa_currency intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "recommendation_movies":
                    Debug.Log("Handling recommendation_movies intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "general_joke":
                    Debug.Log("Handling general_joke intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "recommendation_locations":
                    Debug.Log("Handling recommendation_locations intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "email_querycontact":
                    Debug.Log("Handling email_querycontact intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "lists_remove":
                    Debug.Log("Handling lists_remove intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "play_audiobook":
                    Debug.Log("Handling play_audiobook intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "email_addcontact":
                    Debug.Log("Handling email_addcontact intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "lists_createoradd":
                    Debug.Log("Handling lists_createoradd intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "play_radio":
                    Debug.Log("Handling play_radio intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "qa_stock":
                    Debug.Log("Handling qa_stock intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "alarm_query":
                    Debug.Log("Handling alarm_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "email_sendemail":
                    Debug.Log("Handling email_sendemail intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "general_quirky":
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "music_likeness":
                    Debug.Log("Handling music_likeness intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "cooking_recipe":
                    Debug.Log("Handling cooking_recipe intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "email_query":
                    Debug.Log("Handling email_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "datetime_query":
                    Debug.Log("Handling datetime_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "transport_traffic":
                    Debug.Log("Handling transport_traffic intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "play_podcasts":
                    Debug.Log("Handling play_podcasts intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "iot_hue_lightchange":
                    Debug.Log("Handling iot_hue_lightchange intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "calendar_remove":
                    Debug.Log("Handling calendar_remove intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "transport_query":
                    Debug.Log("Handling transport_query intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "transport_ticket":
                    Debug.Log("Handling transport_ticket intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "qa_factoid":
                    Debug.Log("Handling qa_factoid intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "iot_cleaning":
                    Debug.Log("Handling iot_cleaning intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "alarm_set":
                    Debug.Log("Handling alarm_set intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "datetime_convert":
                    Debug.Log("Handling datetime_convert intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "iot_hue_lightoff":
                    Debug.Log("Handling iot_hue_lightoff intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "qa_definition":
                    Debug.Log("Handling general_greet intent");
                    animation_identifier(tm.currentReceivedText);
                    break;

                case "music_dislikeness":
                    Debug.Log("Handling music_dislikeness");
                    animation_identifier(tm.currentReceivedText);
                    break;

                default:
                    Debug.Log("Unknown Intent");
                    break;
            }

            lastProcessedIntent = tm.intent;
        }
    }
    public void animation_identifier(string msg)
    {
        string prompt = @"You are an AI animation suggestion system. Analyze the message and suggest ONE appropriate animation name based on its emotional context, action, or intent. 
                        Guidelines:
                        1. Respond with ONLY the animation name in lowercase, no extra text
                        2. For greetings/emotions, suggest animations like:
                           - wave, bow, nod (for greetings)
                           - jump, dance, celebrate (for joy/excitement)
                           - cry, sob (for sadness)
                           - angry, rage (for anger)
                           - laugh (for humor)
                           - shrug (for confusion/uncertainty)
                        3. For actions/commands, suggest literal animations like:
                           - walk, run, sprint
                           - sit, stand, crouch
                           - attack, defend, block
                           - eat, drink, sleep
                           - pickup, throw, drop
                        4. For unclear messages, suggest 'idle'

                        The message is: """ + msg + @"""";

        StartCoroutine(QueryAPI(prompt, response =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    JsonData responseData = JsonUtility.FromJson<JsonData>(response);
                    if (responseData?.candidates != null &&
                        responseData.candidates.Length > 0 &&
                        responseData.candidates[0]?.content?.parts != null &&
                        responseData.candidates[0].content.parts.Length > 0)
                    {
                        string animationName = responseData.candidates[0].content.parts[0].text.Trim();
                        PlayAnimation(animationName);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing response: {e.Message}");
                }
            }
        }));
    }

    private void PlayAnimation(string animationName)
    {
        if (wc.characterAnimator != null)
        {
            switch (animationName.ToLower())
            {
                case "wave":
                    wc.characterAnimator.applyRootMotion = true;
                    wc.characterAnimator.SetTrigger("wave");
                    break; 
                default:
                    wc.characterAnimator.SetTrigger("Idle");
                    break;
            }

            Debug.Log($"Playing animation: {animationName}");
        }
        else
        {
            Debug.LogError("Animator not found!");
        }
    }

    private IEnumerator QueryAPI(string message, System.Action<string> onResponse)
    {
        string fullUrl = apiUrl + apiKey;
        RequestData requestData = new RequestData
        {
            contents = new[] {
                new Content {
                    parts = new[] { new Part { text = message } }
                }
            }
        };

        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest webRequest = new UnityWebRequest(fullUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                onResponse?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"API Request Failed: {webRequest.error}");
                onResponse?.Invoke(null);
            }
        }
    }

    // JSON Classes
    [System.Serializable]
    private class RequestData
    {
        public Content[] contents;
    }

    [System.Serializable]
    private class JsonData
    {
        public Candidate[] candidates;
    }

    [System.Serializable]
    private class Candidate
    {
        public Content content;
    }

    [System.Serializable]
    private class Content
    {
        public Part[] parts;
        public string role;
    }

    [System.Serializable]
    private class Part
    {
        public string text;
    }
}
