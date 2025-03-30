using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;
    
    // This text displays the timer.
    [SerializeField] private TMP_Text TimerDisplay;
    // Initialize the timer to 11:00 (11 hours, 0 minutes, 0 seconds).
    private TimeSpan currentTime = new TimeSpan(11, 0, 0);

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents;

    private List<GameObject> tempResponseButtons = new List<GameObject>();

    // Keeps track of responses that have been clicked.
    private static HashSet<string> clickedResponseKeys = new HashSet<string>();

    // Define the original and clicked colors. 
    private Color originalColor = new Color(170f / 255f, 62f / 255f, 202f / 255f, 1f); // Full.
    private Color clickedColor = new Color(170f / 255f, 62f / 255f, 202f / 255f, 0.3f); // Semi-transparent.
    private Color currentColor = new Color(170f / 255f, 62f / 255f, 202f / 255f, 0.3f); // Current color

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
        UpdateTimerDisplay();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    public void ShowResponse(Response[] responses)
    {
        float responseBoxHeight = 40f;

        foreach (Response response in responses)
        {
            currentColor = originalColor;
            int responseIndex = Array.IndexOf(responses, response);
            string key = response.GetHashCode() + "_" + responseIndex;

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            
            string displayText = response.ResponseText;
            if (displayText.Contains("[+15 мин]") && clickedResponseKeys.Contains(key))
            {
                displayText = displayText.Replace("[+15 мин]", "");
            }

            if (clickedResponseKeys.Contains(key))
            {
                currentColor = clickedColor;
            }
            
            TMP_Text buttonText = responseButton.GetComponent<TMP_Text>();
            buttonText.text = responseIndex + 1 + ") - " + displayText;
            buttonText.color = currentColor; // Set the color.

            Button buttonComponent = responseButton.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => OnPickedResponse(response, responseIndex, displayText, key, buttonText));

            tempResponseButtons.Add(responseButton);

            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex, string responseText, string key, TMP_Text buttonText)
    {
        // If the response contains "[+15 мин]" and hasn't been clicked, add 15 minutes.
        if (responseText.Contains("[+15 мин]") && !clickedResponseKeys.Contains(key))
        {
            currentTime = currentTime.Add(TimeSpan.FromMinutes(15));
            UpdateTimerDisplay();
            clickedResponseKeys.Add(key);
        }
        if (!clickedResponseKeys.Contains(key))
        {
            clickedResponseKeys.Add(key);
        }

        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        if (responseEvents != null && responseIndex < responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }
        responseEvents = null;
        
        if (response.DialogueObject)
        {
            dialogueUI.ShowDialogue(response.DialogueObject);
        }
        else
        {
            dialogueUI.CloseDialogueBox();
        }
    }

    // Updates the timer display text in hh:mm format.
    private void UpdateTimerDisplay()
    {
        TimerDisplay.text = "Время: " + currentTime.ToString(@"hh\:mm");
    }

    public void Interact(Player_movement player)
    {
        throw new System.NotImplementedException();
    }
}
