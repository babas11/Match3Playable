using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskPlacer : MonoBehaviour
{
    [SerializeField] GameObject maskPrefab;
    private GameObject maskInstance;
    // Start is called before the first frame update
    public void InıtMask(float scale)
    {
       if (maskInstance == null)
        {
            maskInstance = Instantiate(maskPrefab, transform);
        }

        // Set the position and scale of the mask instance
        maskInstance.transform.position = transform.position;

        Vector3 maskSize = GetComponent<SpriteRenderer>().bounds.size;

        maskInstance.transform.localScale = maskSize / scale;
    }
}
