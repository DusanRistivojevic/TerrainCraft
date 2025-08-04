using UnityEngine;
using System.Collections;
namespace TerrainCraft
{
    public class FixParticle : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            var particle = GetComponent<ParticleSystem>().main;
            particle.startRotation = Mathf.Deg2Rad * -transform.eulerAngles.y;
        }
    }
}