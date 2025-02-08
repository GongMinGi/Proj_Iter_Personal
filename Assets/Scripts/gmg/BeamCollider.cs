using UnityEngine;

public class BeamCollider : MonoBehaviour
{
    public int beamDamage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster")) // ���Ϳ� ���� ������ ó��
        {

            BaseMonster monster = other.GetComponent<BaseMonster>();
            if (monster != null)
            {
                monster.TakeDamage(beamDamage, transform.position);
            }


        }

        else if (other.CompareTag("Boss")) // ������ ���� ������ ó��
        {
            other.GetComponent<Boss>().TakeDamage(2);
        }
        else if( other.CompareTag("Destroyable"))
        {
            Destroy(other.gameObject);
            AudioManager.instance.PlaySfx(AudioManager.Sfx.RockCrush1);

        }
    }
}
