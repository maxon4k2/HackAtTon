using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;
    
    [SerializeField] private TMP_Text MovesLeft;
    public float NumberMovesLeft = 40;

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents;

    private List<GameObject> tempResponseButtons = new List<GameObject>();

    // Keeps track of responses for which the move cost has already been applied.
    private static HashSet<string> spentMoveKeys = new HashSet<string>();

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
        MovesLeft.text = "Осталось ходов: " + NumberMovesLeft;
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    public void ShowResponse(Response[] responses)
    {
        float responseBoxHeight = 40f;

        for (int i = 0; i < responses.Length; i++)
        {
            Response response = responses[i];
            int responseIndex = i;
            // Create a unique key for this response option.
            // (This assumes that response.GetHashCode() is sufficiently unique along with its index.)
            string key = response.GetHashCode() + "_" + i;

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);

            // Prepare the display text.
            string displayText = response.ResponseText;
            // If the option contains the move cost caption and the move has been spent before, remove the caption.
            if (displayText.Contains("[-1 Ход]") && spentMoveKeys.Contains(key))
            {
                displayText = displayText.Replace("[-1 Ход]", "");
            }
            responseButton.GetComponent<TMP_Text>().text = displayText;
              
            // When clicking, pass along the key so that we can check whether the move has been spent.
            responseButton.GetComponent<Button>().onClick.AddListener(() => 
                OnPickedResponse(response, responseIndex, displayText, key)
            );

            tempResponseButtons.Add(responseButton);

            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex, string responseText, string key)
    {
        // If this response has the caption and the move hasn't been spent yet, subtract one move.
        if (responseText.Contains("[-1 Ход]") && !spentMoveKeys.Contains(key))
        {
            NumberMovesLeft--;
            MovesLeft.text = "Осталось ходов: " + NumberMovesLeft;
            spentMoveKeys.Add(key);
        }
        
        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        if (responseEvents != null && responseIndex <= responseEvents.Length)
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

    public void Interact(Player_movement player)
    {
        throw new System.NotImplementedException();
    }
}
