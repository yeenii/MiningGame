using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandcuffDesk : MonoBehaviour
{
    [Header("Stack Settings")]
    [Tooltip("테이블 위에 수갑이 쌓일 기준점")]
    public Transform tableStackPoint;
    [Tooltip("테이블 위 수갑 적재 간격(Y축)")]
    public float tableProductHeight = 0.1f;

    [Header("Transfer Settings")]
    [Tooltip("플레이어 손에서 테이블로 옮겨지는 속도")]
    public float transferInterval = 0.1f;

    // 테이블에 쌓인 수갑들을 관리하는 리스트
    private List<GameObject> tableProducts = new List<GameObject>();
    private Coroutine depositRoutine;

    // ★ 외부 발판(Zone)에서 플레이어가 들어왔을 때 호출할 함수
    public void StartDeposit(PlayerInventory inventory)
    {
        if (depositRoutine == null)
        {
            depositRoutine = StartCoroutine(DepositRoutine(inventory));
        }
    }

    // ★ 외부 발판(Zone)에서 플레이어가 나갔을 때 호출할 함수
    public void StopDeposit()
    {
        if (depositRoutine != null)
        {
            StopCoroutine(depositRoutine);
            depositRoutine = null;
        }
    }

    // 플레이어 손 -> 테이블로 수갑을 옮기는 코루틴
    IEnumerator DepositRoutine(PlayerInventory inventory)
    {
        while (true)
        {
            GameObject product = inventory.PopProductFromHand();

            if (product != null)
            {
                product.transform.SetParent(tableStackPoint, false);

                int index = tableProducts.Count;
                product.transform.localPosition = new Vector3(0f, index * tableProductHeight, 0f);
                product.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); //테이블 위 수갑 각도 고정
                product.transform.localScale = new Vector3(20f, 20f, 20f); //테이블 위 수갑 scale 고정

                tableProducts.Add(product);
            }
            else
            {
                // 플레이어 손에 더 이상 수갑이 없으면 루프 탈출
                break;
            }

            yield return new WaitForSeconds(transferInterval);
        }

        depositRoutine = null;
    }

    // 죄수 AI가 수갑이 있는지 확인하기 위한 함수 (유지)
    public bool HasHandcuffs()
    {
        return tableProducts.Count > 0;
    }

    // 죄수 AI가 수갑을 하나 소비할 때 호출할 함수 (유지)
    public void ConsumeHandcuff()
    {
        if (!HasHandcuffs()) return;

        int lastIndex = tableProducts.Count - 1;
        GameObject topHandcuff = tableProducts[lastIndex];

        tableProducts.RemoveAt(lastIndex);
        Destroy(topHandcuff);
    }
}