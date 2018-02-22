using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PanelMgr.instance.OpenPanel<LoginPanel>("LoginPanel");
        Application.runInBackground = true;
        Screen.SetResolution(960, 540, false);
    }
	
	// Update is called once per frame
	void Update () {
        NetMgr.Update();
	}
}
