using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : Enemy {

    private enum StaticEnemyState {
        Idle,
        Catch
    }

    private StaticEnemyState state;

    protected override void Awake() {
        base.Awake();
    }

    private void Update() {
        if (Destroying) return;
        switch (state) {
            case StaticEnemyState.Idle:
                if (Vector3.Distance(transform.position, manager.character.transform.position) < viewDistance) {
                    Vector3 forwardToCharacter = (manager.character.transform.position - transform.position).normalized;
                    RaycastHit hitInfo;
                    if (Physics.Raycast(transform.position, forwardToCharacter, out hitInfo, viewDistance, detectionMask)) {
                        ChangeState(StaticEnemyState.Catch);
                        break;
                    }
                }
                break;
            case StaticEnemyState.Catch:
                CatchBehaviour();
                break;
        }
    }

    public override void Restart() {
        base.Restart();
        animator.SetTrigger("restart");
        ChangeState(StaticEnemyState.Idle);
    }

    private void ChangeState(StaticEnemyState newState) {
        OnEnterState(newState);
        state = newState;
    }

    private void OnEnterState(StaticEnemyState newState) {
        switch (newState) {
            case StaticEnemyState.Idle:
                break;
            case StaticEnemyState.Catch:
                //manager.character.active = false;
                //manager.cursor.line.ResetLine();
                animator.SetTrigger("attack");
                audioSource.PlayOneShot(attackAudioClip);
                StartCoroutine(CurveSpeed(speed, speedOnDash, curveDuration));
                break;
        }
    }

    private void OnExitState() {
        switch (state) {
            case StaticEnemyState.Idle:
                originalSpeed = speed = manager.character.Speed;
                break;
            case StaticEnemyState.Catch:
                break;
        }
    }

    public override bool CanDie() { return state != StaticEnemyState.Catch; }
}
