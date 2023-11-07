using System.Collections;
using UnityEngine;

namespace Project_SL
{
    public class AttackStatus : MonoBehaviour
    {
        public enum AttackType { Weak, Strong }

        public AttackType attackType = AttackType.Strong;
        public float attackDamaged = 0.0f;
        public float attackID = 0.0f;
        public Collider attackCollider = null;
        public WeaponTrail attackTrail = null;

        public float Damage { get { return attackDamaged; } set { attackDamaged = value; } }

        private void Awake()
        {
            attackCollider = GetComponent<Collider>();
        }

        public void SetRandomID()
        {
            attackID = Random.Range(0.0f, 100.0f);
        }

        public void AttackColliderExit()
        {
            attackCollider.enabled = false;
        }
    }
}
