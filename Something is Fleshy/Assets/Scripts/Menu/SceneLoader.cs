using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        print(sceneName);
        switch (sceneName)
        {
            case "LevelSelection":
                if (PlayersManager.instance.players[0] && PlayersManager.instance.players[1])
                {
                    PlayersManager.instance.inputMap.Menu.Disable();
                    goto default;
                }
                break;
            case "MainMenu":
                foreach (GameObject item in PlayersManager.instance.players)
                    Destroy(item);
                Destroy(PlayersManager.instance.gameObject);
                goto default;
            default:
                SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.MenuValidation, GameObject.FindGameObjectWithTag("UI_Master").GetComponent<AudioSource>());
                SceneManager.LoadScene(sceneName);
                break;
        }
    }
}
