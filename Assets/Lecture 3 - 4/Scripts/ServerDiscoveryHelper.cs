using Mirror.Discovery;
using Mirror_Tanks;
using UnityEngine;

public class ServerDiscoveryHelper : MonoBehaviour
{
    [SerializeField] private MainMenuUI mainMenuUI;

    [SerializeField] private NetworkDiscovery networkDiscovery;

    private bool serverFound = false;

    private void Awake() => networkDiscovery.OnServerFound.AddListener(OnServerFound);

    private void OnServerFound(ServerResponse response)
    {
        Debug.Log("Server found: " + response.uri);
        serverFound = true;
    }

    public void StartAsHostOrClient()
    {
        networkDiscovery.StartDiscovery();
        Invoke(nameof(CheckDiscoveredServers), 1f);
    }

    private void CheckDiscoveredServers()
    {
        if (serverFound)
        {
            Debug.Log("Joining as a client");
            mainMenuUI.OnStartClientClicked();
        }
        else
        {
            Debug.Log("Starting as a host");
            mainMenuUI.OnStartHostClicked();
        }
    }
}