using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Rafting
{
    // 이제 MoveBoat는 싱글톤으로 관리되어 NetWorkManager에서 쉽게 접근할 수 있습니다.
    public class MoveBoat : MonoBehaviour
    {
        [SerializeField] protected float _rotateAngle = 2.0f;  // 회전 각도 (제트축) 양수면 위로 음수면 아래로
        [SerializeField] protected Animator[] _paddles;

        // 보간 속도 (값이 클수록 더 빠르게 서버 상태를 따라잡습니다)
        //[SerializeField] private float _interpolationSpeed = 15f;
        protected Rigidbody2D _rigidbody;
        // 서버로부터 받은 최신 상태
        private Vector2 _serverPosition;
        private float _serverRotation;

        // 보간에 사용될 변수
        private float _interpolationSpeed = 20f; // 보간 속도, 필요에 따라 조정

        protected virtual void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _serverPosition = _rigidbody.position;
            _serverRotation = _rigidbody.rotation;
        }

        void FixedUpdate()
        {
            // 현재 위치와 서버 위치 사이를 부드럽게 보간
            Vector2 newPos = Vector2.Lerp(transform.position, _serverPosition, Time.fixedDeltaTime * _interpolationSpeed);
            _rigidbody.MovePosition(newPos);

            // 현재 각도와 서버 각도 사이를 부드럽게 보간
            float newRot = Mathf.LerpAngle(transform.rotation.eulerAngles.z, _serverRotation, Time.fixedDeltaTime * _interpolationSpeed);
            _rigidbody.MoveRotation(newRot);
        }

        public void UpdateStateFromServer(float x, float y, float rot)
        {
            _serverPosition = new Vector2(x, y);
            _serverRotation = rot;
        }

        // 서버에서 받아서 노의 애니메이션을 재생
        public void TriggerPaddleAnimation(int paddleIndex)
        {
            //Debug.Log($"Triggering paddle animation for paddle index: {paddleIndex}");
            if (paddleIndex >= 0 && paddleIndex < _paddles.Length)
            {
                _paddles[paddleIndex].SetTrigger("PushButton");
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Finish")
            {
                PaddleAI paddleAI = GetComponent<PaddleAI>();
                if (paddleAI != null)
                {
                    GameUIManager.Instance.ResultText.text = "defeat";
                }
                else
                {
                    GameUIManager.Instance.ResultText.text = "win";
                }
                GameUIManager.Instance.FinishText.SetActive(true);
                StopAllCoroutines();
                PlayerInput playerInput = GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    playerInput.DeactivateInput();
                }
                StartCoroutine(GameOverCo());
            }
        }
        IEnumerator GameOverCo()
        {
            // NetWorkManager를 통해 방을 나가는 요청을 보냅니다.
            if (NetWorkManager.Instance != null && NetWorkManager.Instance.IsConnected)
            {
                NetWorkManager.Instance.LeaveRoom();
            }

            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
