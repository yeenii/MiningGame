using UnityEngine;

public class QuarterViewCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("카메라가 따라다닐 플레이어 오브젝트를 연결하세요.")]
    public Transform target;

    [Header("Camera Settings")]
    [Tooltip("플레이어로부터 카메라가 떨어져 있을 거리와 높이(벡터)")]
    public Vector3 offset = new Vector3(0f, 10f, -10f);

    [Tooltip("카메라가 따라가는 속도 (낮을수록 부드럽게, 높을수록 딱 붙어서 이동)")]
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        // 타겟이 없으면 에러를 방지하기 위해 실행하지 않음
        if (target == null) return;

        // 1. 카메라가 있어야 할 목표 위치 계산 (플레이어 위치 + 오프셋)
        Vector3 desiredPosition = target.position + offset;

        // 2. 현재 위치에서 목표 위치로 부드럽게 이동 (선형 보간)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 3. 카메라 위치 적용
        transform.position = smoothedPosition;

        // (선택) 카메라가 항상 플레이어를 정면으로 쳐다보게 하려면 아래 주석을 해제하세요.
        // 하지만 쿼터뷰는 각도를 인스펙터에서 고정해두는 것이 더 깔끔할 때가 많습니다.
        // transform.LookAt(target);
    }
}