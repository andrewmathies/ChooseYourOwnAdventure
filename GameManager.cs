using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    //public static GameManager Instance { get; set; }

    public Text text;

    public float lat, lon;
    public bool foundLoc, gotData;

    private string URL;

    // Use this for initialization
    void Start () {
        foundLoc = false;
        MapReader map = FindObjectOfType<MapReader>();
        map.ReadInput("data1");
        //StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User did not enable location services.");
            yield break;
        }

        Input.location.Start();
        int waitTimeOut = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && waitTimeOut > 0)
        {
            yield return new WaitForSeconds(1);
            waitTimeOut--;
        }

        if (waitTimeOut <= 0)
        {
            Debug.Log("Timed out.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location.");
            yield break;
        }

        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;
        foundLoc = true;
        Input.location.Stop();

        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        if (foundLoc)
        {
            Debug.Log("found location of: " + this.lat.ToString() + " " + this.lon.ToString() + " stopping location coroutine");
            foundLoc = false;
            GetMapData();
        }
    }

    void GetMapData()
    {
        float latHeight = 0.010f;
        float lonWidth = 0.025f;
        float lonMin = lon - lonWidth / 2, lonMax = lon + lonWidth / 2, latMin = lat - latHeight / 2, latMax = lat + latHeight / 2;
        // longitude is "x", latitude is "y"
        // url bounds in form: longitude min, latitude min, longitude max, latitude max
        URL = "https://overpass-api.de/api/map?bbox=" + lonMin.ToString() + "," + latMin.ToString() + "," 
            + lonMax.ToString() + "," + latMax.ToString();
        Debug.Log(URL);

        StartCoroutine(MapAPICall());
    }

    IEnumerator MapAPICall()
    {
        UnityWebRequest uwr = UnityWebRequest.Get(URL);
        String filePath = Application.persistentDataPath + "/data1.txt"; // this needs to change
        uwr.downloadHandler = new DownloadHandlerFile(filePath);
        yield return uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
        {
            Debug.Log(uwr.error);
            yield break;
        }
        else
        {
            Debug.Log("Saved data to: " + filePath);
            MapReader map = FindObjectOfType<MapReader>();
            map.ReadInput(filePath);
            yield break;
        }
    }
}
