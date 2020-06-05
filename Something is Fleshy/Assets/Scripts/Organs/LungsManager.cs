public class LungsManager : PrimarySystem
{
    public static LungsManager instance;

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject); 
    }

    protected override void Start()
    {
        colorPipeOpen = GameManager.instance.oxygenPipeOpenColor;
        colorPipeClose = GameManager.instance.oxygenPipeCloseColor;
        base.Start();
    }
}
