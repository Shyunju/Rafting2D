using Sfs2X.Entities.Data;
using Sfs2X.Requests;
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
        private bool _isCollidingWithRock = false; // 바위와 충돌 중인지 여부
        public string boatId; // Unique ID for this boat instance

        protected virtual void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _serverPosition = _rigidbody.position;
            _serverRotation = _rigidbody.rotation;
        }

        protected virtual void OnEnable()
        {
            if (NetWorkManager.Instance != null && !string.IsNullOrEmpty(boatId))
            {
                NetWorkManager.Instance.RegisterBoat(boatId, this);
            }
        }

        protected virtual void OnDisable()
        {
            if (NetWorkManager.Instance != null && !string.IsNullOrEmpty(boatId))
            {
                NetWorkManager.Instance.UnregisterBoat(boatId);
            }
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

        private void SendCollisionReport(float normalX, float normalY)
        {
            var sfs = NetWorkManager.Instance.Sfs;
            if (sfs != null && sfs.LastJoinedRoom != null)
            {
                ISFSObject data = new SFSObject();
                data.PutFloat("x", transform.position.x);
                data.PutFloat("y", transform.position.y);
                data.PutFloat("rot", _rigidbody.rotation);
                data.PutFloat("normalX", normalX);
                data.PutFloat("normalY", normalY);
                data.PutUtfString("boatId", this.boatId); // Add boatId
                
                sfs.Send(new ExtensionRequest(ConstantClass.COLLISION_REPORT, data, sfs.LastJoinedRoom, false)); 
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Rock"))
            {
                if (_isCollidingWithRock) return; // 이미 바위 충돌 처리 중이면 중복 실행 방지

                _isCollidingWithRock = true;
                // Extract collision normal
                Vector2 normal = collision.contacts[0].normal;
                StartCoroutine(HandleRockCollision(normal.x, normal.y)); // Pass normal to coroutine
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Rock"))
            {
                // 코루틴이 끝나기 전에 바위에서 떨어지는 경우를 대비하여 플래그 리셋
                _isCollidingWithRock = false;
            }
        }

        IEnumerator HandleRockCollision(float normalX, float normalY)
        {
            // Switch to Kinematic to prevent jitter and sliding
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            SendCollisionReport(normalX, normalY);

            // Wait for a short duration, slightly longer than the server's input block
            yield return new WaitForSeconds(0.35f);

            // Revert to Dynamic to allow player control again
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            // _isCollidingWithRock 플래그는 OnCollisionExit2D에서만 리셋됩니다.
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
