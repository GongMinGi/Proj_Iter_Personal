using UnityEngine;

public class LightningCollider : MonoBehaviour
{
    public int lightningDamage = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponentInChildren<PlayerHealth>().TakeDamage(lightningDamage, transform.position);
        } 
    }
}
