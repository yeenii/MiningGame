using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingMachine : MonoBehaviour
{
    [Header("Drop Zone Settings")]
    [Tooltip("바닥에 광석이 쌓일 기준점 (빈 오브젝트)")]
    public Transform dropZone;
    [Tooltip("플레이어 등에서 바닥으로 즉시 이동하는 간격 (0.1초 추천)")]
    public float dropInterval = 0.1f; //**dropInterval > ProcessingTime 이어야 바닥에 광석 쌓임
    [Tooltip("바닥에 쌓일 때 아이템 간의 높이(Y축)")]
    public float itemHeight = 0.4f;

    [Header("Machine Settings")]
    [Tooltip("기계의 회전 애니메이터")]
    public Animator machineAnimator_Rotate;
    [Tooltip("기계의 상하 애니메이터")]
    public Animator machineAnimator_UpDown;
    [Tooltip("기계가 광석 1개를 가공하는 데 걸리는 시간")]
    public float processingTime = 0.5f;

    // 바닥에 쌓인 광석들을 관리하는 리스트
    private List<GameObject> groundItems = new List<GameObject>();

    // 플레이어가 내려놓는 루틴을 제어하는 변수
    private Coroutine playerDropRoutine;

    void Start()
    {
        // 게임이 시작되면 기계는 항상 바닥을 감시하며 가공 루틴을 돌림
        StartCoroutine(MachineProcessRoutine());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("바닥인식");
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            Debug.Log(inventory);
            Animator playerAnim = other.GetComponentInChildren<Animator>();

            if (inventory != null)
            {
                // 바닥에 내려놓기 시작
                playerDropRoutine = StartCoroutine(PlayerToGroundRoutine(inventory, playerAnim));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerDropRoutine != null) StopCoroutine(playerDropRoutine);

            //Animator playerAnim = other.GetComponentInChildren<Animator>();
            //if (playerAnim != null) playerAnim.SetBool("IsDropping", false);
        }
    }

    // 1. 플레이어 가방 -> 바닥으로 하나씩 즉시 던지는 루틴 (수정됨)
    IEnumerator PlayerToGroundRoutine(PlayerInventory inventory, Animator playerAnim)
    {
        //if (playerAnim != null) playerAnim.SetBool("IsDropping", true);

        while (inventory.HasItems())
        {
            // 등에서 하나 뺌
            GameObject item = inventory.PopItem();

            if (item != null)
            {
                Debug.Log("광석 쌓음", item);
                // 부모를 바닥 기준점(DropZone)으로 즉시 설정
                item.transform.SetParent(dropZone, true);

                // 바닥에 쌓여있는 개수만큼 위로 위치 즉시 지정 (Lerp 애니메이션 제거됨)
                int currentGroundCount = groundItems.Count;
                item.transform.localPosition = new Vector3(0f, currentGroundCount * itemHeight, 0f);
                item.transform.localRotation = Quaternion.identity;

                // 기계가 인식할 수 있도록 바닥 리스트에 추가
                groundItems.Add(item);
            }

            // 지정된 시간(dropInterval) 대기 후 다음 광석을 등에서 뺌
            yield return new WaitForSeconds(dropInterval);
        }

        //if (playerAnim != null) playerAnim.SetBool("IsDropping", false);
    }

    // 2. 바닥 -> 기계 가공 루틴 (백그라운드에서 무한 감시)
    IEnumerator MachineProcessRoutine()
    {
        while (true)
        {
            // 바닥에 광석이 하나라도 도착했다면!
            if (groundItems.Count > 0)
            {
                // 바닥 스택의 맨 위(마지막 인덱스) 광석을 가져옴
                int lastIndex = groundItems.Count - 1;
                GameObject itemToProcess = groundItems[lastIndex];

                // 리스트에서 제거
                groundItems.RemoveAt(lastIndex);

                // 광석 파괴 (기계 안으로 들어간 연출)
                Destroy(itemToProcess);

                // 기계 가공 애니메이션 실행 (수정된 두 개의 애니메이터 동시 작동)
                if (machineAnimator_Rotate != null && machineAnimator_UpDown != null)
                {
                    machineAnimator_Rotate.SetTrigger("Process");
                    machineAnimator_UpDown.SetTrigger("Process");
                }

                // 기계가 하나를 가공하는 동안 대기 (이 시간 동안은 바닥에 광석이 더 쌓여도 가져가지 않음)
                yield return new WaitForSeconds(processingTime);
            }
            else
            {
                // 바닥에 광석이 없으면 다음 프레임까지 대기
                yield return null;
            }
        }
    }
}