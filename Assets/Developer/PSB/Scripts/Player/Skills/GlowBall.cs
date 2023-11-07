using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowBall : Skill
{
    [SerializeField]
    float moveDistance;

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
        Instantiate<GameObject>(gameObject, spawnPosition.position, Quaternion.identity, null);

        StartCoroutine(MoveBall(moveDistance, 3.0f));

        PlayerStatus.Player.UseMp(mpCost);
    }

    public void OnParticleCollision(GameObject other)
    {
        transform.position = transform.position;
        StopCoroutine("MoveBall");
    }
    IEnumerator MoveBall(float dist, float time)
    {
        float per = 0.0f;
        float cur = 0.0f;
        while (per < 1.0f)
        {
            cur += Time.deltaTime;
            per = cur / time;

            transform.position = Vector3.Lerp(transform.position, transform.position + Camera.main.transform.forward * dist, per);

            yield return null;
        }
    }
}
