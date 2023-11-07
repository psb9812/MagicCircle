using UnityEngine;

namespace Project_SL
{
    public class HitEffect : MonoBehaviour
    {
        private void Start()
        {
            Invoke("Destroy", 1.0f);
        }

        private void Destroy()
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}
