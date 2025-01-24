using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip anim;
    [SerializeField] private TextMeshPro text;
    
    private Camera cam;

    private GameObject owner;
    
    private float damage;

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        Collider[] size = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity);

        if (size.Length > 0)
        {
            foreach (Collider coll in size)
            {
                if (coll.TryGetComponent(out DamageNumber damageNumber) && damageNumber.owner == owner)
                {
                    if (damageNumber.GetHashCode() > GetHashCode())
                    {
                        damageNumber.Merge(damage);
                        Destroy();
                        break;
                    }
                }
            }
        }
        
        cam = Camera.main;
        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 0.25f), Random.Range(-0.5f, 0.5f));
    }

    /// <summary>
    /// Call this function when spawning a DamageNumber object.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="damage"></param>
    public void Initialise(GameObject owner, float damage)
    {
        this.owner = owner;
        SetDamage(damage);
    }

    void SetDamage(float damage)
    {
        this.damage = damage;
        text.text = Mathf.Floor(damage).ToString();
    }

    void LateUpdate()
    {
        transform.parent.LookAt(cam.transform);
    }

    void Merge(float newDamage)
    {
        SetDamage(newDamage + damage);
        animator.Play(anim.name, 0, 0);;
    }
    
    //Called in Animation
    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
