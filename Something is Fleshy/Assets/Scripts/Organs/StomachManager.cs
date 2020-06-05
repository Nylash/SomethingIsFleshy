public class StomachManager : PrimarySystem
{
    public static StomachManager instance;

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
        colorPipeOpen = GameManager.instance.energyPipeOpenColor;
        colorPipeClose = GameManager.instance.energyPipeCloseColor;
        base.Start();
    }
}
