[System.Serializable]
public class SigninResponse
{
    public bool success;
    public UserData result;
    public string token;

    public bool isOnboarded;
    public string msg;
}
