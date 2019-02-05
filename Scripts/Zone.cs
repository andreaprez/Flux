using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

    [SerializeField] private bool start;
    [SerializeField] private float range;
    public float Range { get { return range; } }

    private void Awake() {
        if (start) { Manager.instance.startZone = this; }
        else { Manager.instance.endZone = this; }

        transform.parent = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, range);
    }
#endif
}
