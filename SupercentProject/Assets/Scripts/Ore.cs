using System.Collections;
using UnityEngine;

public class Ore : MonoBehaviour
{
    [Header("Ore Settings")]
    public int maxHp = 1;                  // 광석의 총 내구도 (예: 3번 치면 파괴)
    private int currentHp;
    public float respawnTime = 10f;         // 재생성 대기 시간 (초)

    [Header("Prefabs & Visuals")]
    //public GameObject dropItemPrefab;      // 캐졌을 때 튀어나올 작은 석탄 아이템 프리팹
    public MeshRenderer meshRenderer; // GameObject 대신 MeshRenderer 사용
    private Collider triggerCollider;       // 플레이어를 감지하는 트리거 콜라이더

    void Start()
    {
        currentHp = maxHp;
        triggerCollider = GetComponent<Collider>();

        // 인스펙터에서 할당하지 않았다면 자기 자신의 MeshRenderer를 자동으로 가져옴
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    // 플레이어가 때릴 때 호출될 함수
    public bool TakeDamage(int amount)
    {
        // 1. 이미 체력이 0 이하면 false 반환 (안 부서짐)
        if (currentHp <= 0) return false;

        currentHp -= amount;

        // 2. 이번 타격으로 체력이 0이 되었다면
        if (currentHp <= 0)
        {
            StartCoroutine(RespawnRoutine());
            return true; // 이번 타격으로 인해 광석이 부서졌다는 신호(true)를 보냄!
        }

        // 3. 체력이 깎였지만 아직 안 부서졌다면 false 반환
        return false;
    }

    // 아이템 생성 로직
    //void SpawnDropItem()
    //{
    //    if (dropItemPrefab != null)
    //    {
    //        // 광석 위치보다 약간 위(Y축 +0.5)에서 생성해서 자연스럽게 바닥으로 떨어지도록 함
    //        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
    //        Instantiate(dropItemPrefab, spawnPosition, Quaternion.identity);
    //    }
    //}

    // 리스폰 비비동기 루틴 (코루틴)
    IEnumerator RespawnRoutine()
    {
        // 1. 오브젝트 전체를 끄는 대신, 화면에 그리기(Rendering)와 충돌(Collision)만 비활성화
        // 이렇게 하면 게임 오브젝트 자체는 살아있으므로 코루틴이 정상적으로 계속 돌아감!
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (triggerCollider != null) triggerCollider.enabled = false;

        // 2. 지정된 시간만큼 안전하게 대기
        yield return new WaitForSeconds(respawnTime);

        // 3. 체력 초기화 및 다시 그리기/충돌 활성화
        currentHp = maxHp;
        if (meshRenderer != null) meshRenderer.enabled = true;
        if (triggerCollider != null) triggerCollider.enabled = true;
    }
}