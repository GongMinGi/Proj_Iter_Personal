using UnityEngine;

public class BossIntroStarter : MonoBehaviour
{
    public BossIntroController bossIntroController;

    bool introIsPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("플레이어감지");
        if (other.CompareTag("Player") && introIsPlayed == false)
        {
            bossIntroController.StartBossIntro();
            introIsPlayed = true;
        }
    }

}
