using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static async void Restart(float delay = 0)
    {
        await Task.Delay(Mathf.RoundToInt(delay * 1000));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
