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
    private Color currentColor = new Color(170f / 255f, 62f / 255f, 202f / 255f, 0.3f); // Current color (!!!Maybe to delete)
    private Color ContinuebuttonColor = new Color(34f / 255f, 139f / 255f, 34f / 255f, 1f); //Green color

    private DialogueActivator currentDialogueActivator; // Add this field

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
        UpdateTimerDisplay();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    public void SetCurrentDialogueActivator(DialogueActivator activator)
    {
        currentDialogueActivator = activator;
    }

    public void ShowResponse(Response[] responses)
    {
        float responseBoxHeight = 40f;

        for (int i = 0; i < responses.Length; i++)
        {
            Response response = responses[i];
            int responseIndex = i;
            // Generate a unique key using the response's hash code and the loop index.
            string key = response.GetHashCode() + "_" + i;
            // Create a display version that may have "[+15 мин]" removed if this unique key was clicked.
            string displayText = response.ResponseText;
            Color buttonColor = originalColor;

            //Удаляет [+15 мин] из Response если он уже был нажат и делает текст прозрачным
            if (displayText.Contains("[+15 мин]") && clickedResponseKeys.Contains(key))
            {
                displayText = displayText.Replace("[+15 мин]", "");
                buttonColor = clickedColor;
            }

            //Меняет цвет кнопки [Продолжить..]
            if (response.ResponseText.Contains("[Продолжить..]"))
            {
                buttonColor = ContinuebuttonColor;
            }

            // Instantiate and set up the response button.
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.SetActive(true);
            TMP_Text buttonText = responseButton.GetComponent<TMP_Text>();
            buttonText.text = i + 1 + ") - " + displayText;
            buttonText.color = buttonColor;

            Button buttonComponent = responseButton.GetComponent<Button>();
            // Note: Now we pass only four arguments, matching your OnPickedResponse signature.
            buttonComponent.onClick.AddListener(() => OnPickedResponse(response, responseIndex, key));

            tempResponseButtons.Add(responseButton);
            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex, string key)
    {
        // Handle time-related responses
        if (response.ResponseText.Contains("[+15 мин]") && !clickedResponseKeys.Contains(key))
        {
            currentTime = currentTime.Add(TimeSpan.FromMinutes(15));
            UpdateTimerDisplay();
            clickedResponseKeys.Add(key);
        }

        if (!clickedResponseKeys.Contains(key))
        {
            clickedResponseKeys.Add(key);
        }

        // Execute current response events FIRST
        if (responseEvents != null && responseIndex < responseEvents.Length)
        {
            Debug.Log($"Executing response event at index {responseIndex}");
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        // Clear buttons and box
        responseBox.gameObject.SetActive(false);
        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        // Handle dialogue transition
        if (response.DialogueObject)
        {
            string currentDialogueName = response.DialogueObject.name;
            Debug.Log($"Moving to DialogueObject: {currentDialogueName}");

            // Find the DialogueResponseEvents for the next dialogue
            DialogueResponseEvents[] allEvents = currentDialogueActivator.GetComponents<DialogueResponseEvents>();
            foreach (var events in allEvents)
            {
                if (events.DialogueObject == response.DialogueObject)
                {
                    Debug.Log($"Found events for {currentDialogueName}, updating responses");
                    dialogueUI.AddResponseEvents(events.Events);
                    break;
                }
            }

            // Show the dialogue
            dialogueUI.ShowDialogue(response.DialogueObject);
        }
        else
        {
            responseEvents = null;
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
