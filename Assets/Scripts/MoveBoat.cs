using UnityEngine;
using System.Collections;

namespace Rafting
{
    public class MoveBoat : MonoBehaviour
    {
        protected Rigidbody2D _rigidbody;
        [SerializeField] protected float _moveDistance = 0.5f; // 이동 거리 (엑스축 기준)
        [SerializeField] protected float _rotateAngle = 1.0f;  // 회전 각도 (제트축) 야수면 위로 음수면 아래로
        [SerializeField] protected Animator[] _paddles;
        protected float _duration = 1.0f;     // 애니메이션 재생 시간(초)
        protected float _force = 5f;
        protected private Vector2 lastVelocity;    // 이전 종료 속도 (필요하면)
        protected  float lastRotation;      // 이전 종료 각도
        protected int _paddleIdx;
        protected int _dir;
        [SerializeField] protected float _loopTime = 1.0f;  //시간의 차이가 잘 느껴지지 않음
        protected void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            // 초기 저장
            lastVelocity = _rigidbody.linearVelocity;
            lastRotation = _rigidbody.rotation;
        }
        protected IEnumerator MoveAndRotate(int totalDir, int inputCount)
        {
            // 시작 시점 위치/회전 고정
            Vector2 startVelocity = _rigidbody.linearVelocity;
            // 입력 수에 비례하여 힘 증가
            Vector2 targetVelocity = Vector2.right.normalized * (_force * inputCount);

            float startRotation = _rigidbody.rotation;
            // 모든 입력의 방향을 합산하여 회전 각도 계산
            float targetRotation = startRotation + totalDir * _rotateAngle;

            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.fixedDeltaTime;
                float t = elapsed / _duration;

                // 속도 보간
                _rigidbody.linearVelocity = Vector2.Lerp(startVelocity, targetVelocity, t);

                // 회전 보간
                float newRot = Mathf.LerpAngle(startRotation, targetRotation, t);
                _rigidbody.MoveRotation(newRot);

                yield return new WaitForFixedUpdate();
            }

            // 최종 위치 및 회전 확정
            _rigidbody.linearVelocity = targetVelocity;
            _rigidbody.MoveRotation(targetRotation);

            // ★ 종료 상태 저장
            lastVelocity = _rigidbody.linearVelocity;
            lastRotation = _rigidbody.rotation;
        }
    }
}
