using UnityEngine;

public class PlayerMiner : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    [Tooltip("¼Õ¿¡ Áã¾îÁØ °î±ªÀÌ ¿ÀºêÁ§Æ®¸¦ ¿©±â¿¡ ¿¬°áÇÏ¼¼¿ä.")]
    public GameObject pickaxeObject; // Ãß°¡µÈ ºÎºÐ

    [Header("Mining Status")]
    private Transform targetOre = null;
    public float rotationSpeed = 10f;

    void Start()
    {
        // ½ÃÀÛÇÒ ¶§´Â °î±ªÀÌ¸¦ ¼û°ÜµÒ
        if (pickaxeObject != null)
        {
            pickaxeObject.SetActive(false);
        }
    }

    void Update()
    {
        if (targetOre != null)
        {
            Vector3 direction = (targetOre.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ore"))
        {
            targetOre = other.transform;
            animator.SetBool("IsMining", true);

            // °î±ªÀÌ ²¨³»±â
            if (pickaxeObject != null) pickaxeObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ore") && targetOre == other.transform)
        {
            targetOre = null;
            animator.SetBool("IsMining", false);

            // °î±ªÀÌ ¼û±â±â
            if (pickaxeObject != null) pickaxeObject.SetActive(false);
        }
    }
}