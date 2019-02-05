using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Enemy : MonoBehaviour {

    [Header("Dash Speed Curve")]
    [SerializeField] protected AnimationCurve catchAnimationCurve;
    [SerializeField] protected float curveDuration;
    [SerializeField] protected float speedOnDash;

    [Header("Sound")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip dashAudioClip;
    [SerializeField] protected AudioClip attackAudioClip;
    [SerializeField] protected AudioClip dieAudioClip;

    [Header("Variables")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected float dieTime;
    [SerializeField] protected LayerMask detectionMask;
    [SerializeField] protected float viewDistance;
    [SerializeField] protected float minDistanceToCatch = 0.2f;

    protected float speed;
    protected float originalSpeed;
    protected float aux_elapsedTime = 0;
    protected Manager manager;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private bool aux_destroying = false;

    protected bool Destroying { get { return aux_destroying; } }

    protected virtual void Awake() {
        manager = Manager.instance;
        manager.enemies.Add(this);
        originalPos = transform.position;
        originalRot = transform.rotation;
    }

    public void Destroy() {
        if (!aux_destroying) {
            aux_destroying = true;
            animator.SetTrigger("die");
            audioSource.PlayOneShot(dieAudioClip);
            Invoke("Hide", dieTime);
        }
    }

    private void Hide() {
        transform.position = originalPos;
        gameObject.SetActive(false);
        aux_destroying = false;
    }

    public virtual void Restart() {
        CancelInvoke("Hide");
        transform.position = originalPos;
        transform.rotation = originalRot;
    }

    protected void CatchBehaviour() {
        if (!aux_destroying) {
            transform.position = Vector3.MoveTowards(transform.position, manager.character.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, manager.character.transform.position) < minDistanceToCatch) {
                manager.character.Kill();
            }
        }
    }

    protected IEnumerator CurveSpeed(float _originalSpeed, float _targetSpeed, float _duration) {
        float l_elapsedTime = 0f;
        while (l_elapsedTime <= _duration) {
            l_elapsedTime += Time.deltaTime;
            float l_percent = Mathf.Clamp01(l_elapsedTime / _duration);

            float l_curvePercent = catchAnimationCurve.Evaluate(l_percent);
            speed = Mathf.LerpUnclamped(_originalSpeed, _targetSpeed, l_curvePercent);

            yield return null;
        }
    }

    public abstract bool CanDie();

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
#endif
}
