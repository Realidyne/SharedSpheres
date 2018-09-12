using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

using UnityEngine.UI;	

public class WorldMapManager : MonoBehaviour
{
    [SerializeField]
    UnityARCameraManager m_ARCameraManager;

    ARWorldMap m_LoadedMap;

	serializableARWorldMap serializedWorldMap;



    // Use this for initialization
    void Start ()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += OnFrameUpdate;
    }

    ARTrackingStateReason m_LastReason;

    void OnFrameUpdate(UnityARCamera arCamera)
    {
        if (arCamera.trackingReason != m_LastReason)
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Debug.LogFormat("worldTransform: {0}", arCamera.worldTransform.column3);
            Debug.LogFormat("trackingState: {0} {1}", arCamera.trackingState, arCamera.trackingReason);
            m_LastReason = arCamera.trackingReason;
        }
    }

    static UnityARSessionNativeInterface session
    {
        get { return UnityARSessionNativeInterface.GetARSessionNativeInterface(); }
    }

	string WorldName;

	public InputField worldInput;

	public List<Button> SaveFileButtons; 

    string path
    {
		get { return Path.Combine(Application.persistentDataPath, WorldName+".worldmap"); }
    }

    void OnWorldMap(ARWorldMap worldMap)
    {
        if (worldMap != null)
        {
            worldMap.Save(path);
            Debug.LogFormat("ARWorldMap saved to {0}", path);
        }
    }

	public List<string> levelNames;

    public void Save()
    {
		
        session.GetCurrentWorldMapAsync(OnWorldMap);



    }

	public void ResetScene() {
		ARKitWorldTrackingSessionConfiguration sessionConfig = new ARKitWorldTrackingSessionConfiguration ( UnityARAlignment.UnityARAlignmentGravity, UnityARPlaneDetection.HorizontalAndVertical);
		UnityARSessionNativeInterface.GetARSessionNativeInterface().RunWithConfigAndOptions(sessionConfig, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking);
	}


	public GameObject menuObject;

	public GameObject MenuHolder;

	public List<GameObject> menuList = new List<GameObject>();
	public void PopulateLoadMenu() {

		for (int i = 0; i < menuList.Count; i++) {
			Destroy (menuList [i]);




		}
		menuList.Clear ();


		for (int i = 0; i < levelNames.Count; i++) {
			GameObject menuobj = Instantiate (menuObject) as GameObject;

			menuobj.transform.parent = MenuHolder.transform;

			menuList.Add (menuobj);

			string levelName = levelNames [i];

			LoadFromLocationID loader = menuobj.GetComponent<LoadFromLocationID> ();

			loader.Setup (levelName,this);

		}




	}

    public void Load()
    {
        Debug.LogFormat("Loading ARWorldMap {0}", path);
        var worldMap = ARWorldMap.Load(path);
        if (worldMap != null)
        {
            m_LoadedMap = worldMap;
            Debug.LogFormat("Map loaded. Center: {0} Extent: {1}", worldMap.center, worldMap.extent);

            UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;

            var config = m_ARCameraManager.sessionConfiguration;
            config.worldMap = worldMap;
			UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;

			Debug.Log("Restarting session with worldMap");
			session.RunWithConfigAndOptions(config, runOption);

        }
    }


	void OnWorldMapSerialized(ARWorldMap worldMap)
	{
		if (worldMap != null)
		{
			//we have an operator that converts a ARWorldMap to a serializableARWorldMap
			serializedWorldMap = worldMap;
			Debug.Log ("ARWorldMap serialized to serializableARWorldMap");
		}
	}


	public void SaveSerialized()
	{

		WorldName = worldInput.text;
		levelNames.Add (WorldName);


		session.GetCurrentWorldMapAsync(OnWorldMapSerialized);
	}

	public void LoadWithNewName(string newName) {
		WorldName = newName;
		LoadSerialized ();

		LoadMenuPanel.SetActive (false);
	}

	public GameObject LoadMenuPanel;

	public void LoadSerialized()
	{
		Debug.Log("Loading ARWorldMap from serialized data: "+WorldName);
		//we have an operator that converts a serializableARWorldMap to a ARWorldMap
		ARWorldMap worldMap = serializedWorldMap;
		if (worldMap != null)
		{
			m_LoadedMap = worldMap;
			Debug.LogFormat("Map loaded. Center: {0} Extent: {1}", worldMap.center, worldMap.extent);

			UnityARSessionNativeInterface.ARSessionShouldAttemptRelocalization = true;

			var config = m_ARCameraManager.sessionConfiguration;
			config.worldMap = worldMap;
			UnityARSessionRunOption runOption = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;

			Debug.Log("Restarting session with worldMap");
			session.RunWithConfigAndOptions(config, runOption);
		}

	}
}
