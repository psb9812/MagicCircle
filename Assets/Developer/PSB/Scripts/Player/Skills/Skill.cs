using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Skill : MonoBehaviour
{
    [SerializeField]
    protected float damage;
    [SerializeField]
    protected float mpCost;

    public float Damage { get { return damage; } set { damage = value; } }
    public float MpCost { get { return mpCost; } set { mpCost = value; } }
    public abstract void UseSkill(Transform spawnPosition);
}
