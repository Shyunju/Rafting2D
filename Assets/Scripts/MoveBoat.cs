using UnityEngine;

namespace Rafting
{
    // 이제 MoveBoat는 싱글톤으로 관리되어 NetWorkManager에서 쉽게 접근할 수 있습니다.
    public class MoveBoat : Singleton<MoveBoat>
    {
        protected Rigidbody2D _rigidbody;
        [SerializeField] protected Animator[] _paddles;

        // 서버로부터 받은 타겟 위치 및 회전 값
        private Vector2 _targetPosition;
        private float _targetRotation;

        // 보간 속도 (값이 클수록 더 빠르게 서버 상태를 따라잡습니다)
        [SerializeField] private float _interpolationSpeed = 15f;

        protected override void OnAwake()
        {
            base.OnAwake();
            _rigidbody = GetComponent<Rigidbody2D>();
            
            // 시작 시 타겟 위치를 현재 위치로 초기화하여 순간이동 방지
            _targetPosition = transform.position;
            _targetRotation = _rigidbody.rotation;
        }

        private void FixedUpdate()
        {
            // 현재 위치/회전을 서버가 보내준 타겟 값으로 부드럽게 보간합니다.
            Vector2 newPosition = Vector2.Lerp(transform.position, _targetPosition, Time.fixedDeltaTime * _interpolationSpeed);
            float newRotation = Mathf.LerpAngle(_rigidbody.rotation, _targetRotation, Time.fixedDeltaTime * _interpolationSpeed);

            _rigidbody.MovePosition(newPosition);
            _rigidbody.MoveRotation(newRotation);
        }

        /// <summary>
        /// 서버로부터 보트의 상태 업데이트를 받으면 호출될 함수입니다.
        /// </summary>
        /// <param name="position">서버가 계산한 새로운 위치</param>
        /// <param name="rotation">서버가 계산한 새로운 회전값</param>
        public void OnServerStateUpdate(Vector2 position, float rotation)
        {
            _targetPosition = position;
            _targetRotation = rotation;
        }

        // TODO: 서버로부터 받은 애니메이션 트리거 이벤트에 따라 아래 함수들을 호출해야 합니다.
        // 예를 들어, 서버가 "Player 1 paddled left"라는 이벤트를 보내면,
        // 클라이언트는 그에 맞는 노의 애니메이션을 재생해야 합니다.
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
