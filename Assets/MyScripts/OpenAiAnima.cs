using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using UnityEngine.AI;

public class OpenAiAnima : MonoBehaviour
{
    public static OpenAiAnima instance; // 싱글톤 인스턴스
    private Transform myT;// start 위치
    private void Awake()
    {
        myT=GetComponent<Transform>();
        myT=this.transform;

        // 처음 인스턴스가 null인 경우 (시작되는 경우), 현재 인스턴스를 지정
        if (instance == null)
        {
            instance = this;
        }
        // 만약 중복되는 instance가 존재한다면, 파괴
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    //네비게이션 설정
    public Transform target;
    private NavMeshAgent agent;

    //에니메이션
    private static Animator animator;
    public GameObject CprMan;//cpr 액션시 활성화될 캐릭터


    public void animaIndex(int index)
    {
        this.target = myT;
        switch (index)
        {
            case 1:
                print("손인사");
                animator.SetTrigger("hello");
                break;
            case 2:
                print("cpr");
                StartCoroutine(cpr());
                animator.SetTrigger("cpr");
                break;
            case 3:
                print("악수");
                animator.SetTrigger("hand");
                break;
        }
        
    }
    IEnumerator cpr()
    {
        CprMan.SetActive(true);
        yield return new WaitForSeconds(8.567f);
        CprMan.SetActive(false); 
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
