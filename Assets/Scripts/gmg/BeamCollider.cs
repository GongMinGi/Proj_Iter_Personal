using UnityEngine;

public class BeamCollider : MonoBehaviour
{
    public int beamDamage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster")) // 몬스터에 대한 데미지 처리
        {

            BaseMonster monster = other.GetComponent<BaseMonster>();
            if (monster != null)
            {
                monster.TakeDamage(beamDamage, transform.position);
            }


        }

        else if (other.CompareTag("Boss")) // 보스에 대한 데미지 처리
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
