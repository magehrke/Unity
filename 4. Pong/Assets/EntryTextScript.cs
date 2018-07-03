using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EntryTextScript : NetworkBehaviour {

	[SyncVar]
	public string msg = "";

	void Update() {
		this.GetComponent<Text> ().text = msg;
	}
		
	[Command]
	public void CmdChangeTextMessage(string new_msg) {
		msg = new_msg;
	}
}
