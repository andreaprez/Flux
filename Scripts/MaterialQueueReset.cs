using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialQueueReset : MonoBehaviour {

    [SerializeField] private Material[] materials;

	public void ResetQueue()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].renderQueue = -1;
        }
    }
}
