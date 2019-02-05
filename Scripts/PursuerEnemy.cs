using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PursuerEnemy : Enemy {

    private enum PursuerEnemyState {
        Idle,
        Pursue,
        Catch
    }

    [SerializeField] private float viewAngle;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private PursuerEnemyState state;

    protected override void Awake() {
        base.Awake();
        navMeshAgent.speed = speed;
    }

    private void Update() {
        if (Destroying) return;
        switch (state) {
            case PursuerEnemyState.Idle:
                #region View Angle
                /*if (Vector3.Distance(transform.position, manager.character.transform.position) < viewDistance) {
                    Vector3 characterPos = manager.character.transform.position;
                    characterPos.y = transform.position.y;
                    Vector3 forwardToCharacter = (characterPos - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, forwardToCharacter) < viewAngle / 2) {
                        forwardToCharacter = (manager.character.transform.position - transform.position).normalized;
                        RaycastHit hitInfo;
                        if (Physics.Raycast(transform.position, forwardToCharacter, out hitInfo, viewDistance, detectionMask)) {
                            ChangeState(PursuerEnemyState.Pursue);
                            Debug.Log("DETECT");
                            break;
                        }
                    }
                }*/
                #endregion
                if (Vector3.Distance(transform.position, manager.character.transform.position) < viewDistance) {
                    Vector3 forwardToCharacter = (manager.character.transform.position - transform.position).normalized;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(transform.position, forwardToCharacter, out hitInfo, viewDistance, detectionMask)) {
                        ChangeState(PursuerEnemyState.Pursue);
                        break;
                    }
                }
                break;
            case PursuerEnemyState.Pursue:
                navMeshAgent.SetDestination(manager.character.transform.position);
                if (Vector3.Distance(transform.position, manager.character.transform.position) < minDistanceToCatch) {
                    ChangeState(PursuerEnemyState.Catch);
                }

                if (Vector3.Distance(manager.character.transform.position, manager.endZone.transform.position) < manager.EnemyAlertRangeAroundEndZone) {
                    ChangeState(PursuerEnemyState.Catch);
                }
                break;
            case PursuerEnemyState.Catch:
                CatchBehaviour();
                break;
        }
    }

    public override void Restart() {
        base.Restart();
        animator.SetTrigger("restart");
        ChangeState(PursuerEnemyState.Idle);
    }

    private void ChangeState(PursuerEnemyState newState) {
        OnExitState();
        OnEnterState(newState);
        state = newState;
    }

    private void OnExitState() {
        switch (state) {
            case PursuerEnemyState.Idle:
                originalSpeed = speed = manager.character.Speed;
                navMeshAgent.isStopped = false;
                break;
            case PursuerEnemyState.Pursue:
                navMeshAgent.ResetPath();
                break;
            case PursuerEnemyState.Catch:
                break;
        }
    }

    private void OnEnterState(PursuerEnemyState newState) {
        switch (newState) {
            case PursuerEnemyState.Idle:
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;
                break;
            case PursuerEnemyState.Pursue:
                animator.SetTrigger("chase");
                speed = originalSpeed;
                navMeshAgent.speed = speed;
                navMeshAgent.acceleration = speed;
                break;
            case PursuerEnemyState.Catch:
                //manager.character.active = false;
                //manager.cursor.line.ResetLine();
                animator.SetTrigger("dash");
                audioSource.PlayOneShot(dashAudioClip);
                StartCoroutine(CurveSpeed(speed, speedOnDash, curveDuration));
                break;
        }
    }

    public override bool CanDie() { return state != PursuerEnemyState.Catch; }
}
