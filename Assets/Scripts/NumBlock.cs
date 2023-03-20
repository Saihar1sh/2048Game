using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumBlock : MonoBehaviour
{
    [SerializeField] TextMeshPro valueTxt;

    int value = 0;

    //properties
    public int Value 
    {   get { return value; } 
        set 
        {
            this.value = value;
            valueTxt.text = value.ToString(); 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init(int numValue)
    {
        Value = numValue;
    }

}
