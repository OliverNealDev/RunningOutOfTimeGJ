using System;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip anim;
    [SerializeField] private TextMeshPro text;
    
    private Camera cam;
    
    private float damage;

    private void Start()
    {
        cam = Camera.main;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
        text.text = damage.ToString();
    }

    void LateUpdate()
    {
        transform.parent.LookAt(cam.transform);
    }
    
    //Do like the cool merge thing later - sincerely jurgen
    
    //Called in Animation
    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
