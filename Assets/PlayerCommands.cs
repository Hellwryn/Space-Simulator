using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerCommands : MonoBehaviour
{
    public static PlayerCommands inst;

    public PlayerInput playerInput;
    public LayerMask mask;

    public GameObject CameraCenter;
    public GameObject MainCamera;
    public GameObject SelectedItem;
    public EventSystem eventSystem;
    public float zoomlevel = 8;
    public float zoomDistance = 8;
    public float currZoomDistance;
    public CameraOrbit orbit;
    Vector2 move;

    // TEST

    public GameObject GalSelector;
    public int selected;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        ActualizeZoom();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currScrollWheelMove = Mouse.current.scroll.ReadValue().normalized;
        if (currScrollWheelMove.y > 0.1f)
        {
            zoomlevel += 0.5f;
            if (zoomlevel > 10f)
                zoomlevel = 10f;
            SetZoom();
        }
        else if (currScrollWheelMove.y < -0.1f)
        {
            zoomlevel -= 0.5f;
            if (zoomlevel < 1f)
                zoomlevel = 1f;
            SetZoom();
        }
        ActualizeZoom();
        if (move.x != 0 || move.y != 0)
        {
            SelectedItem = null;
            orbit.enabled = false;
        }
        if (SelectedItem == null)
            CameraCenter.transform.position = new Vector3(CameraCenter.transform.position.x + move.y * Time.deltaTime * zoomlevel * 4, CameraCenter.transform.position.y, CameraCenter.transform.position.z - move.x * Time.deltaTime * zoomlevel * 4);
        else
        {
            if (Vector3.Distance(CameraCenter.transform.position, SelectedItem.transform.position) < 1f)
            {

            }
            else
                CameraCenter.transform.position = new Vector3((CameraCenter.transform.position.x + SelectedItem.transform.position.x) / 2, (CameraCenter.transform.position.y + SelectedItem.transform.position.y) / 2, (CameraCenter.transform.position.z + SelectedItem.transform.position.z) / 2);
        }
        if (!Mouse.current.middleButton.isPressed)
            if (SelectedItem == null)
            {
                float rotationY = transform.rotation.eulerAngles.y;
                if (rotationY > 180f)
                    rotationY -= 360f;
                CameraCenter.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x * 0.95f, rotationY * 0.95f, transform.rotation.eulerAngles.z * 0.8f);
            }
        AutoSelect();
    }

    public void LeftClick(InputAction.CallbackContext context)
    {
        // Debug.Log(eventSystem.IsPointerOverGameObject());
        if (context.phase == InputActionPhase.Started && !eventSystem.IsPointerOverGameObject())
        {
            RaycastHit intersection;
            Ray rayon = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(rayon, out intersection, 1000f, mask))
            {

                if (intersection.transform.gameObject.TryGetComponent(typeof(Selectable), out Component component))
                {
                    SelectedItem = intersection.transform.gameObject;
                    if (SelectedItem.transform.parent.gameObject.TryGetComponent(out Orbit newOrbit))
                    {
                        orbit.enabled = true;
                        orbit.revolutionDistance = newOrbit.revolutionDistance;
                        orbit.revolutionSpeed = newOrbit.revolutionSpeed;
                        orbit.revolutionOffset = newOrbit.revolutionOffset;
                        SetZoom();
                    }
                }
                // Debug.Log(intersection.point);
                else
                {
                    int starPos = GalacticMap.inst.Vector2ToStarPos(new Vector2(intersection.point.x, intersection.point.z));
                    if (selected == starPos)
                    {
                        int starData = GalacticMap.inst.GetStarPos(selected);
                        if (GalacticMap.inst.starSystemDatas[starData].starPos == selected)
                        {
                            SystemMap.inst.Initialize(GalacticMap.inst.starSystemDatas[starData]);
                            CameraCenter.transform.position = new Vector3(CameraCenter.transform.position.x, 0f, CameraCenter.transform.position.z);
                        }
                    }
                    else
                    {
                        selected = starPos;
                        if (starPos < 0 || starPos >= GalacticMap.inst.squares.Count)
                            return;
                        GalSelector.transform.position = new Vector3(GalacticMap.inst.squares[starPos].x, 1200f, GalacticMap.inst.squares[starPos].y);

                        // TEST : Create Hyperlanes
                        int starData = GalacticMap.inst.GetStarPos(selected);
                        if (starData < GalacticMap.inst.starSystemDatas.Count && GalacticMap.inst.starSystemDatas[starData].starPos == selected)
                        {
                            // Hyperlane Display
                            HyperlanesManager.inst.GenerateLane(starData);
                            /*
                            foreach (Transform child in HyperlanesManager.inst.transform)
                            {
                                GameObject.Destroy(child.gameObject);
                            }
                            int min = GalacticMap.inst.starSystemDatas[starData].closeStar.Count;
                            if (min > 3)
                                min = 3;
                            //foreach (int closeStarData in GalacticMap.inst.starSystemDatas[starData].closeStar)
                            for (int i = 0; i < min; i++)
                                HyperlanesManager.inst.Link(starData, GalacticMap.inst.starSystemDatas[starData].closeStar[i]);*/
                        }
                    }
                }
            }
        }
    }

    void AutoSelect()
    {
        if (GalacticMap.inst.squares.Count < 1)
            return;
        // Debug.Log(eventSystem.IsPointerOverGameObject());
        if (!eventSystem.IsPointerOverGameObject())
        {
            RaycastHit intersection;
            Ray rayon = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(rayon, out intersection, 1000f, mask))
            {
                if (GalSelector.transform.position.y < 1000f)
                {
                    return;
                }
                // Debug.Log(intersection.point);
                else
                {
                    int starPos = GalacticMap.inst.Vector2ToStarPos(new Vector2(intersection.point.x, intersection.point.z));
                    if (starPos < 0 || starPos >= GalacticMap.inst.squares.Count)
                        return;
                    if (selected == starPos)
                    {
                        return;
                    }
                    else
                    {
                        selected = starPos;

                        // TEST : Create Hyperlanes
                        int starData = GalacticMap.inst.GetStarPos(selected);
                        if (starData < GalacticMap.inst.starSystemDatas.Count && GalacticMap.inst.starSystemDatas[starData].starPos == selected)
                        {
                            GalSelector.transform.position = new Vector3(GalacticMap.inst.squares[starPos].x, 1200f, GalacticMap.inst.squares[starPos].y);
                            // Hyperlane Display
                            HyperlanesManager.inst.GenerateLane(starData);
                            /*
                            foreach (Transform child in HyperlanesManager.inst.transform)
                            {
                                GameObject.Destroy(child.gameObject);
                            }
                            int min = GalacticMap.inst.starSystemDatas[starData].closeStar.Count;
                            if (min > 3)
                                min = 3;
                            //foreach (int closeStarData in GalacticMap.inst.starSystemDatas[starData].closeStar)
                            for (int i = 0; i < min; i++)
                                HyperlanesManager.inst.Link(starData, GalacticMap.inst.starSystemDatas[starData].closeStar[i]);*/
                        }
                    }
                }
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        CameraCenter.transform.position = new Vector3(CameraCenter.transform.position.x, 1200f, CameraCenter.transform.position.z);
    }

    void SetZoom()
    {
        zoomDistance = Mathf.Pow(1.5f, zoomlevel) / 8;
        if (SelectedItem != null)
        {
            zoomDistance *= 0.05f;
            zoomDistance += SelectedItem.transform.localScale.x * 0.05f;
        }

        // MainCamera.transform.position = new Vector3(CameraCenter.transform.position.x - 10f * zoomDistance, CameraCenter.transform.position.y + 10f * zoomDistance, CameraCenter.transform.position.z);
    }

    void ActualizeZoom()
    {
        // float ecart = Mathf.Abs(zoomDistance - currZoomDistance);
        currZoomDistance = currZoomDistance + (zoomDistance - currZoomDistance) / 4;
        MainCamera.transform.localPosition = new Vector3(-10f * currZoomDistance, 10f * currZoomDistance, 0f);
    }

    public void MiddleClick(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        if (Mouse.current.middleButton.isPressed)
        {
            if (SelectedItem == null)
                CameraCenter.transform.rotation = Quaternion.Euler(0f, input.x + CameraCenter.transform.rotation.eulerAngles.y, 0f);
            else
            {
                CameraCenter.transform.rotation = Quaternion.Euler(0f, input.x * 0.2f + CameraCenter.transform.rotation.eulerAngles.y, input.y * 0.2f - CameraCenter.transform.rotation.eulerAngles.z);
                //Debug.Log(CameraCenter.transform.rotation.eulerAngles.z);
                /*
                if (CameraCenter.transform.rotation.eulerAngles.z > 0 && CameraCenter.transform.rotation.eulerAngles.z < 180f)
                {
                    CameraCenter.transform.rotation = Quaternion.Euler(0f, CameraCenter.transform.rotation.eulerAngles.y, 180f);
                }*/
                /*
                if (CameraCenter.transform.rotation.eulerAngles.z > 0 && CameraCenter.transform.rotation.eulerAngles.z > 270f)
                {
                    CameraCenter.transform.rotation = Quaternion.Euler(0f, CameraCenter.transform.rotation.eulerAngles.y, 270f);
                }*/
            }
        }
    }
}
