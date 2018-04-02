using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // z is up/down: + is up, x is left/right: + is right

    public GameObject camObject;
    private Camera cam;

    private Vector3 camPos;
    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position

    private float perspectiveZoomSpeed = 0.2f;
    private float cameraEdgeLimit = 1000f;

    // Use this for initialization
    void Start()
    {
        camPos = camObject.transform.position;
        cam = camObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {      
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended) // update the last position based on where they moved
            {
                lp = touch.position;

                camPos.x = Mathf.Clamp(camPos.x - (lp.x - fp.x), -1*cameraEdgeLimit, cameraEdgeLimit);
                camPos.z = Mathf.Clamp(camPos.z - (lp.y - fp.y), -1*cameraEdgeLimit, cameraEdgeLimit);

                camObject.transform.position = camPos;

                fp = lp;
            }
        } else if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Otherwise change the field of view based on the change in distance between the touches.
            cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

            // Clamp the field of view to make sure it's between 0 and 180.
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 40f, 140f);
        }
    }
}