using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {

    public static Manager instance = null;
    #region Public References
        public CharacterControl character;
        public CursorControl cursor;
        public Zone startZone;
        public Zone endZone;
        public CameraControl mainCamera;
        public List<Enemy> enemies;
        public GameObject lightMask;
    #endregion

    [Space]
    [SerializeField] private float skipSpeed = 5f;
    [SerializeField] private float enemyAlertRangeAroundEndZone = 10f;

    public float EnemyAlertRangeAroundEndZone { get { return enemyAlertRangeAroundEndZone; } }

    private bool aux_skipActive = false;

    private void Awake() {
        ///Singletone
        if(instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else if(instance != this) { DestroyImmediate(gameObject); return; }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
#if !UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.UpArrow)) LoadNextLevel();
        if (Input.GetKeyDown(KeyCode.DownArrow)) LoadLevel(SceneManager.GetActiveScene().buildIndex - 1);
#endif
    }

    public void EnableCharacter(Vector3[] _path) {
        ///Add player start position to the path
        Vector3[] l_pathWithCharacterPos = new Vector3[_path.Length + 1];
        l_pathWithCharacterPos[0] = character.transform.position;
        for (int i = 1; i < l_pathWithCharacterPos.Length; i++) {
            l_pathWithCharacterPos[i] = _path[i - 1];
        }
        character.StartPath(l_pathWithCharacterPos);

        //set camera target to character
        mainCamera.SetTarget(character.transform);
        Cursor.lockState = CursorLockMode.Locked;
        lightMask.transform.position = new Vector3 (character.transform.position.x, lightMask.transform.position.y, character.transform.position.z);
        lightMask.transform.parent = character.transform;
    }

    public void EnableCursor() {
        cursor.SetActive();
        //set camera target to player
        mainCamera.SetTarget(cursor.transform);
        mainCamera.RelocateToCharacterPos();
        Cursor.lockState = CursorLockMode.None;
        lightMask.transform.position = new Vector3(character.transform.position.x, lightMask.transform.position.y, character.transform.position.z);
        lightMask.transform.parent = null;
    }

    public Vector3 GetMousePosInWorld(Camera _camera, float cameraYOffset) {
        Vector3 l_screenMousePosition = Input.mousePosition;
        Ray l_cameraToMouse = _camera.ScreenPointToRay(l_screenMousePosition);
        Plane l_planeXZ = new Plane(Vector3.up, Vector3.up * cameraYOffset);
        float l_cameraToPlaneDistance;
        l_planeXZ.Raycast(l_cameraToMouse, out l_cameraToPlaneDistance);
        return l_cameraToMouse.GetPoint(l_cameraToPlaneDistance);
    }

    public void Skip(bool cancel = false) {
        if(!cancel && !aux_skipActive) Time.timeScale *= skipSpeed;
        else if(cancel && aux_skipActive) Time.timeScale /= skipSpeed;

        aux_skipActive = !cancel;
    }

    public void RestartAll() {
        character.Restart();
        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].gameObject.SetActive(true);
            enemies[i].Restart();
        }
    }

    public void LoadNextLevel() {
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex >= SceneManager.sceneCountInBuildSettings) nextLevelIndex = 0;
        SceneManager.LoadScene(nextLevelIndex);
    }

    public void LoadLevel(int i) {
        SceneManager.LoadScene(i);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (endZone) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(endZone.transform.position, enemyAlertRangeAroundEndZone);
        }
    }
#endif
}
