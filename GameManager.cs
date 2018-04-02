using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    //public Text text;

    public enum Phase { SearhcingGPS, APICall, Drawing, Play }
    public Phase currentPhase;

    public float currentLat, currentLon;
    //public bool foundLoc, gotData;

    public float minX, maxX, minY, maxY;

    public Transform locationPrefab;

    private List<Vector3> locationPoints;
    MapReader map;

    //private string URL;

    // Use this for initialization
    void Start () {
        Debug.ClearDeveloperConsole();
        map = gameObject.GetComponent<MapReader>();
        //map.ReadInput("iu");
        currentPhase = Phase.SearhcingGPS;
        locationPoints = new List<Vector3>();
        StartCoroutine(StartLocationService());
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

        currentLat = Input.location.lastData.latitude;
        currentLon = Input.location.lastData.longitude;
        currentPhase = Phase.APICall;
        Input.location.Stop();

        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPhase == Phase.APICall)
        {
            Debug.Log("found location of: " + currentLat.ToString() + " " + currentLon.ToString());
            currentPhase = Phase.Drawing;

            StartCoroutine(MapAPICall());
            StartCoroutine(GenerateLocations());
        }
    }

    IEnumerator MapAPICall()
    {
        float latHeight = 0.010f;
        float lonWidth = 0.025f;
        float lonMin = currentLon - lonWidth / 2, lonMax = currentLon + lonWidth / 2, latMin = currentLat - latHeight / 2, latMax = currentLat + latHeight / 2;

        // setting class variables for future use
        minX = (float) (MercatorProjection.lonToX(lonMin) - MercatorProjection.lonToX(currentLon));
        maxX = (float) (MercatorProjection.lonToX(lonMax) - MercatorProjection.lonToX(currentLon));
        minY = (float) (MercatorProjection.latToY(latMin) - MercatorProjection.latToY(currentLat));
        maxY = (float) (MercatorProjection.latToY(latMax) - MercatorProjection.latToY(currentLat));

        // longitude is "x", latitude is "y"
        // url bounds in form: longitude min, latitude min, longitude max, latitude max
        string URL = "https://overpass-api.de/api/map?bbox=" + lonMin.ToString() + "," + latMin.ToString() + ","
            + lonMax.ToString() + "," + latMax.ToString();

        UnityWebRequest uwr = UnityWebRequest.Get(URL);
        String filePath = Application.persistentDataPath + "/data1.txt"; // this needs to change
        if (File.Exists(filePath))
        {
            Debug.Log("File already exists! Deleting it now.");
            File.Delete(filePath);
            //map.ReadInput(filePath);
            //yield break;
        }

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
            map.ReadInput(filePath);
            yield break;
        }
    }

    IEnumerator GenerateLocations()
    {
        var minDistanceBetweenLocations = 25;
        var radius = 150f;


        // make random angles, find points on circumference at given angle, check if they are close
        // to other points and in bounds, if so add to locations list

        Debug.Log("minx: " + minX + " maxX: " + maxX + " minY: " + minY + " maxY: " + maxY);

        while (locationPoints.Count < 3)
        {
            var angle = UnityEngine.Random.Range(0f, 1f) * Math.PI * 2;
            var x = ((float)Math.Cos(angle) * radius);
            var y = ((float)Math.Sin(angle) * radius);

            Vector3 point = new Vector3(x, 10, y);

            if (point.x < minX || point.x > maxX || point.y < minY || point.y > maxY)
                continue;

            if (locationPoints.Count == 0)
            {
                locationPoints.Add(point);
                //Debug.Log("found good point at: " + point);
                continue;
            }

            for (int i = 0; i < locationPoints.Count; i++)
            {
                if (Vector3.Distance(locationPoints[i], point) < minDistanceBetweenLocations)
                {
                    break;
                }
                if (i == locationPoints.Count - 1)
                {
                    locationPoints.Add(point);
                    //Debug.Log("found good point at: " + point);
                }
            }
        }

        Debug.Log("Instantiating locations!");
        // we now have 3 Vector3's that correspond to location coordinates
        foreach (Vector3 p in locationPoints)
        {
            Instantiate(locationPrefab, p, Quaternion.identity);
        }
        
        yield break;
    }
}
