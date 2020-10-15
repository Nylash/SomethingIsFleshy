using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager instance;
    public GameObject[] players = new GameObject[2];
    public CustomizationMap inputMap;
    public GameMode currentGameMode;
    public bool switchingPlayer;
    public GameObject DEBUGplayerPrefab;
    public bool DEBUG;
    public Canvas P1canvas;
    public Canvas P2canvas;

    public enum GameMode
    { 
        OnePlayer, TwoPlayers       
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            foreach (GameObject item in instance.players)
                Destroy(item);
            Destroy(instance.gameObject);
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        inputMap = new CustomizationMap();

        if (!DEBUG)
        {
            inputMap.Menu.Start.started += ctx => GetComponent<SceneLoader>().LoadScene("LevelSelection");
            inputMap.Menu.Back.started += ctx => GetComponent<SceneLoader>().LoadScene("MainMenu");

            if (currentGameMode == GameMode.OnePlayer)
            {
                players[0].gameObject.GetComponent<CharacterController2D>().playerNumber = 1;
                players[1].gameObject.GetComponent<CharacterController2D>().playerNumber = 2;
                players[1].gameObject.GetComponent<PlayerCustomization>().SetColor(Color.green);
                foreach (GameObject item in players)
                    DontDestroyOnLoad(item);
            }
        }
    }

    private void OnEnable() => inputMap.Menu.Enable();
    private void OnDisable() => inputMap.Menu.Disable();

    void OnPlayerJoined(PlayerInput player)
    {
        if(players[0] == null)
        {
            players[0] = player.gameObject;
            player.gameObject.GetComponent<CharacterController2D>().playerNumber = 1;
            player.transform.position = new Vector3(-3.5f, 2, 0);
            P1canvas.enabled = false;
        }
        else
        {
            player.gameObject.GetComponent<PlayerCustomization>().SetColor(Color.green);
            player.gameObject.GetComponent<CharacterController2D>().playerNumber = 2;
            player.transform.position = new Vector3(3.5f, 2, 0);
            players[1] = player.gameObject;
            GetComponent<PlayerInputManager>().DisableJoining();
            P2canvas.enabled = false;
        }
        DontDestroyOnLoad(player.gameObject);
    }

    public IEnumerator ResetSwitch()
    {
        yield return new WaitForSeconds(.25f);
        switchingPlayer = false;
    }
}
