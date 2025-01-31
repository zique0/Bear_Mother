using UnityEngine;

public class SceneLoadTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
