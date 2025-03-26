using UnityEngine;

public class Player_movement : MonoBehaviour
{

    [SerializeField] private DialogueUI dialogueUI;
    
    public float moveSpeed = 5f;

    public DialogueUI DialogueUI => dialogueUI;

    public IInteracteble Interacteble { get; set; }

    public Rigidbody2D rb;
    public Animator animator;

    Vector2 movement;

    // Update is for input
    private void Update()
    {   
        if (DialogueUI.IsOpen)
        {
            animator.SetFloat("Speed", 0);
            return;
        } 
        movement.x = Input.GetAxisRaw("Horizontal"); //Gets a value -1 if left and +1 if right. Works for a and d and arrow keys.
        movement.y = Input.GetAxisRaw("Vertical"); 

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude); //Magnitude (скорость) sqrMagnitude - типо лучше)

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Interacteble != null)
            {
                Interacteble.Interact(player:this);
            }
        }

    }
    
    // FixedUpdate is for Movement
    private void FixedUpdate()
    {
        if (DialogueUI.IsOpen) return;
        
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        //MovePosition - двигает указаный rigidbody. 
        //rb.position - текущее положение нашего персонажа в пространстве
        //movement - моя переменная отвечающая за направление движения
        //moveSpeed - моя переменная отвечающая за скорость передвижения
        //Time.fixedDeltaTime - время до того как последняя функция была вызвана??? Позволяет получить постоянную скорость передвижения??? (Мб убирает зависимость от лагов?)
    }
}
