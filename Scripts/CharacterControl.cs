using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {

    [SerializeField] private GameObject skipMessage;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dieAudioClip;
    [SerializeField] private float respawnTime;
    [SerializeField] private LayerMask invalidMask;
    [SerializeField] private Material[] materials;
    [SerializeField] private float speed;
    public bool active = false;
    private bool reversed = false;
    private Manager manager;
    private Vector3[] path;
    private int targetPos;
    private Vector3 originalPos;

    public float Speed { get { return speed; } }

    private float aux_respawnTimer = 0;

	// Use this for initialization
	void Awake () {
        manager = Manager.instance;
        originalPos = transform.position;
        manager.character = this;
        aux_respawnTimer = respawnTime;
        for (int i = 0; i < materials.Length; i++) {
            materials[i].renderQueue = 4900 - i;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (active) {
            if (Input.GetKeyDown(KeyCode.Space)) manager.Skip();

            if (Vector3.Distance(transform.position, path[targetPos]) <= 0.01f) {
                AssignNextPosition();
            }

            transform.position = Vector3.MoveTowards(transform.position, path[targetPos], speed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((path[targetPos] - transform.position).normalized), Time.deltaTime);
        }
	}

    public void Restart() {
        manager.cursor.line.ResetLine();
        transform.position = originalPos;
        EndPath();
        manager.mainCamera.transform.position = new Vector3(transform.position.x, manager.mainCamera.transform.position.y, transform.position.z);
        reversed = false;
        aux_respawnTimer = respawnTime;
    }

    public void StartPath(Vector3[] _path) {
        skipMessage.SetActive(true);
        animator.SetBool("move", true);
        path = _path;
        targetPos = 0;
        active = true;
    }

    public void EndPath() {
        skipMessage.SetActive(false);
        animator.SetBool("move", false);
        manager.Skip(true);
        active = false;
        manager.EnableCursor();
    }
    
    private void RemovePathSinceIndex(int i) {
        List<Vector3> resizedPath = new List<Vector3>(path);
        for (int point = i; point < path.Length; point++) {
            resizedPath.RemoveAt(i);
        }
        path = resizedPath.ToArray();
    }

    private void AssignNextPosition() {
        if(targetPos == path.Length - 1) {
            if (Vector3.Distance(transform.position, manager.endZone.transform.position) <= manager.endZone.Range) {
                EndPath();
                manager.LoadNextLevel();
            }
            else {
                if (!reversed) {
                    System.Array.Reverse(path);
                    reversed = true;
                    targetPos = 1;
                }
                else {
                    reversed = false;
                    EndPath();
                    manager.RestartAll();
                }
            }
            manager.cursor.line.ResetLine();
        }
        else {
            Collider[] l_collisions = Physics.OverlapSphere(path[targetPos + 1], 0.2f, invalidMask);
            if (l_collisions.Length > 0) {
                RemovePathSinceIndex(targetPos + 1);
                System.Array.Reverse(path);
                reversed = true;
                targetPos = 0;
                manager.cursor.line.ResetLine();
            }
            else {
                targetPos++;
                if (Vector3.Distance(path[targetPos], manager.endZone.transform.position) <= manager.endZone.Range) {
                    RemovePathSinceIndex(targetPos + 1);
                }
                manager.cursor.line.RemoveFirstPoint();
            }
        }
    }

    public void Kill() {
        active = false;
        if (aux_respawnTimer == respawnTime) {
            animator.SetTrigger("die");
            audioSource.Stop();
            audioSource.PlayOneShot(dieAudioClip);
        }
        aux_respawnTimer -= Time.deltaTime;
        if (aux_respawnTimer <= 0) {
            animator.SetTrigger("restart");
            manager.RestartAll();
            audioSource.Play();
        }
    }
}
