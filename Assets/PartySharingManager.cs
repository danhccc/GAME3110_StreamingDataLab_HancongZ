using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartySharingManager : MonoBehaviour
{

    public GameObject sharePartyButton, sharingRoomNameInputField, joinSharingRoonButton;

    NetworkedClient networkedClient;

    // Start is called before the first frame update
    void Start()
    {
        sharePartyButton.GetComponent<Button>().onClick.AddListener(SharePartyButtonPressed);
        joinSharingRoonButton.GetComponent<Button>().onClick.AddListener(joinSharingRoonButtonPressed);

        networkedClient = GetComponent<NetworkedClient>();
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void joinSharingRoonButtonPressed()
    {
        string name = sharingRoomNameInputField.GetComponent<InputField>().text;

        networkedClient.SendMessageToHost(ClientToServerSignifier.JoinSharingRoom + "," + name);
    }

    private void SharePartyButtonPressed()
    {
        AssignmentPart2.SendPartyDataToSever(networkedClient);

    }
}
