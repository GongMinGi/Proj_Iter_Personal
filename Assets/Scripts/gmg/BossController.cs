using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public IEnumerator DelayedIdle(Animator anim, float delay)
    {
        yield return new WaitForSeconds(delay);

    }

}
