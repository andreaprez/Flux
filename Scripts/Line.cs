using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float groundOffset = 0.1f;
    [SerializeField] private float distanceBetweenPoints = 0.1f;
    private List<Vector3> points;

    public List<Vector3> PointsList { get { return points; } }

    private void Awake() {
        lineRenderer.material.renderQueue = 5000;
    }

    public void UpdateLine (Vector3 _mousePos) {
        ///Transform mouse to world point
        Vector3 l_mouseWorldPos = _mousePos + Vector3.up * groundOffset;

        if (points == null) {
            points = new List<Vector3>();
            SetPoint(l_mouseWorldPos);
            return;
        }

        if (Vector3.Distance(points[points.Count - 1], l_mouseWorldPos) > distanceBetweenPoints)
            SetPoint(l_mouseWorldPos);
    }

    private void SetPoint(Vector3 _point) {
        points.Add(_point);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, _point);
    }

    public Vector3[] GetPath() { audioSource.Stop(); return points.ToArray(); }
    public void ResetLine() { lineRenderer.positionCount = 0; points = null; }
    public void RemoveFirstPoint() {
        if (lineRenderer.positionCount > 0) {
            for (int i = 0; i < lineRenderer.positionCount - 1; i++) {
                lineRenderer.SetPosition(i, lineRenderer.GetPosition(i + 1));
            }
            lineRenderer.positionCount--;
        }
    }
    public bool CheckCanDraw(Camera _mainCamera)
    {
        if (points == null)
            return Vector3.Distance(Manager.instance.GetMousePosInWorld(_mainCamera, Manager.instance.startZone.transform.position.y), Manager.instance.startZone.transform.position) <= Manager.instance.startZone.Range;
        else {
            return Vector3.Distance(Manager.instance.GetMousePosInWorld(_mainCamera, points[points.Count-1].y), points[points.Count - 1]) <= Manager.instance.startZone.Range;
        }
    }
}
