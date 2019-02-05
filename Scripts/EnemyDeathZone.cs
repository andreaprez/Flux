using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathZone : MonoBehaviour {

    [SerializeField] private Enemy[] couples;
    [SerializeField] private float range;

    void Update () {
        for (int i = 0; i < couples.Length; i++) {
            if (Vector3.Distance(couples[i].transform.position, transform.position) <= range) {
                if (couples[i].CanDie()) { couples[i].Destroy(); }
            }
        }
	}

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
#endif
}
