using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rafting
{
    // 이제 MoveBoat는 싱글톤으로 관리되어 NetWorkManager에서 쉽게 접근할 수 있습니다.
    public class MoveBoat : MonoBehaviour
    {
        [SerializeField] protected float _moveDistance = 0.5f; // 이동 거리 (엑스축 기준)
        [SerializeField] protected float _rotateAngle = 1.0f;  // 회전 각도 (제트축) 야수면 위로 음수면 아래로
        [SerializeField] protected Animator[] _paddles;
        
        // 보간 속도 (값이 클수록 더 빠르게 서버 상태를 따라잡습니다)
        [SerializeField] private float _interpolationSpeed = 15f;
        protected Rigidbody2D _rigidbody;
        protected float _duration = 1.0f;     // 애니메이션 재생 시간(초)
        protected float _force = 5f;
        protected private Vector2 lastVelocity;    // 이전 종료 속도 (필요하면)
        protected float lastRotation;      // 이전 종료 각도
        protected int _paddleIdx;
        protected int _dir;

        // 동시 입력을 처리하기 위한 변수
        readonly List<int> _pendingDirections = new List<int>();
        Coroutine _inputProcessingCoroutine;
        const float INPUT_COLLECTION_TIME = 0.1f; // 입력을 수집하는 시간 (초)  협력성을 고려하면 더 길게 해야할지도

        protected virtual void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            // 초기 저장
            lastVelocity = _rigidbody.linearVelocity;
            lastRotation = _rigidbody.rotation;
        }

        protected IEnumerator MoveAndRotateCo(int totalDir, int inputCount)
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

        // 입력 처리 로직
        public void ProcessInput(int dir)
        {
            _pendingDirections.Add(dir);
            if (_inputProcessingCoroutine == null)
            {
                _inputProcessingCoroutine = StartCoroutine(ProcessPendingInputs());
            }
        }

        IEnumerator ProcessPendingInputs()
        {
            Debug.Log("Starting to collect inputs...");
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
                StartCoroutine(MoveAndRotateCo(totalDir, inputCount));
            }
        }

        // 서버에서 받아서 노의 애니메이션을 재생
        public void TriggerPaddleAnimation(int paddleIndex)
        {
            Debug.Log($"Triggering paddle animation for paddle index: {paddleIndex}");
            if (paddleIndex >= 0 && paddleIndex < _paddles.Length)
            {
                _paddles[paddleIndex].SetTrigger("PushButton");
            }
        }
    }
}
