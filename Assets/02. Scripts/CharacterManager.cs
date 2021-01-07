using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{

    public List<GameObject> SearchSelectionObj()
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Character>().objectSelection)
            {
                result.Add(transform.GetChild(i).gameObject);
            }
        }
        return result;
    }
}
