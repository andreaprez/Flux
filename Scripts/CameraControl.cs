using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float panBorderThickness = 10f;
    [SerializeField] private Vector2 panLimit;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float distanceYToTarget = 9.0f;

    private Manager manager;

    private void Awake() {
        manager = Manager.instance;
        manager.mainCamera = this;
        SetTarget(manager.cursor.transform);
        Invoke("RelocateToCharacterPos", 0.2f);
    }


    private void Update() {
        if (targetTransform.tag == "Cursor") {
            if (Input.GetMouseButton(0)) {
                Vector3 pos = transform.position;
                if (Input.mousePosition.y >= Screen.height - panBorderThickness)
                    pos.z += panSpeed * Time.deltaTime;
                if (Input.mousePosition.y <= panBorderThickness)
                    pos.z -= panSpeed * Time.deltaTime;
                if (Input.mousePosition.x >= Screen.width - panBorderThickness)
                    pos.x += panSpeed * Time.deltaTime;
                if (Input.mousePosition.x <= panBorderThickness)
                    pos.x -= panSpeed * Time.deltaTime;

                pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
                pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

                transform.position = pos;
            }
        }
        else {
            Vector3 l_desiredPosition = targetTransform.position - Vector3.forward * distanceYToTarget/2.0f;
            l_desiredPosition.y = transform.position.y;
            Vector3 l_smoothedPosition = Vector3.Lerp(transform.position, l_desiredPosition, panSpeed);
            transform.position = l_smoothedPosition;
        }
    }

    public void RelocateToCharacterPos() {
        transform.position = manager.character.transform.position + Vector3.up * distanceYToTarget - Vector3.forward * distanceYToTarget/2.0f;
        float x = Mathf.Clamp(transform.position.x, -panLimit.x, panLimit.x);
        float z = Mathf.Clamp(transform.position.z, -panLimit.y, panLimit.y);
        transform.position = new Vector3(x, transform.position.y, z);
    }

    public void SetTarget(Transform _targetTransform) {
        targetTransform = _targetTransform;
    }
}
