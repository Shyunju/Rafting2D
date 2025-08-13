using UnityEngine;
using System.Collections;

namespace Rafting
{
    public class PaddleAI : MonoBehaviour
    {
        [SerializeField] float _moveDistance = 0.5f; // 이동 거리 (엑스축 기준)
        [SerializeField] float _rotateAngle = 3.0f;  // 회전 각도 (제트축) 야수면 위로 음수면 아래로
        [SerializeField] float _loopTime = 2.0f;
        [SerializeField] Animator[] _paddles;
        float _duration = 1.0f;     // 애니메이션 재생 시간(초)
        Rigidbody2D _rigidbody;
        int _paddleIdx;
        int _dir;
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            StartCoroutine(MoveAndRotate());
        }

        IEnumerator MoveAndRotate()
        {
            Debug.Log("coroutine");
            _paddleIdx = Random.Range(0, 4);
            _paddles[_paddleIdx].SetTrigger("PushButton");
            _dir = _paddleIdx % 2 == 0 ? -1 : 1;
            Vector2 startPos = _rigidbody.position;
            Vector2 targetPos = startPos + Vector2.right * _moveDistance;

            float startRot = _rigidbody.rotation;
            float targetRot = startRot + _dir * _rotateAngle;

            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.fixedDeltaTime;
                float t = elapsed / _duration;

                // 위치 보간
                Vector2 newPos = Vector2.Lerp(startPos, targetPos, t);
                _rigidbody.MovePosition(newPos);

                // 회전 보간
                float newRot = Mathf.Lerp(startRot, targetRot, t);
                _rigidbody.MoveRotation(newRot);

                yield return new WaitForFixedUpdate();
            }

            // 최종 위치/각도 정밀 보정
            _rigidbody.MovePosition(targetPos);
            _rigidbody.MoveRotation(targetRot);

            yield return new WaitForSeconds(_loopTime);
            StartCoroutine(MoveAndRotate());
        }

    }
}
