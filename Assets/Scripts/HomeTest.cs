using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeScript : MonoBehaviour
{
    public TextMeshProUGUI welcomeText;

    void Start()
    {
        // Retrieve user data from PlayerPrefs
        UserData user = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString("UserData"));// "Guest" is the default if no user data is found

        // Update the UI with the user's name
        welcomeText.text = "Welcome, " + user.name + "!";

        // You can also use the token for any authenticated requests if needed
    }

    // Rest of your "Home" scene code
}
