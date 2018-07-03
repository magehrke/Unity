using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Network : NetworkManager
{
     //Assign a Text component in the GameObject's Inspector
    public Text entryText;
    public GameObject ball;

    //Detect when a client connects to the Server
    public override void OnClientConnect(NetworkConnection connection)
    {
		entryText = GameObject.FindGameObjectWithTag ("EntryText").GetComponent<Text>();

		ClientScene.Ready (connection);
		ClientScene.AddPlayer (0);
//        ball.GetComponent<BallControll>().CmdcreateBall();
        //Change the text to show the connection on the client side
        Debug.Log("Ein Spieler hat sich verbunden!");
		entryText.GetComponent<EntryTextScript>().CmdChangeTextMessage("Ein Spieler hat sich verbunden!");

		Invoke ("DisableText", 1.5f);

    }


    //Detect when a client connects to the Server
    public override void OnClientDisconnect(NetworkConnection connection)
    {

		entryText = GameObject.FindGameObjectWithTag ("EntryText").GetComponent<Text>();

        ball.GetComponent<BallControll>().CmddestroyBall();
        //Change the text to show the connection loss on the client side
        Debug.Log("Ein Spieler hat das Spiel verlassen!");
		entryText.GetComponent<EntryTextScript>().CmdChangeTextMessage("Ein Spieler hat das Spiel verlassen!");
		Invoke ("DisableText", 1.5f);
    }

	void DisableText() {
		if (entryText != null) {
			entryText.GetComponent<EntryTextScript>().CmdChangeTextMessage("");
		}
	}
    
    public override void OnServerConnect(NetworkConnection connection)
    {
        
        if(numPlayers == 1)
        {
			ball = GameObject.FindGameObjectWithTag ("StartBall");
            ball.GetComponent<BallControll>().CmdcreateBall();
        }
    }
}
