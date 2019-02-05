using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CursorControl : MonoBehaviour {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float lightDuration;
    [SerializeField] private float minLightDuration;
    [SerializeField] private float substractTimeEachTry;
    [SerializeField] private float lightHeight = 5f;
    [SerializeField] private Material lightMaterial;
    [SerializeField] private GameObject confirmPathButton;
    [SerializeField] private GameObject resetPathButton;
    [SerializeField] private GameObject startDrawingButton;
    [SerializeField] private Image lightTimerIcon;
    [SerializeField] private GameObject centerCameraButton;
    private bool active = true;
    private bool drawingModeActive = false;
    private bool drawingLine = false;
    private float lightTimer = 0.0f;
    private Manager manager;

    public Line line;
    public bool lightOn = true;

    private void Awake() {
        mainCamera = Camera.main;
        manager = Manager.instance;
        manager.cursor = this;

        transform.position = manager.GetMousePosInWorld(mainCamera, 20f);
    }

    private void Update () {
        if (active) {
            CheckMouseInput();

            if (drawingModeActive) {
                if (drawingLine) DrawPath(); }
            else
                LightUp();
        }
    }

    //LIGHTING FUNCTION
    private void LightUp() {
        //light timer
        if (lightOn) {
            lightTimer += Time.deltaTime;
            float l_alpha = 1.0f - Mathf.Lerp(0.0f, 1.0f, lightTimer / lightDuration);
            lightMaterial.SetFloat("_AlphaValue", l_alpha);
            lightTimerIcon.fillAmount = 1 - lightTimer / lightDuration;
            if (lightTimer >= lightDuration) {
                StartDrawing();
            }
        }
    }

    //DRAWING FUNCTION
    private void DrawPath()
    {
        line.UpdateLine(manager.GetMousePosInWorld(mainCamera, manager.character.transform.position.y));
    }

    //MOUSE INPUT CHECKING
    private void CheckMouseInput() {
        //update cursor gameobject position
        if (Input.GetMouseButton(0))
            transform.position = manager.GetMousePosInWorld(mainCamera, 20f) + Vector3.up * lightHeight;

        if (drawingModeActive) {
            if (Input.GetMouseButtonDown(0) && line.CheckCanDraw(mainCamera))
                drawingLine = true;
            if (Input.GetMouseButtonUp(0))
                drawingLine = false;

            if (line.PointsList != null) {
                if (line.PointsList.Count > 5)
                    confirmPathButton.SetActive(true);
            }
        }
    }

    public void StartDrawing() {
        drawingModeActive = true;
        lightMaterial.SetFloat("_AlphaValue", 0.0f);
        lightTimer = 0.0f;
        lightOn = false;
        line.gameObject.SetActive(true);
        
        resetPathButton.SetActive(true);
        startDrawingButton.SetActive(false);
        lightTimerIcon.gameObject.SetActive(false);
        manager.mainCamera.RelocateToCharacterPos();
    }
    public void ConfirmPath() {
        SetActive(false);
        lightDuration = Mathf.Clamp(lightDuration - substractTimeEachTry, minLightDuration, lightDuration);
        lightMaterial.SetFloat("_AlphaValue", 1.0f);
        manager.EnableCharacter(line.GetPath());
    }
    public void ResetPath() {
        line.ResetLine();
        confirmPathButton.SetActive(false);
    }

    //SET PLAYER ACTIVE
    public void SetActive(bool _active = true) {
        active = _active;
        if (active) {
            lightOn = true;
            startDrawingButton.SetActive(true);
            lightTimerIcon.gameObject.SetActive(true);
            centerCameraButton.SetActive(true);
        }
        else {
            drawingModeActive = false;
            confirmPathButton.SetActive(false);
            resetPathButton.SetActive(false);
            centerCameraButton.SetActive(false);
        }
    }
}
