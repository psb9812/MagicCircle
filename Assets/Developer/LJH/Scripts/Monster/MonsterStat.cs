using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_SL
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Create Enemy", order = 1)]
    public class MonsterStat : ScriptableObject
    {
        public float curHp = 100f;
        public float maxHp = 100f;
        public float walkSpeed = 1.5f;
        public float runSpeed = 2.5f;
        public float rotSpeed = 50.0f;
        public float attackDistance = 1f;
        public float traceDistance = 10f;
    }
}
