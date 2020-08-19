public class StomachManager : PrimarySystem
{
    public static StomachManager instance;

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
}
