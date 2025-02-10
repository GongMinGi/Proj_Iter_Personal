using UnityEngine;

public class PhaseTwoIntro : StateMachineBehaviour
{
    private GameObject boss;
    private SpriteRenderer bossSprite;
    private Color startColor;
    private Color endColor;

    public float colorChanageDuration = 2f;
    public float elapsedTime = 0;
    public float lerpSpeed = 0.001f;
    /*
     *  * 2 ������γѾ �� �����ؾ� �� ��.
     *    - ������ �������·� �����.  (O)
     *    - ������ ������ ���������� �����.
     *    - �������¸� �����Ѵ�.
     *    - 2�������� idle,  attack, lightning  ������Ʈ�� �̵���Ų��.
     */

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.gameObject;
        bossSprite = boss.GetComponent<SpriteRenderer>();

        startColor = bossSprite.color;
        endColor = new Color(1, 0, 0, startColor.a);


        // �Ʒ�ó�� �ۼ��ϸ� while���� ��� ������ 1������ ������ �۵��ϱ� �빮��,
        // lerp�� �󸶳� ����ߵ� �߰����� ���� ȭ�鿡 �ݿ��� �� ����.

        //while (elapsedTime < colorChanageDuration)
        //{
        //    elapsedTime += Time.deltaTime * lerpSpeed;
        //    bossSprite.color = Color.Lerp(startColor, endColor, elapsedTime / colorChanageDuration);
        //}

        //if (elapsedTime > colorChanageDuration)
        //{
        //    bossSprite.color = endColor;
        //    animator.SetTrigger("Idle");

        //}

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (elapsedTime < colorChanageDuration)
        {
            elapsedTime += Time.deltaTime * lerpSpeed;
            bossSprite.color = Color.Lerp(startColor, endColor, elapsedTime / colorChanageDuration);
        }

        else if (elapsedTime > colorChanageDuration)
        {
            bossSprite.color = endColor;
            animator.SetTrigger("Idle");

        }



    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}


}
