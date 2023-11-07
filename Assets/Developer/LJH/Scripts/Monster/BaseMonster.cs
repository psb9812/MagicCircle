using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System;

namespace Project_SL
{
    public class BaseMonster : MonoBehaviour
    {
        public enum MonsterState
        {
            Idle,
            Patrol,
            Trace,
            Attack,
            Hit,
            Return,
            Die,
            Dash,
            Stun
        }

        public Slider hpUI;
        public bool isPatrol;
        protected NavMeshAgent _navAgent;
        [SerializeField]
        protected MonsterState _mState;
        protected Animator _mAnim;
        protected Rigidbody _mRigidbody;
        protected Transform _playerTr;
        protected AudioSource _mAudioSource;
        [SerializeField]
        protected AttackStatus _attackStatus;
        [SerializeField]
        protected Transform _lockOnPos;
        protected MonsterStat _mStat;
        protected float lastDamagedID;
        protected bool isAttack;
        protected bool isHit;
        protected bool isDie;
        protected bool isSuperAmor = false;
        protected float distance;
        protected Vector3 _originPos;
        protected Action _skillAction = null;

        void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();
            _mAnim = GetComponent<Animator>();
            _mRigidbody = GetComponent<Rigidbody>();
            _mAudioSource = GetComponent<AudioSource>();
            _mStat = GetComponent<MonsterStat>();
        }
        public virtual void Start()
        {
            _mState = isPatrol ? MonsterState.Patrol : MonsterState.Idle;
            _playerTr = PlayerStatus.Player.transform;
            _attackStatus.attackCollider.enabled = false;
            _originPos = transform.position;
        }
        public virtual void Update()
        {
            distance = Vector3.Distance(_playerTr.position, transform.position);
            StateCheck();
        }
        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("PlayerAttack") && !isDie)
            {
                AttackStatus attackStatus = collider.GetComponent<AttackStatus>();
                if (attackStatus.attackID == lastDamagedID)
                {
                    return;
                }
                //Managers.Sound.Play("SlashDamage", Define.Sound.Effect, 0.15f);
                StartCoroutine(HpDecrease(attackStatus.attackDamaged));
                //PlayerStatus.instance.Player.EnemyHitEffect(transform.position + (Vector3.up * 1.4f));
                isHit = false;
                if (_mState != MonsterState.Stun)
                    HIT();
                //_mState = MonsterState.Hit;
            }
        }

        protected virtual void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("PlayerAttack") && !isDie)
            {
                print("Monster hit");
                //Managers.Sound.Play("SlashDamage", Define.Sound.Effect, 0.15f);
                StartCoroutine(HpDecrease(other.GetComponent<Skill>().Damage));
                //PlayerStatus.instance.Player.EnemyHitEffect(transform.position + (Vector3.up * 1.4f));
                isHit = false;
                if (_mState != MonsterState.Stun)
                    HIT();
                //_mState = MonsterState.Hit;
            }
        }

        public virtual void StateCheck()
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
                    //공격할 때 무조건 자리에 있기
                    _navAgent.isStopped = true;
                    _navAgent.velocity = Vector3.zero;
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

        public virtual void IDLE()
        {
            if (isDie || isHit) return;

            _mAnim.SetBool("isIdle", true);
            if (distance <= _mStat.attackDistance)
            {
                //TRACE -> ATTACK
                _mAnim.SetBool("isIdle", false);
                _mState = MonsterState.Attack;
                return;
            }
            else if (distance <= _mStat.traceDistance)
            {
                //IDLE -> TRACE
                _mAnim.SetBool("isIdle", false);
                _navAgent.isStopped = false;
                _mState = MonsterState.Trace;
            }
        }

        public virtual void PATROL() { }

        public virtual void TRACE()
        {
            if (isHit || isDie || isAttack) return;

            _mAnim.SetBool("isTrace", true);
            _navAgent.speed = _mStat.runSpeed;
            _navAgent.SetDestination(_playerTr.position);

            if (distance <= _mStat.traceDistance)
            {
                if (distance <= _mStat.attackDistance)
                {
                    //TRACE -> ATTACK
                    _mAnim.SetBool("isTrace", false);
                    _mState = MonsterState.Attack;
                    return;
                }
            }
            if (distance > _mStat.traceDistance)
            {
                //TRACE -> RETURN
                _mAnim.SetBool("isTrace", false);
                StartCoroutine(HpRegen());
                _mState = MonsterState.Return;
            }
        }

        public virtual void ATTACK() { }

        public virtual void AttackEnter()
        {
            CallAttackSound();
            TurnToPlayer();
            _attackStatus.SetRandomID();
            _attackStatus.attackCollider.enabled = true;
        }

        public virtual void AttackExit()
        {
            isAttack = false;
            _attackStatus.attackCollider.enabled = false;
            _navAgent.isStopped = false;
        }

        public virtual void HIT() { }

        public virtual void HitExit()
        {
            if (isDie) { return; }
            isHit = false;
            _navAgent.isStopped = false;
            if (_mState == MonsterState.Attack && isSuperAmor) return;  //슈퍼아머 기술이 들어갈 때 상태가 변환되는 것을 방어하는 코드
            _mState = isPatrol ? MonsterState.Patrol : MonsterState.Idle;
        }

        public virtual void RETURN() { }

        public virtual void DIE()
        {
            if (isDie) { return; }
            if (isAttack)
            {
                isAttack = false;
                _attackStatus.attackCollider.enabled = false;
            }

            _lockOnPos.gameObject.SetActive(false);
            isDie = true;
            _mAnim.SetBool("isDie", true);
            _navAgent.isStopped = true;
            _navAgent.velocity = Vector3.zero;
            GetComponent<CapsuleCollider>().enabled = false;
            _attackStatus.attackCollider.enabled = false;
            //gameObject.layer = LayerMask.NameToLayer("DiedEnemy");
            Destroy(gameObject, 8f);
        }

        protected void TurnToPlayer()
        {
            Vector3 targetDirection = _playerTr.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _mStat.rotSpeed * Time.deltaTime);
        }

        //랜덤으로 액션에 등록된 함수 중 하나를 호출
        protected virtual void UseSkill(ref Action action)
        {
            //델리게이트에 등록된 여러 함수중 한 함수의 인덱스
            int index = UnityEngine.Random.Range(0, action.GetInvocationList().Length);
            Action selectionSkill = (Action)_skillAction.GetInvocationList()[index];
            selectionSkill();
        }

        protected IEnumerator HpRegen()
        {
            float cur = 0;
            float per = 0;

            while (per < 1)
            {
                cur += Time.deltaTime;
                per = cur / 100f;

                _mStat.curHp = Mathf.Lerp(_mStat.curHp, _mStat.maxHp, per);
                hpUI.value = _mStat.curHp / _mStat.maxHp;

                if (_mStat.maxHp - _mStat.curHp <= 0.01f)
                {
                    _mStat.curHp = _mStat.maxHp;
                    hpUI.value = _mStat.curHp / _mStat.maxHp;
                    break;
                }

                yield return null;
            }
        }

        protected IEnumerator HpDecrease(float Damage)
        {
            float cur = 0;
            float per = 0;
            float prevHp = _mStat.curHp;
            _mStat.curHp -= Damage;

            while (per < 1)
            {
                cur += Time.deltaTime;
                per = cur / 0.5f;
                float value = Mathf.Lerp(prevHp, _mStat.curHp, per);
                hpUI.value = value / _mStat.maxHp;

                yield return null;
            }

            if (_mStat.curHp <= 0.0f)
            {
                hpUI.gameObject.SetActive(false);
            }
        }

        public virtual void CallAttackSound()
        {
            //Managers.Sound.Play("MonsterAttack" + UnityEngine.Random.Range(1, 2), Define.Sound.Sound3D, 0.2f, _mAudioSource);
        }
    }
}
