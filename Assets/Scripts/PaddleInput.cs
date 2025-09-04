using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rafting
{
    public class PaddleInput : MonoBehaviour
    {
        Rigidbody2D _rigidbody;
        [SerializeField] float _moveDistance; // 이동 거리 (엑스축 기준)
        [SerializeField] float _rotateAngle;  // 회전 각도 (제트축) 양수면 위로 음수면 아래로
        [SerializeField] Animator[] _paddles;
        float _duration = 1.0f;     // 애니메이션 재생 시간(초)
        float _force = 5f;
        private Vector2 lastVelocity;    // 이전 종료 속도 (필요하면)
        private float lastRotation;      // 이전 종료 각도

        // 동시 입력을 처리하기 위한 변수
        private readonly List<int> _pendingDirections = new List<int>();
        private Coroutine _inputProcessingCoroutine;
        private const float INPUT_COLLECTION_TIME = 0.1f; // 입력을 수집하는 시간 (초)  협력성을 고려하면 더 길게 해야할지도

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            // 초기 저장
            lastVelocity = _rigidbody.linearVelocity;
            lastRotation = _rigidbody.rotation;
        }

        // 입력 처리 로직
        private void ProcessInput(int dir)
        {
            _pendingDirections.Add(dir);
            if (_inputProcessingCoroutine == null)
            {
                _inputProcessingCoroutine = StartCoroutine(ProcessPendingInputs());
            }
        }

        IEnumerator ProcessPendingInputs()
        {
            // 짧은 시간 동안 추가 입력을 기다림
            yield return new WaitForSeconds(INPUT_COLLECTION_TIME);

            int totalDir = 0;
            foreach (int dir in _pendingDirections)
            {
                totalDir += dir;
            }

            int inputCount = _pendingDirections.Count;

            _pendingDirections.Clear();
            _inputProcessingCoroutine = null;

            if (inputCount > 0)
            {
                // 수집된 입력을 기반으로 이동 및 회전 실행
                StartCoroutine(MoveAndRotate(totalDir, inputCount));
            }
        }

        void OnLeftUp()  //a0
        {
            _paddles[0].SetTrigger("PushButton");
            ProcessInput(-1);
        }
        void OnLeftDown()  //s1
        {
            _paddles[1].SetTrigger("PushButton");
            ProcessInput(1);
        }
        void OnRightUp() //d2
        {
            _paddles[2].SetTrigger("PushButton");
            ProcessInput(-1);
        }
        void OnRightDown()  //f3
        {
            _paddles[3].SetTrigger("PushButton");
            ProcessInput(1);
        }
        
        IEnumerator MoveAndRotate(int totalDir, int inputCount)
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