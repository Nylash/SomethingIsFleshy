using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager instance;
    public GameObject[] players = new GameObject[2];
    public CustomizationMap inputMap;

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

        inputMap.Menu.Start.started += ctx => GetComponent<SceneLoader>().LoadScene("LevelSelection");
        inputMap.Menu.Back.started += ctx => GetComponent<SceneLoader>().LoadScene("MainMenu");
    }

    private void OnEnable() => inputMap.Menu.Enable();
    private void OnDisable() => inputMap.Menu.Disable();

    void OnPlayerJoined(PlayerInput player)
    {
        if(players[0] == null)
        {
            players[0] = player.gameObject;
            player.gameObject.GetComponent<CharacterController2D>().playerNumber = 1;
            player.transform.position = new Vector3(-5, 2, 0);
        }
        else
        {
            player.gameObject.GetComponent<PlayerCustomization>().SetColor(Color.green);
            player.gameObject.GetComponent<CharacterController2D>().playerNumber = 2;
            player.transform.position = new Vector3(5, 2, 0);
            players[1] = player.gameObject;
            GetComponent<PlayerInputManager>().DisableJoining();
        }
        DontDestroyOnLoad(player.gameObject);
    }
}
