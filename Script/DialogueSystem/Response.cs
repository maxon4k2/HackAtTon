using UnityEngine;

[System.Serializable]

public class Response 
{
    [SerializeField] public string responseText;
    [SerializeField] private DialogueObject dialogueObject;

    public string ResponseText => responseText;

    public DialogueObject DialogueObject => dialogueObject;
}
