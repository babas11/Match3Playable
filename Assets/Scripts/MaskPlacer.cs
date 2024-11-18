using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskPlacer : MonoBehaviour
{
    [SerializeField] GameObject maskObject;
    // Start is called before the first frame update
    public void InıtMask(float scale){
        maskObject = Instantiate(maskObject);
        maskObject.transform.parent = transform;
        maskObject.transform.position = transform.position;
        Vector3 maskSize = GetComponent<SpriteRenderer>().bounds.size;

        maskObject.transform.localScale = maskSize / scale;
    }
}
