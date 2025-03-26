using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    //Загрузка следующего уровня
    public void LoadNextLevel ()
    {
        
        //Загружаем следующую сцену (прописанную в build settings) 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}