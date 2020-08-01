using System.Collections.Generic;
using UnityEngine;

public class FinalDestructibleObject : MonoBehaviour
{
    [SerializeField] private float force = 10;
    [SerializeField] private List<Rigidbody> rigidBodies;

    internal void BlastFinalObject()
    {
        foreach (var rigidbody in rigidBodies)
        {
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            rigidbody.AddForce(transform.forward * Random.Range(25, force), ForceMode.Force);
        }
    }
}
