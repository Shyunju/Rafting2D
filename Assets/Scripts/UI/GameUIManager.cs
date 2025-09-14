using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Rafting
{
    public class GameUIManager : Singleton<GameUIManager>
    {
        [SerializeField] GameObject _finishText;
        [SerializeField] TMP_Text _resultText;
        [Header("Countdown Settings")]
        [SerializeField] private TMP_Text _countdownText; // 카운트다운을 표시할 TextMeshPro
        [SerializeField] private float _startMessageDuration = 0.5f; // "START!" 메시지 표시 시간

        [Header("Player List Settings")]
        [SerializeField] private List<GameObject> _playerSlots; // 플레이어 슬롯 GameObject 리스트

        public GameObject FinishText { get { return _finishText; } set { _finishText = value; } }
        public TMP_Text ResultText { get { return _resultText; } set { _resultText = value; } }

        protected override void Awake()
        {
            base.Awake();
            if (_countdownText != null) _countdownText.gameObject.SetActive(false);
        }

        private void Start()
        {
            // 씬 시작 시 현재 룸의 유저 리스트를 가져와 UI를 업데이트합니다.
            var sfs = NetWorkManager.Instance?.Sfs;
            if (sfs != null && sfs.LastJoinedRoom != null)
            {
                UpdatePlayerList(sfs.LastJoinedRoom.UserList);
            }
        }

        /// <summary>
        /// 서버로부터 받은 카운트다운 데이터를 처리합니다.
        /// </summary>
        public void UpdateCountdown(ISFSObject data)
        {
            if (_countdownText == null) return;

            if (data.ContainsKey("count"))
            {
                int count = data.GetInt("count");
                _countdownText.text = count.ToString();
                if (!_countdownText.gameObject.activeSelf) _countdownText.gameObject.SetActive(true);
            }
            else if (data.ContainsKey("text"))
            {
                string text = data.GetUtfString("text"); // "START!"
                _countdownText.text = text;

                // 카운트다운이 끝나면 PaddleInput을 활성화합니다.
                if (PaddleInput.Instance != null)
                {
                    PaddleInput.Instance.SetInputEnabled(true);
                }

                // 일정 시간 후 카운트다운 텍스트를 비활성화합니다.
                StartCoroutine(HideCountdownTextCo(_startMessageDuration));
            }
        }

        private IEnumerator HideCountdownTextCo(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_countdownText != null) _countdownText.gameObject.SetActive(false);
        }

        /// <summary>
        /// 유저 리스트를 UI에 업데이트합니다.
        /// </summary>
        /// <param name="users">표시할 유저 리스트</param>
        public void UpdatePlayerList(List<User> users)
        {
            if (_playerSlots == null) return;

            for (int i = 0; i < _playerSlots.Count; i++)
            {
                if (i < users.Count)
                {
                    _playerSlots[i].SetActive(true);
                    TextMeshProUGUI nameText = _playerSlots[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (nameText != null)
                    {
                        nameText.text = users[i].Name;
                    }
                }
                else
                {
                    // 남는 UI 슬롯은 비활성화합니다.
                    _playerSlots[i].SetActive(false);
                }
            }
        }
    }
}
