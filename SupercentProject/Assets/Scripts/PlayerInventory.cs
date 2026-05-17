using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int maxCapacity = 20;               // 최대 들 수 있는 광석 개수
    public float itemHeight = 0.4f;            // 아이템 하나당 쌓이는 높이
    public Transform backpackPoint;            // 캐릭터 등 뒤의 기준점 (빈 오브젝트)

    [Tooltip("등 뒤에 쌓일 작은 광석(석탄) 프리팹을 연결하세요.")]
    public GameObject stackedItemPrefab;

    private List<GameObject> stackedItems = new List<GameObject>();

    // 인벤토리에 여유 공간이 있는지 확인
    public bool CanCollect()
    {
        return stackedItems.Count < maxCapacity;
    }

    // 광석을 등 뒤에 즉시 생성하고 쌓는 함수
    public void AddItemInstantly()
    {
        if (!CanCollect()) return;

        // 월드 좌표 기준 안전장치 코드(필요시 대체 사용)
        GameObject newItem = Instantiate(stackedItemPrefab); // 부모를 지정하지 않고 생성

        int index = stackedItems.Count;
        // 플레이어 등 뒤 기준점의 월드 위치에서 순수한 월드 위쪽(Vector3.up)으로 쌓아 올림
        newItem.transform.position = backpackPoint.position + (Vector3.up * index * itemHeight);

        // 생성이 끝난 뒤 부모를 backpackPoint로 묶어주어 캐릭터를 따라다니게 함
        newItem.transform.SetParent(backpackPoint);
        newItem.transform.localRotation = Quaternion.identity;

        // 3. 리스트에 보관
        stackedItems.Add(newItem);
    }

    //---광석 가공 로직---

    // 인벤토리에 아이템이 하나라도 있는지 확인
    public bool HasItems()
    {
        return stackedItems.Count > 0;
    }

    // 가방 맨 위(마지막 인덱스)에서 아이템을 하나 꺼내서 반환
    public GameObject PopItem()
    {
        if (!HasItems()) return null;

        // 리스트의 가장 마지막 요소(맨 위에 쌓인 광석) 인덱스
        int lastIndex = stackedItems.Count - 1;
        GameObject topItem = stackedItems[lastIndex];

        // 리스트에서 제거
        stackedItems.RemoveAt(lastIndex);

        // 캐릭터의 등(BackpackPoint)에서 분리시켜 월드 객체로 만듦
        topItem.transform.SetParent(null);

        return topItem; // 꺼낸 광석 오브젝트를 가공소 스크립트로 넘겨줌
    }
}