using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject GameManager;

    public static Vector3 touchStart;
    public static Vector3 touchEnd;

    public float touchDistance;
    public static float difference;
    public float zoomOutMin = 2;
    public float zoomOutMax = 50;
    public static bool isScrolling = false;

    public Camera cam;
    public float groundZ = 0;
    // Start is called before the first frame update
    void Start()
    {
        isScrolling = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GetComponent<GameController>().isPannelActive == false)
        {
            if (Input.GetMouseButtonDown(0))
            {

                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePreviousPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePreviousPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                 difference = currentMagnitude - prevMagnitude;

                zoom(difference * 0.01f);
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += direction;
                isScrolling = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                touchEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isScrolling = false;
            }
            zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}

/*
if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
            
        }
        zoom(Input.GetAxis("Mouse ScrollWheel"));
*/

/*
if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPosition(groundZ);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - GetWorldPosition(0);
            Camera.main.transform.position += direction;
        }
*/