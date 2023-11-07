using System;
using System.Collections;
using UnityEngine;

namespace Project_SL
{
    public class KnightMonster : BaseMonster
    {
        public enum Kinght
        {
            Basic,
            Horns,
            Red
        }

        public Transform[] wayPoint;
        [SerializeField]
        int _wayidx;

        public override void Start()
        {
            base.Start();

            _wayidx = 0;
            _skillAction += BasicAttack;
            _skillAction += SkillAttack;
        }
        public override void Update()
        {
            if (isDie) return;

            base.Update();
        }
        public override void IDLE()
        {
            base.IDLE();
        }
        public override void PATROL()
        {
            if (isDie || isHit) return;

            _mAnim.SetBool("isPatrol", true);
            _navAgent.speed = _mStat.walkSpeed;

            if (distance <= _mStat.traceDistance)
            {
                //PATROL -> TRACE
                _mAnim.SetBool("isPatrol", false);
                _mState = MonsterState.Trace;
                return;
            }

            _navAgent.SetDestination(wayPoint[_wayidx % wayPoint.Length].position);

            if (Vector3.Distance(transform.position, wayPoint[_wayidx % wayPoint.Length].position) <= 0.1f)
            {
                _wayidx++;
            }
        }

        public override void TRACE()
        {
            base.TRACE();
        }

        public override void ATTACK()
        {
            if (isAttack || isDie) return;

            isAttack = true;
            TurnToPlayer();

            //어떤 스킬 쓸지 결정
            UseSkill(ref _skillAction);

            _navAgent.isStopped = true;
            _navAgent.velocity = Vector3.zero;
        }

        public void BasicAttack()
        {
            _mAnim.SetTrigger("isAttack");
        }

        public void SkillAttack()
        {
            isSuperAmor = true;
            _mAnim.applyRootMotion = true;
            _mAnim.SetTrigger("isSkill");
        }

        public void SkillAttackEnter()
        {
            CallAttackSound();
            base.AttackEnter();
            TurnToPlayer();

        }

        public void SkillAttackExit()
        {
            isSuperAmor = false;
            _mAnim.applyRootMotion = false;
        }

        public override void AttackExit()
        {
            base.AttackExit();
            if (distance <= _mStat.attackDistance)
            {
                _navAgent.isStopped = true;
                _mState = MonsterState.Idle;
                return;
            }
            _navAgent.isStopped = false;
            _mState = MonsterState.Trace;
        }

        public override void HIT()
        {
            //체력이 0이라면 바로 멈추고 죽음 상태로 보냄
            if (_mStat.curHp <= 0)
            {
                _navAgent.isStopped = true;
                _navAgent.velocity = Vector3.zero;
                _mRigidbody.isKinematic = true;
                _mState = MonsterState.Die;
                return;
            }
            //이미 죽거나 맞고 있는 중이거나 슈퍼 아머라면 히트 판정은 무시
            if (isDie || isHit || isSuperAmor) { return; }

            if (isAttack)
            {
                isAttack = false;
                _attackStatus.attackCollider.enabled = false;
            }
            isHit = true;
            _mAnim.SetTrigger("isHit");

            _navAgent.isStopped = true;
            _navAgent.velocity = Vector3.zero;
            _mRigidbody.AddForce((transform.position - _playerTr.position).normalized * 700.0f);

        }
        public override void HitExit()
        {
            base.HitExit();
            _mRigidbody.velocity = Vector3.zero;
        }
        public override void RETURN()
        {
            //RETURN -> PATROL
            if (isPatrol)
            {
                _navAgent.isStopped = false;
                _mState = MonsterState.Patrol;
                return;
            }

            _navAgent.destination = _originPos;
            //Return -> Idle
            if (Vector3.Distance(transform.position, _originPos) < 0.2f)
            {
                _navAgent.isStopped = true;
                _navAgent.velocity = Vector3.zero;
                _mState = MonsterState.Idle;
            }

        }
        public override void DIE()
        {
            base.DIE();
        }

        public void FootStep()
        {
            Managers.Sound.Play("GrassFootStep" + UnityEngine.Random.Range(1, 5), Define.Sound.Sound3D, 0.2f, _mAudioSource);
        }

    }
}
