// credits to Brett Hewitt:
// youtube link: https://www.youtube.com/watch?v=pKRnfwFOc_c
// made this with the help of this tutorial


using UnityEngine;
using System.Collections;



public class RtsMinimapController : MonoBehaviour {

    public float squareWidth = 2;

    // This script creates the minimap

    public static RtsMinimapController instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RtsMinimapController>();
            }
            return _instance;
        }
    }

    private static RtsMinimapController _instance;


    private Camera c_MiniCam;
    internal Rect r_MinimapRect;
    private Camera c_MainCam;

    // vectors for rays that contain the location of the 4 corners of the view of the main camera
    Vector3 v_MainCameraCorner1;
    Vector3 v_MainCameraCorner2;
    Vector3 v_MainCameraCorner3;
    Vector3 v_MainCameraCorner4;



    // Vectors containing the bottomleft and topright corners of the main camera view
    [HideInInspector]
    public Vector3 v_BottomLeftCorner;
    [HideInInspector]
    public Vector3 v_TopRightCorner;

    // cursor offset
    float f_OffsetX;
    float f_OffsetZ;

    Rect paddedMinimapBounds;
    // Material to draw the lines in the minimap of the main camera view
    public Material m_LineMaterial;

    // Use this for initialization
    void Start()
    {

        c_MainCam = Camera.main;
        c_MiniCam = GetComponent<Camera>();

        var shader = Shader.Find("Hidden/Internal-Colored");
        m_LineMaterial = new Material(shader);
    }

    // Update is called once per frame
    void Update()
    {
        if (ExitPopUp.b_StillPlaying == false)
            return;
        
        LoadMiniMap();
        MouseClicked();
        
        
    }



    // if we click inside the minimap's rect, get where we clicked and move the main camera to that location for quick navigation
    void MouseClicked()
    {
        // once mouse button is clicked...
        if (Input.GetMouseButtonDown(0))
        {

            // make a ray to the location where we clicked... and
            if (r_MinimapRect.Contains(Input.mousePosition))
            {

                Ray ray = c_MiniCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // move the camera to that location
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 targetPos = new Vector3(hit.point.x , RtsPlayerController.instance.transform.position.y, hit.point.z);

                    LimitCamera();

                    if (!paddedMinimapBounds.Contains(Input.mousePosition))
                    {
                        Vector3 screenTargetPos = c_MiniCam.WorldToScreenPoint(targetPos);

                        if (screenTargetPos.x < paddedMinimapBounds.x)
                            screenTargetPos.x = paddedMinimapBounds.x;

                        if (screenTargetPos.x > paddedMinimapBounds.x + paddedMinimapBounds.width)
                            screenTargetPos.x = paddedMinimapBounds.x + paddedMinimapBounds.width;

                        if (screenTargetPos.y < paddedMinimapBounds.y)
                            screenTargetPos.y = paddedMinimapBounds.y;

                        if (screenTargetPos.y > paddedMinimapBounds.y + paddedMinimapBounds.height)
                            screenTargetPos.y = paddedMinimapBounds.y + paddedMinimapBounds.height;

                        targetPos = c_MiniCam.ScreenToWorldPoint(screenTargetPos);
                    }

                    targetPos.y = transform.position.y;
                    RtsPlayerController.instance.transform.position = targetPos;

                    UpdateViewPort();
                }
            }
        }
    }

    public void EdgeMovement() {

       
       
        Vector3 targetPos = new Vector3 (RtsPlayerController.instance.transform.position.x, RtsPlayerController.instance.transform.position.y, RtsPlayerController.instance.transform.position.z);

       }

  public void LimitCamera();
      //  if (!paddedMinimapBounds.Contains(RtsPlayerController.instance.transform.position)) {

            Vector3 screenPos = c_MiniCam.WorldToScreenPoint(RtsPlayerController.instance.transform.position);

            if (screenPos.x < paddedMinimapBounds.x) {
                screenPos.x = paddedMinimapBounds.x;
            }

            if (screenPos.x > paddedMinimapBounds.x + paddedMinimapBounds.width) {
                screenPos.x = paddedMinimapBounds.x + paddedMinimapBounds.width;
            }

            if (screenPos.y < paddedMinimapBounds.y)
                screenPos.y = paddedMinimapBounds.y;

            if (screenPos.y > paddedMinimapBounds.y + paddedMinimapBounds.height)
                screenPos.y = paddedMinimapBounds.y + paddedMinimapBounds.height;

            targetPos = c_MiniCam.ScreenToWorldPoint(screenPos);
       // }
        targetPos.y = transform.position.y;
        RtsPlayerController.instance.transform.position = targetPos;

        UpdateViewPort();
    }

    void LimitCamera() {

        Rect smallRectangleBounds = new Rect(0, 0, v_MainCameraCorner4.x - v_MainCameraCorner1.x, v_MainCameraCorner2.y - v_MainCameraCorner1.y);

        paddedMinimapBounds = new Rect();
        paddedMinimapBounds.x = r_MinimapRect.x + smallRectangleBounds.width / 2f;
        paddedMinimapBounds.y = r_MinimapRect.y + smallRectangleBounds.height / 2f;
        paddedMinimapBounds.width = r_MinimapRect.width - smallRectangleBounds.width;
        paddedMinimapBounds.height = r_MinimapRect.height - smallRectangleBounds.height;

    }


    public void UpdateViewPort()
    {
        // get the 4 border corners of the main camera view
        Ray ray1 = c_MainCam.ScreenPointToRay(new Vector3(0, 0, 0));
        Ray ray2 = c_MainCam.ScreenPointToRay(new Vector3(0, Screen.height - 1, 0));
        Ray ray3 = c_MainCam.ScreenPointToRay(new Vector3(Screen.width, Screen.height - 1, 0));
        Ray ray4 = c_MainCam.ScreenPointToRay(new Vector3(Screen.width, 0, 0));


        RaycastHit hit;
        Physics.Raycast(ray1, out hit, Mathf.Infinity);
        Vector3 v1 = hit.point;
        Physics.Raycast(ray2, out hit, Mathf.Infinity);
        Vector3 v2 = hit.point;
        Physics.Raycast(ray3, out hit, Mathf.Infinity);
        Vector3 v3 = hit.point;
        Physics.Raycast(ray4, out hit, Mathf.Infinity);
        Vector3 v4 = hit.point;

        v_MainCameraCorner1 = c_MiniCam.WorldToScreenPoint(v1);
        v_MainCameraCorner2 = c_MiniCam.WorldToScreenPoint(v2);
        v_MainCameraCorner3 = c_MiniCam.WorldToScreenPoint(v3);
        v_MainCameraCorner4 = c_MiniCam.WorldToScreenPoint(v4);

        // this is to find the offset of where player is clicking so main camera transform always happens for the middle of the cam
        Ray ray5 = c_MainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray5, out hit, Mathf.Infinity))
        {
            f_OffsetZ = hit.point.z - c_MainCam.transform.position.z;
            float midpointX = hit.point.x;

            Ray ray6 = c_MainCam.ScreenPointToRay(new Vector3((Screen.width) / 2, Screen.height / 2, 0));
            if (Physics.Raycast(ray6, out hit, Mathf.Infinity))
            {
                f_OffsetX = hit.point.x - midpointX;
            }
            else
            {
                f_OffsetX = 0;
            }
        }
        else
        {
            f_OffsetX = 0;
            f_OffsetZ = 0;
        }
        c_MiniCam.orthographicSize = orthographicSize * normalAspect / ((float)c_MiniCam.pixelWidth / c_MiniCam.pixelHeight);

    }

    float normalAspect = 16 / 9f;
    public float orthographicSize = 48f;

    // load and get minimap info
    public void LoadMiniMap()
    {
        // get the aspect ratio of the screen so that when we make a minimap it'll always be made with respect to the screen size resolution
        float f_AspectRatio = (float)Screen.width / (float)Screen.height;
        // put the minimap rect on the bottom
        float f_ViewPortY = 0;
        float f_ViewPortHeight = 0.25f;//1f / 3.5f;
        // put the minimap rect on the right
        float f_ViewportWidth = 0.18f;// 1f / (3.5f * f_AspectRatio);
        float f_ViewPortX = 0;

        // assign minimap rect size
        c_MiniCam.rect = new Rect(f_ViewPortX, f_ViewPortY, f_ViewportWidth, f_ViewPortHeight);

        r_MinimapRect = c_MiniCam.rect;
        // mini rect location and with respect to screen resolution
        r_MinimapRect = new Rect((f_ViewPortX * Screen.width), (f_ViewPortY * Screen.height), f_ViewportWidth * Screen.width, f_ViewPortHeight * Screen.height);

        UpdateViewPort();

        // find the bounds of the minimap
        Ray ray1 = c_MiniCam.ViewportPointToRay(new Vector3(0, 0, 0));
        Ray ray3 = c_MiniCam.ViewportPointToRay(new Vector3(1, 1, 0));


        RaycastHit hit;
        Physics.Raycast(ray1, out hit, Mathf.Infinity);
        v_BottomLeftCorner = hit.point;
        Physics.Raycast(ray3, out hit, Mathf.Infinity);
        v_TopRightCorner = hit.point;


       c_MiniCam.orthographicSize = orthographicSize*normalAspect/ ((float)c_MiniCam.pixelWidth / c_MiniCam.pixelHeight);
       // Debug.Log(c_MiniCam.orthographicSize);
    }

    // draw the map view on the minimap 
    void OnPostRender()
    {
        GL.PushMatrix();
        m_LineMaterial.SetPass(0);
       // GL.LoadOrtho();
        GL.LoadPixelMatrix();
       
        GL.Begin(GL.LINES);
        GL.Color(Color.white);

        GL.Vertex(new Vector3(v_MainCameraCorner1.x, v_MainCameraCorner1.y, 0));
        GL.Vertex(new Vector3(v_MainCameraCorner2.x, v_MainCameraCorner2.y, 0));

        GL.Vertex(new Vector3(v_MainCameraCorner2.x, v_MainCameraCorner2.y, 0));
        GL.Vertex(new Vector3(v_MainCameraCorner3.x, v_MainCameraCorner3.y, 0));

        GL.Vertex(new Vector3(v_MainCameraCorner3.x, v_MainCameraCorner3.y, 0));
        GL.Vertex(new Vector3(v_MainCameraCorner4.x, v_MainCameraCorner4.y, 0));

        GL.Vertex(new Vector3(v_MainCameraCorner4.x, v_MainCameraCorner4.y, 0));
        GL.Vertex(new Vector3(v_MainCameraCorner1.x, v_MainCameraCorner1.y, 0));

        GL.End();
        GL.PopMatrix();
    }

  
}
