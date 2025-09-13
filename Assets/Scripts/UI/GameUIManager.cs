using Sfs2X.Entities.Data;
using System.Collections;
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

        public GameObject FinishText { get { return _finishText; } set { _finishText = value; } }
        public TMP_Text ResultText { get { return _resultText; } set { _resultText = value; } }

        protected override void Awake()
        {
            base.Awake();
            if (_countdownText != null) _countdownText.gameObject.SetActive(false);
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
    }
}
