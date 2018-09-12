using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadFromLocationID : MonoBehaviour {



	public Text fieldName;
	public WorldMapManager worldMan;


	public void Setup(string saveName, WorldMapManager worldManager) {

		fieldName.text = saveName;
		worldMan = worldManager;

	}

	public void Load() {

		worldMan.LoadWithNewName (fieldName.text);


	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
