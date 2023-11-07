using System;
using System.Collections;
using UnityEngine;

namespace Project_SL
{
    public class WereWolfMonster : BaseMonster
    {

        [SerializeField]
        public Transform[] wayPoint;
        [SerializeField]
        int _wayidx;
        [SerializeField]
        AttackStatus _leftArmAttackStatus;

        public override void Start()
        {
            base.Start();
            isSuperAmor = false;

            _wayidx = 0;
            _skillAction += BasicAttack;
            _skillAction += ClapAttack;


        }
        public override void Update()
        {
            if (isDie) return;

            base.Update();
        }

        public override void StateCheck()
        {
            switch (_mState)
            {
                case MonsterState.Idle:
                    IDLE();
                    break;
                case MonsterState.Patrol:
                    PATROL();
                    break;
                case MonsterState.Trace:
                    TRACE();
                    break;
                case MonsterState.Attack:
                    ATTACK();
                    break;
                case MonsterState.Hit:
                    HIT();
                    break;
                case MonsterState.Return:
                    RETURN();
                    break;
                case MonsterState.Die:
                    DIE();
                    break;

            }
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

            if (Vector3.Distance(transform.position, wayPoint[_wayidx % wayPoint.Length].position) <= 0.3f)
            {
                _wayidx++;
            }
        }
        public override void TRACE()
        {
            AttackColliderOn(false, false);
            base.TRACE();
        }
        public override void ATTACK()
        {
            if (isAttack || isDie || isHit) return;

            isAttack = true;
            TurnToPlayer();

            //어떤 스킬 쓸지 결정
            UseSkill(ref _skillAction);



        }

        public void BasicAttack()
        {
            _navAgent.isStopped = true;
            _navAgent.velocity = Vector3.zero;
            _mAnim.SetTrigger("isAttack");
        }

        public void ClapAttack()
        {
            isSuperAmor = true;
            _mAnim.SetTrigger("isSkill");
        }

        public void JumpAttackEnter()
        {
            CallAttackSound();
            _navAgent.isStopped = false;
            _navAgent.SetDestination(_playerTr.position);
            isSuperAmor = true;
            TurnToPlayer();

        }

        public void JumpAttackTop()
        {
            _navAgent.SetDestination(_playerTr.position);
            TurnToPlayer();
            ChangeAttackId();
            AttackColliderOn(true, true);
        }

        public void JumpAttackExit()
        {
            AttackExit();
        }

        public void ClapAttackExit()
        {
            isSuperAmor = false;
            isAttack = false;
            AttackColliderOn(false, false);
            _navAgent.isStopped = false;

            if (distance <= _mStat.attackDistance)
            {
                _navAgent.isStopped = true;
                _mState = MonsterState.Idle;
                return;
            }
            _navAgent.isStopped = false;
            _mState = MonsterState.Trace;
        }

        public void RightHandAttackEnter()
        {
            CallAttackSound();
            TurnToPlayer();
            ChangeAttackId();
            AttackColliderOn(false, true);
        }

        public void LeftHandAttackEnter()
        {
            CallAttackSound();
            TurnToPlayer();
            ChangeAttackId();
            AttackColliderOn(true, false);
        }

        public override void AttackEnter()
        {
            CallAttackSound();
            TurnToPlayer();
            ChangeAttackId();
            AttackColliderOn(true, true);
        }


        public override void AttackExit()
        {
            base.AttackExit();
            AttackColliderOn(false, false);
            isSuperAmor = false;
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
            }
            //이미 죽거나 맞고 있는 중이거나 슈퍼 아머라면 히트 애니메이션은 무시
            if (isDie || isHit || isSuperAmor) { return; }

            if (isAttack)   //슈퍼 아머가 아닌 공격을 할 때는 공격 취소
            {
                isAttack = false;
                AttackColliderOn(false, false);
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
            AttackColliderOn(false, false);
            base.DIE();
        }

        public void ChangeAttackId()
        {
            _leftArmAttackStatus.SetRandomID();
            _attackStatus.SetRandomID();
        }

        public void AttackColliderOn(bool leftArm, bool rightArm)
        {
            _attackStatus.attackCollider.enabled = rightArm;
            _leftArmAttackStatus.attackCollider.enabled = leftArm;
        }

        public void FootStep()
        {
            Managers.Sound.Play("GrassFootStep" + UnityEngine.Random.Range(1, 5), Define.Sound.Sound3D, 0.2f, _mAudioSource);
        }

    }
}
