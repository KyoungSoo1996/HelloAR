using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManger : MonoBehaviour
{
    public GameObject Character;
    public GameObject CharacterManager;
    ARRaycastManager arRaycastManager;
    List<ARRaycastHit> hits;

    float walkSpeed = 0.01f, runSpeed = 0.02f;
    float timer = 3, limitTime = 3.0f;

    public List<GameObject> SelectionGroup = new List<GameObject>();

    enum DelaySW
    {
        Start,
        End
    }
    DelaySW delaySW;

    enum ObjectFunc
    {
        Selection,
        Move,
        Rotate,
        Scale,
        AnmationChange
    }
    int objectFun;

    int animationSelector = 0;

    void Start()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        hits = new List<ARRaycastHit>();
        objectFun = (int)ObjectFunc.Selection;
    }


    void Update()
    {
        ARCamera(Character);
    }


    private void ARCamera(GameObject obj)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            arRaycastManager.Raycast(Input.mousePosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.FeaturePoint);

            // 캐릭터를 클릭하면, 선택된 캐릭터 체크 
            if (Physics.Raycast(ray, out rayHit))
            {
                if (rayHit.collider.tag == "Object" && (ObjectFunc)objectFun == ObjectFunc.Selection)
                {
                    rayHit.collider.GetComponent<Character>().ObjectSelectionSW();
                    SelectionGroup = CharacterManager.GetComponent<CharacterManager>().SearchSelectionObj();
                }

                if (rayHit.collider.tag == "Diamond")
                {
                    objectFun++;
                    objectFun = objectFun % System.Enum.GetValues(typeof(ObjectFunc)).Length;
                    for (int i = 0; i < SelectionGroup.Count; i++)
                    {
                        SelectionGroup[i].GetComponent<Character>().DiamondColor(objectFun);
                    }
                }
            }

            // AR이 인식한 화면을 클릭하면, 캐릭터 생성
            if (SelectionGroup.Count == 0)
            {
                if (hits.Count > 0)
                {
                    Pose pos = hits[0].pose;
                    Instantiate(obj, pos.position, pos.rotation, CharacterManager.transform);
                }
            }
        }

        if (SelectionGroup.Count > 0)
        {
            switch ((ObjectFunc)objectFun)
            {
                case ObjectFunc.Selection:
                    break;
                case ObjectFunc.Move:
                    Move(SelectionGroup);
                    break;
                case ObjectFunc.Rotate:
                    Rotate(SelectionGroup);
                    break;
                case ObjectFunc.Scale:
                    Scale(SelectionGroup);
                    break;
                case ObjectFunc.AnmationChange:
                    AniamtionChange(SelectionGroup);
                    break;
            }
        }
    }

    private void Move(List<GameObject> objects)
    {
        Touch touch;
        touch = Input.GetTouch(0);
        Debug.Log(touch);
        if (Input.touchCount == 1 && touch.phase == TouchPhase.Moved)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].transform.localPosition += transform.right * Time.deltaTime * 0.1f * touch.deltaPosition.x + transform.forward * 0.1f * Time.deltaTime * touch.deltaPosition.y;
            }
        }
    }

    private void Rotate(List<GameObject> objects)
    {
        Touch touch;
        touch = Input.GetTouch(0);
        if (Input.touchCount == 1 && touch.phase == TouchPhase.Moved)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].transform.Rotate(Vector3.down * 40f * Time.deltaTime * touch.deltaPosition.x, Space.Self);
            }
        }
    }

    private void Scale(List<GameObject> objects)
    {
        if (Input.touchCount == 2)
        {
            Touch touchOne = Input.GetTouch(0);
            Touch touchTwo = Input.GetTouch(1);

            Vector2 touchOnePos = touchOne.position - touchOne.deltaPosition;
            Vector2 touchTwoPos = touchTwo.position - touchTwo.deltaPosition;

            float prevTouchDeltaMag = (touchOnePos - touchTwoPos).magnitude;
            float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float pinchAmount = deltaMagnitudeDiff * 0.02f * Time.deltaTime;

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].transform.localScale += new Vector3(-pinchAmount, -pinchAmount, -pinchAmount);
            }
        }
    }

    private void AniamtionChange(List<GameObject> objects)
    {
        if (Input.touchCount == 1)
        {
            animationSelector++;
            animationSelector = animationSelector % 5;
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].GetComponent<Animator>().SetInteger("animationSelector", animationSelector);
            }
            if (animationSelector == 1 || animationSelector == 2)
            {
                timer = 0;
                delaySW = DelaySW.Start;
            }
        }
        DelayFunc(objects);
    }

    private void DelayFunc(List<GameObject> objects)
    {
        switch (delaySW)
        {
            case DelaySW.Start:
                timer += Time.deltaTime;
                if (timer > limitTime)
                {
                    delaySW = DelaySW.End;
                }
                break;
            case DelaySW.End:
                // Run
                if (animationSelector == 1)
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        objects[i].transform.localPosition += objects[i].transform.forward * runSpeed * objects[i].transform.localScale.x;
                    }
                }

                // Walk
                else if (animationSelector == 2)
                {
                    for (int i = 0; i < objects.Count; i++)
                    {
                        objects[i].transform.localPosition += objects[i].transform.forward * walkSpeed * objects[i].transform.localScale.x;
                    }
                }
                break;
        }
    }

}
