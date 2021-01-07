using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static Character Instance = null;
    public GameObject diamond;
    // 객체 선택되어있는지 확인하기 위한 변수
    public bool objectSelection = false;

    public enum ObjColor
    {
        red,
        blue,
        green,
        yellow,
        black
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GetComponent<Outline>().enabled = false;
    }


    public void ObjectSelectionSW()
    {
        objectSelection = !objectSelection;
        GetComponent<Outline>().enabled = objectSelection;
        diamond.SetActive(objectSelection);
    }

    public void DiamondColor(int color)
    {
        switch ((ObjColor)color)
        {
            case ObjColor.red:
                diamond.GetComponent<Renderer>().material.color = Color.red;
                break;
            case ObjColor.blue:
                diamond.GetComponent<Renderer>().material.color = Color.blue;
                break;
            case ObjColor.green:
                diamond.GetComponent<Renderer>().material.color = Color.green;
                break;
            case ObjColor.yellow:
                diamond.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case ObjColor.black:
                diamond.GetComponent<Renderer>().material.color = Color.black;
                break;
        }
    }
}
