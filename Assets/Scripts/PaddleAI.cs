using UnityEngine;
using System.Collections;

namespace Rafting
{
    public class PaddleAI : MoveBoat
    {
        // Rigidbody2D _rigidbody;
        // [SerializeField] float _moveDistance = 0.5f; // 이동 거리 (엑스축 기준)
        // [SerializeField] float _rotateAngle = 1.0f;  // 회전 각도 (제트축) 야수면 위로 음수면 아래로
        // [SerializeField] Animator[] _paddles;
        // float _duration = 1.0f;     // 애니메이션 재생 시간(초)
        // float _force = 5f;
        // private Vector2 lastVelocity;    // 이전 종료 속도 (필요하면)
        // private float lastRotation;      // 이전 종료 각도
        // int _paddleIdx;
        // int _dir;
        // [SerializeField] float _loopTime = 1.0f;  //시간의 차이가 잘 느껴지지 않음
        void Start()
        {
            // _rigidbody = GetComponent<Rigidbody2D>();

            // // 초기 저장
            // lastVelocity = _rigidbody.linearVelocity;
            // lastRotation = _rigidbody.rotation
            base.Start();
            StartCoroutine(SetDirectionCo());
        }
        IEnumerator SetDirectionCo()
        {
            _paddleIdx = Random.Range(0, 4);
            _paddles[_paddleIdx].SetTrigger("PushButton");
            _dir = _paddleIdx % 2 == 0 ? -1 : 1;
            StartCoroutine(MoveAndRotate(_dir, 1));
             yield return new WaitForSeconds(_loopTime);
             StartCoroutine(SetDirectionCo());
        }

        // IEnumerator MoveAndRotate()
        // {
        //     _paddleIdx = Random.Range(0, 4);
        //     _paddles[_paddleIdx].SetTrigger("PushButton");
        //     _dir = _paddleIdx % 2 == 0 ? -1 : 1;
        //     // 시작 시점 위치/회전 고정
        //     Vector2 startVelocity = _rigidbody.linearVelocity;
        //     Vector2 targetVelocity = Vector2.right.normalized * _force;

        //     float startRotation = _rigidbody.rotation;
        //     float targetRotation = startRotation + _dir * _rotateAngle;

        //     float elapsed = 0f;

        //     while (elapsed < _duration)
        //     {
        //         elapsed += Time.fixedDeltaTime;
        //         float t = elapsed / _duration;

        //         // 속도 보간
        //         _rigidbody.linearVelocity = Vector2.Lerp(startVelocity, targetVelocity, t);

        //         // 회전 보간
        //         float newRot = Mathf.LerpAngle(startRotation, targetRotation, t);
        //         _rigidbody.MoveRotation(newRot);

        //         yield return new WaitForFixedUpdate();
        //     }

        //     // 최종 위치 및 회전 확정
        //     _rigidbody.linearVelocity = targetVelocity;
        //     _rigidbody.MoveRotation(targetRotation);

        //     // ★ 종료 상태 저장
        //     lastVelocity = _rigidbody.linearVelocity;
        //     lastRotation = _rigidbody.rotation;

        //     yield return new WaitForSeconds(_loopTime);
        //     StartCoroutine(MoveAndRotate());
        // }
        

    }
}
