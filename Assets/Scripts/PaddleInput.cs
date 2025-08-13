using System.Collections;
using UnityEngine;

namespace Rafting
{
    public class PaddleInput : MonoBehaviour
    {
        Rigidbody2D _rigidbody;
        [SerializeField] float _moveDistance = 0.5f; // 이동 거리 (엑스축 기준)
        [SerializeField] float _rotateAngle = 3.0f;  // 회전 각도 (제트축) 야수면 위로 음수면 아래로
        [SerializeField] Animator[] _paddles;
        float _duration = 1.0f;     // 애니메이션 재생 시간(초)
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
        void OnLeftUp()  //a0
        {
            _paddles[0].SetTrigger("PushButton");
            StartCoroutine(MoveAndRotate(-1));
        }
        void OnLeftDown()  //s1
        {
            _paddles[1].SetTrigger("PushButton");
            StartCoroutine(MoveAndRotate(1));
        }
        void OnRightUp() //d2
        {
            _paddles[2].SetTrigger("PushButton");
            StartCoroutine(MoveAndRotate(-1));
        }
        void OnRightDown()  //f3
        {
            _paddles[3].SetTrigger("PushButton");
            StartCoroutine(MoveAndRotate(1));
        }
        
        IEnumerator MoveAndRotate(int dir)
        {
            Vector2 startPos = _rigidbody.position;
            Vector2 targetPos = startPos + Vector2.right * _moveDistance;

            float startRot = _rigidbody.rotation;
            float targetRot = startRot + dir * _rotateAngle;

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
        }
    }
}
