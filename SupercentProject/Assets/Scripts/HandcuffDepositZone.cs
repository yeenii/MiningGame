using UnityEngine;

public class HandcuffDepositZone : MonoBehaviour
{
    [Header("References")]
    [Tooltip("명령을 전달할 수갑 테이블 본체")]
    public HandcuffDesk targetDesk;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            // 플레이어가 맞고 연결된 데스크가 있다면 적재 시작
            if (inventory != null && targetDesk != null)
            {
                targetDesk.StartDeposit(inventory);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetDesk != null)
            {
                targetDesk.StopDeposit();
            }
        }
    }
}