using UnityEngine;

namespace Project_SL
{
    public class WeaponTrail : MonoBehaviour
    {
        private ParticleSystem weaponTrail;

        private void Start()
        {
            weaponTrail = GetComponent<ParticleSystem>();
            StopWeaponTrail();
        }

        public void PlayWeaponTrail()
        {
            StopWeaponTrail();

            weaponTrail.Play();
        }

        public void StopWeaponTrail()
        {
            weaponTrail.Stop();
        }
    }
}
