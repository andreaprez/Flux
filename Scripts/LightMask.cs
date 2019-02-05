using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMask : MonoBehaviour {

    public float distanceToTarget = 20f;
    private Manager manager;
    private Quaternion fixedRotation;

	void Awake () {
        manager = Manager.instance;
        manager.lightMask = gameObject;
        transform.position = new Vector3(manager.character.transform.position.x, distanceToTarget, manager.character.transform.position.z);
        fixedRotation = transform.rotation;
    }
	
	void LateUpdate () {
        if (transform.parent != null) {
            float l_posY = transform.position.y;
            Vector3 l_adjustedPos = manager.character.transform.position + (manager.mainCamera.transform.position - manager.character.transform.position).normalized * l_posY;
            l_adjustedPos.y = l_posY;
            transform.position = new Vector3(l_adjustedPos.x, l_posY, l_adjustedPos.z);
            transform.rotation = fixedRotation;
        }
        else {
            if (Input.GetMouseButton(0))
                transform.position = new Vector3(manager.cursor.transform.position.x, distanceToTarget, manager.cursor.transform.position.z);
        }
    }
}
