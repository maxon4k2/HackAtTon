using Unity.VisualScripting;
using System.Collections;
using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteracteble
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private GameObject HideUITalk;
    // Reference to the image on this character (assign via Inspector)
    [SerializeField] private GameObject characterImage;
     

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        Debug.Log($"DialogueActivator updating dialogue from {this.dialogueObject?.name} to {dialogueObject?.name}");
        this.dialogueObject = dialogueObject;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        HideUITalk.SetActive(true);
        
        if (other.CompareTag("Player") && other.TryGetComponent(out Player_movement player))
        {
            player.Interacteble = this;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        HideUITalk.SetActive(false);
        
        if (other.CompareTag("Player") && other.TryGetComponent(out Player_movement player))
        {
            if (player.Interacteble is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interacteble = null;
            }
        }
    }

    public void Interact(Player_movement player)
    {
        // Activate the character's dialogue image
        characterImage.SetActive(true);

        // Set this activator as the current one
        player.DialogueUI.GetComponent<ResponseHandler>().SetCurrentDialogueActivator(this);

        // Find the matching DialogueResponseEvents for the initial dialogue
        DialogueResponseEvents[] responseEvents = GetComponents<DialogueResponseEvents>();
        foreach (var events in responseEvents)
        {
            if (events.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(events.Events);
                break;
            }
        }

        player.DialogueUI.ShowDialogue(dialogueObject);
        StartCoroutine(DisableImageAfterDialogue(player));
    }

    private IEnumerator DisableImageAfterDialogue(Player_movement player)
    {
        // Wait until the dialogue is closed
        yield return new WaitUntil(() => !player.DialogueUI.IsOpen);
        // Deactivate the character's dialogue image
        characterImage.SetActive(false);
    }
}
