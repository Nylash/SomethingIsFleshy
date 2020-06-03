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
}
