using System.Collections;
using UnityEngine;

public class ProductZone : MonoBehaviour
{
    public ProcessingMachine machine;
    public float collectInterval = 0.1f;

    private Coroutine collectRoutine;

    void OnTriggerEnter(Collider other)
    {
        // 1단계 진단: 무언가 부딪히긴 했는지 확인
        //Debug.Log($"[ProductZone] 무언가 발판을 밟음: {other.name}");

        if (other.CompareTag("Player"))
        {
            //Debug.Log("[ProductZone] 밟은 오브젝트의 태그가 'Player'인 것을 확인했습니다.");

            // 2단계 진단: PlayerInventory 스크립트가 제대로 붙어있는지 확인
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            //if (inventory == null)
            //{
            //    // 만약 이 로그가 뜬다면, Player 태그가 붙은 오브젝트와 PlayerInventory 스크립트가 붙은 오브젝트가 서로 다른 거야!
            //    Debug.LogError("[ProductZone] 에러: 부딪힌 오브젝트에서 'PlayerInventory' 컴포넌트를 찾을 수 없습니다! (GetComponent 실패)");
            //}

            //if (machine == null)
            //{
            //    Debug.LogError("[ProductZone] 에러: 인스펙터 창에서 가공소(Machine) 오브젝트가 연결되지 않았습니다!");
            //}

            if (inventory != null && machine != null)
            {
                //Debug.Log($"[ProductZone] 모든 조건 만족! 수집 코루틴을 시작합니다. 현재 기계 창고 상태: {machine.HasProducts()}");
                collectRoutine = StartCoroutine(CollectRoutine(inventory));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("[ProductZone] 플레이어가 발판에서 나갔습니다.");
            if (collectRoutine != null) StopCoroutine(collectRoutine);
        }
    }

    IEnumerator CollectRoutine(PlayerInventory inventory)
    {
        while (machine.HasProducts())
        {
            GameObject product = machine.PopProduct();
            if (product != null)
            {
                inventory.AddProductToHand(product);
                //Debug.Log("[ProductZone] 기계에서 수갑을 하나 빼서 플레이어 손으로 전달했습니다.");
            }
            yield return new WaitForSeconds(collectInterval);
        }
    }
}