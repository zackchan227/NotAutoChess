using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler
{
    public Camera cam;
    [SerializeField] float _smoothTime = 0.1f; // as small as fast
    private Vector3 dragOrigin;
    // Start is called before the first frame update
    float sizeTemp = 0;
    float minZoom;
    bool isDragging = false;
    
    void Start()
    {
        minZoom = 6.0f;
        //maxZoom = GameManager.Instance._maxZoom;
        cam.orthographicSize = GameManager.Instance._maxZoom;
        sizeTemp = cam.orthographicSize;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(GameManager.Instance.isPausing) return;
        if(!isDragging)
        {
            ZoomCamera();
            if (cam.orthographicSize <= GameManager.Instance._maxZoom - 0.5f)
            {
                FollowPlayer();
            }
        }

        

        if (cam.orthographicSize < GameManager.Instance._maxZoom)
        {
            if(Input.touchCount < 2) PanCamera();
        }
        
        if(cam.orthographicSize > GameManager.Instance._maxZoom)
        {
            cam.orthographicSize = GameManager.Instance._maxZoom;
        }
       
    }

    private void FollowPlayer()
    {
        Vector3 velocity = Vector3.zero;
        Vector2 playerPosition = Player.Instance.transform.position;
        //cam.transform.position = new Vector3(playerPosition.x, cam.transform.position.y, cam.transform.position.z);
        cam.transform.position = Vector3.SmoothDamp(transform.position, new Vector3(playerPosition.x, cam.transform.position.y, cam.transform.position.z), ref velocity, _smoothTime);
        //StartCoroutine(MoveToTargetPosition(new Vector3(playerPosition.x, cam.transform.position.y, cam.transform.position.z)));
        ClampCamera();
    }

    private void ZoomCamera()
    {
        if (cam.orthographic)
        {
            if(Input.touchCount >= 2)
            {
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);
                Vector2 firstTouchOriginPos = firstTouch.position - firstTouch.deltaPosition;
                Vector2 secondTouchOriginPos = secondTouch.position - secondTouch.deltaPosition;
                float originMagnitude = (firstTouchOriginPos - secondTouchOriginPos).magnitude;
                float currentMagnitude = (firstTouch.position - secondTouch.position).magnitude;
                float diff = currentMagnitude - originMagnitude;
                
                sizeTemp -= diff * 0.01f;
            }
            else
            {
                sizeTemp -= Input.GetAxis("Mouse ScrollWheel") * 1.0f;
            }

            if (sizeTemp < minZoom || sizeTemp > GameManager.Instance._maxZoom)
            {
                sizeTemp = cam.orthographicSize;
                return;
            }
            if (sizeTemp != cam.orthographicSize)
            {
                cam.orthographicSize = sizeTemp;
                //cam.orthographicSize = Mathf.Clamp(sizeTemp,minZoom,GameManager.Instance._maxZoom);
            }
            CenterCamera();
        }
    }

    IEnumerator MoveToTargetPosition(Vector3 targetPos)
    {
        while(cam.transform.position.x != targetPos.x)
        {
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, targetPos, Time.deltaTime / 2);
            yield return null;
        }
    }

    private void CenterCamera()
    {
        if (GameManager.Instance._isReadyToSwitchIsometric)
        {
            if (cam.orthographicSize > GameManager.Instance._maxZoom - 0.5f)
            {
                Vector3 tempPosition = new Vector3(-0.5f, cam.transform.position.y, cam.transform.position.z);
                if (tempPosition.x != cam.transform.position.x)
                {
                    //cam.transform.position = new Vector3(-0.5f, cam.transform.position.y, cam.transform.position.z);
                    StartCoroutine(MoveToTargetPosition(tempPosition));
                }
            }
        }
        else
        {
            if (cam.orthographicSize > GameManager.Instance._maxZoom - 0.5f)
            {
                Vector3 tempPosition = new Vector3(0, cam.transform.position.y, cam.transform.position.z);
                if (tempPosition.x != cam.transform.position.x)
                {
                    //cam.transform.position = new Vector3(-0.5f, cam.transform.position.y, cam.transform.position.z);
                    StartCoroutine(MoveToTargetPosition(tempPosition));
                }
            }
        }
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            //isDragging = true;
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 diff = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position += new Vector3(diff.x, 0, 0);

            ClampCamera();
            //cam.transform.position = ClampCamera(cam.transform.position);
        }

        if(Input.GetMouseButtonUp(0))
        {
            //isDragging = false;
        }
    }

    private void ClampCamera()
    {
        if (GameManager.Instance._maxZoom == 9.0f)
        {
            if (cam.transform.position.x < -1.8f)
            {
                cam.transform.position = new Vector3(-1.8f, cam.transform.position.y, cam.transform.position.z);
            }

            if (cam.transform.position.x > 0.8f)
            {
                cam.transform.position = new Vector3(0.8f, cam.transform.position.y, cam.transform.position.z);
            }
        }
        else
        {
            if (cam.transform.position.x < -4.0f)
            {
                cam.transform.position = new Vector3(-4.0f, cam.transform.position.y, cam.transform.position.z);
            }

            if (cam.transform.position.x > 4.0f)
            {
                cam.transform.position = new Vector3(4.0f, cam.transform.position.y, cam.transform.position.z);
            }
        }
    }

    private Vector3 ClampCamera(Vector3 targetPos)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        // float minX = -(GridManager.Instance.getWidth()/2) + camWidth;
        // float maxX = (GridManager.Instance.getWidth()/2) - camWidth;
        // float minY = -(GridManager.Instance.getHeight()/2) + camHeight;
        // float maxY = (GridManager.Instance.getHeight()/2) - camHeight;
        float minX = -4 / 2 + camWidth;
        float maxX = 4 / 2 - camWidth;
        float minY = -4 / 2 + camHeight;
        float maxY = 4 / 2 - camHeight;

        float newX = Mathf.Clamp(targetPos.x, minX, maxX);
        float newY = Mathf.Clamp(targetPos.y, minY, maxY);

        return new Vector3(newX, newY, cam.transform.position.z);
    }

    // public void OnPointerDown(PointerEventData pointerEventData)
    // {
    //     if(Input.touchCount >= 2)
    //     {
    //         _isZooming = true;
    //     }
    // }

    // public void OnPointerUp(PointerEventData pointerEventData)
    // {
    //     _isZooming = false;
    // }
}