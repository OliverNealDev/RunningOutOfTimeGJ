using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip anim;
    [SerializeField] private TextMeshPro text;
    
    private Camera cam;
    
    private float damage;

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        Collider[] size = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity);

        if (size.Length > 0)
        {
            foreach (Collider coll in size)
            {
                if (coll.TryGetComponent(out DamageNumber damageNumber))
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

    public void SetDamage(float damage)
    {
        this.damage = damage;
        text.text = damage.ToString();
    }

    void LateUpdate()
    {
        transform.parent.LookAt(cam.transform);
    }

    void Merge(float newDamage)
    {
        //Debug.Log(damage + " merging...");
        SetDamage(newDamage + damage);
        //Debug.Log(damage + " merged!");
        animator.Play(anim.name, 0, 0);;
    }
    
    //Called in Animation
    public void Destroy()
    {
        Debug.Log("Destroying... " + damage);
        Destroy(transform.parent.gameObject);
    }
}
