using UnityEngine;

//Создаем новый скипрт, который можно создать в юнити через пкм+ добавить обьект
[CreateAssetMenu(menuName = "Dialog/DialogueObject")]

public class DialogueObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;

    //A getter. Позволит другим функциям читать диалоги, но не даст их изменять.
    public string[] Dialogue => dialogue;

    public bool HasResponses => Responses != null && Responses.Length > 0;

    public Response[] Responses => responses;
}
