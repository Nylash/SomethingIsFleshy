public class LungsManager : PrimarySystem
{
    public static LungsManager instance;

    protected void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject); 
    }

    public override void StartLowRessource()
    {
        throw new System.NotImplementedException();
    }
}
