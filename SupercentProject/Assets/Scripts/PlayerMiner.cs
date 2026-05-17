using UnityEngine;

public class PlayerMiner : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    [Tooltip("손에 쥐어준 곡괭이 오브젝트를 여기에 연결하세요.")]
    public GameObject pickaxeObject; // 추가된 부분

    [Header("Mining Status")]
    private Transform targetOre = null;
    //public float rotationSpeed = 10f;

    //인벤토리 스크립트
    private PlayerInventory inventory;

    void Start()
    {
        // 시작할 때 같은 캐릭터에 붙어있는 인벤토리 컴포넌트를 자동으로 찾아옴
        inventory = GetComponent<PlayerInventory>();

        // 시작할 때는 곡괭이를 숨겨둠
        if (pickaxeObject != null)
        {
            pickaxeObject.SetActive(false);
        }
    }

    //void Update()
    //{
    //    if (targetOre != null)
    //    {
    //        Vector3 direction = (targetOre.position - transform.position).normalized;
    //        direction.y = 0;

    //        if (direction != Vector3.zero)
    //        {
    //            Quaternion lookRotation = Quaternion.LookRotation(direction);
    //            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    //        }
    //    }
    //}

    public void OnPickaxeHit() //광석 캐는 애니메이션 -> 광석에 데미지 전달
    {
        // 1. 타겟 광석이 있고, 2. 인벤토리에 꽉 차지 않았을 때만 채광 진행
        if (targetOre != null && inventory != null && inventory.CanCollect())
        {
            Ore ore = targetOre.GetComponent<Ore>();

            // 3. 데미지를 주는데, 그 결과로 광석이 부서졌다면(true)
            if (ore != null && ore.TakeDamage(1))
            {
                // 4. 인벤토리에 즉시 아이템을 생성해서 쌓음
                inventory.AddItemInstantly();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ore"))
        {
            targetOre = other.transform;
            animator.SetBool("IsMining", true);

            // 곡괭이 꺼내기
            if (pickaxeObject != null) pickaxeObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ore") && targetOre == other.transform)
        {
            targetOre = null;
            animator.SetBool("IsMining", false);

            // 곡괭이 숨기기
            if (pickaxeObject != null) pickaxeObject.SetActive(false);
        }
    }
}