using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        switch (sceneName)
        {
            case "PlayerSelection":
                if(PlayersManager.instance.currentGameMode == PlayersManager.GameMode.OnePlayer)
                {
                    sceneName = "1PSelection";
                    foreach (GameObject item in PlayersManager.instance.players)
                        Destroy(item);
                    Destroy(PlayersManager.instance.gameObject);
                    goto default;
                }
                else
                {
                    sceneName = "2PSelection";
                    foreach (GameObject item in PlayersManager.instance.players)
                        Destroy(item);
                    Destroy(PlayersManager.instance.gameObject);
                    goto default;
                }
            case "LevelSelection":
                if (PlayersManager.instance.players[0] && PlayersManager.instance.players[1])
                {
                    PlayersManager.instance.inputMap.Menu.Disable();
                    foreach (var item in PlayersManager.instance.players)
                        item.GetComponent<PlayerInput>().notificationBehavior = PlayerNotifications.SendMessages;
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
