using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Skill
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UseSkill(Transform spawnPosition)
    {
        Instantiate<GameObject>(gameObject, spawnPosition.position, Quaternion.LookRotation(Camera.main.transform.forward), spawnPosition);

        PlayerStatus.Player.UseMp(mpCost);
    }

    public void OnParticleCollision(GameObject other)
    {
        Destroy(gameObject, 1.0f);
    }
}
