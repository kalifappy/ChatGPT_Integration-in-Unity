using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiscussionManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private DiscussionBubble bubblePrefab;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Transform bubbleParent;


    public static event Action onMessageReceived;

    [Header(" Authentication ")]
    [SerializeField] private string apiKey;
    [SerializeField] private string organisationKey;
    private OpenAIClient api;

    [Header(" Settings ")]
    private List<Message> messages = new List<Message>();

    private void Start()
    {
        Authenticate();
        CreateBubble("Hey there! How can I help you?", false);

        // Initialize chat history
        messages.Add(new Message(Role.System, "You are a chatGPT premium model"));
       
    }

    private void Authenticate()
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(organisationKey))
        {
            Debug.LogError("API Key or Organisation Key is missing!");
            return;
        }

        api = new OpenAIClient(new OpenAIAuthentication(apiKey, organisationKey));
    }

    public async void AskButtonCallback()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        string userMessage = inputField.text;
        CreateBubble(userMessage, true); // Show user message
        inputField.text = ""; // Clear input field after storing the message

        messages.Add(new Message(Role.User, userMessage));

        await SendMessageToAI();
    }

    private async System.Threading.Tasks.Task SendMessageToAI()
    {
        ChatRequest request = new ChatRequest(messages: messages, model: OpenAI.Models.Model.GPT4oMini, temperature: 1.4f);

        try
        {
            var result = await api.ChatEndpoint.GetCompletionAsync(request);
            string aiResponse = result.FirstChoice.Message.ToString();

            messages.Add(new Message(Role.Assistant, aiResponse)); // Save AI response
            StartCoroutine(TypeText(aiResponse)); // Animate word-by-word typing
        }
        catch (Exception e)
        {
            Debug.LogError("OpenAI API Error: " + e.Message);
            CreateBubble("Error: Unable to retrieve response.", false);
        }
    }

    private void CreateBubble(string message, bool isUserMessage)
    {
        DiscussionBubble discussionBubble = Instantiate(bubblePrefab, bubbleParent);
        discussionBubble.Configure(message, isUserMessage);
        onMessageReceived?.Invoke();
    }

    private IEnumerator TypeText(string fullText)
    {
        DiscussionBubble bubble = Instantiate(bubblePrefab, bubbleParent);
        bubble.Configure("", false); // Create an empty bubble to type into

        string currentText = "";
        string[] words = fullText.Split(' ');

        foreach (string word in words)
        {
            currentText += word + " ";
            bubble.Configure(currentText, false); // Update text dynamically
            yield return new WaitForSeconds(0.1f); // Adjust speed for effect

            onMessageReceived?.Invoke(); // Call scrolling event while typing
        }
    }
}
