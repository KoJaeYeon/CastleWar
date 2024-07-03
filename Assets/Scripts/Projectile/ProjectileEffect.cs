using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffect : Projectile
{
    [SerializeField] GameObject Effect_Shoot;

    protected override void InitAction()
    {
        Effect_Shoot.SetActive(true);
        Effect_Shoot.transform.localPosition = Vector3.zero;
        Effect_Shoot.transform.SetParent(null);

        var audio = Effect_Shoot.GetComponent<AudioSource>();
        audio.Play();
    }

    protected override void EndAction()
    {
        Effect_Shoot.SetActive(false);
        Effect_Shoot.transform.SetParent(transform);
    }
}
