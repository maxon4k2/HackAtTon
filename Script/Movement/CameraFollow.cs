using UnityEngine;

// Скрипт позволяющий камере двигаться за игроком (кубиком)
public class CameraFollow : MonoBehaviour
{
    //Добавляет окно скрипта, куда можно вложить какой либо обьект, в нашем случае игрок. Далее это позволит менять его позицию
    public Transform player;
    
    //Добавляет окно скрипта, позволяющее внести векторные в 3d значения по XYZ, в нашем случае будем использовать для удержания камеры на одном расстоянии от игрока
    public Vector3 offset;
    
    // Update is called once per frame
    void Update()
    {
        //transform - параметр, позволяющий изменять параметры объекта, в котором он находиться, в нашем случае камера
        transform.position = player.position + offset;
    }
}
