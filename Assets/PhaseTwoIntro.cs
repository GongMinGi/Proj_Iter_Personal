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
     *  * 2 페이즈로넘어갈 때 구현해야 할 것.
     *    - 보스를 무적상태로 만든다.  (O)
     *    - 보스의 색상을 빨간색으로 만든다.
     *    - 무적상태를 해제한다.
     *    - 2페이즈의 idle,  attack, lightning  스테이트로 이동시킨다.
     */

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.gameObject;
        bossSprite = boss.GetComponent<SpriteRenderer>();

        startColor = bossSprite.color;
        endColor = new Color(1, 0, 0, startColor.a);


        // 아래처럼 작성하면 while문의 모든 동작이 1프레임 내에서 작동하기 대문에,
        // lerp를 얼마나 사용했든 중간값이 게임 화면에 반영될 수 없다.

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
