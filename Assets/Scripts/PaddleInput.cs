using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rafting
{
    public class PaddleInput : MoveBoat
    {
        static PaddleInput _instance;

        public static PaddleInput Instance { get { return _instance; } }

        void Awake()
        {
            _instance = this;
            this.boatId = "playerBoat"; // Set the boatId for this player boat
        }

        protected override void Start()
        {
            base.Start();
            // 게임이 시작되면 입력 비활성화 상태로 시작합니다.
            SetInputEnabled(false);
        }

        private bool _isInputEnabled = false;

        /// <summary>
        /// 내부 플래그를 사용하여 입력 처리를 제어합니다.
        /// </summary>
        public void SetInputEnabled(bool isEnabled)
        {
            _isInputEnabled = isEnabled;
            if (isEnabled)
                Debug.Log("PaddleInput has been enabled.");
            else
                Debug.Log("PaddleInput has been disabled.");
        }

        // 입력 처리 로직: 서버로 노 젓기 데이터를 전송합니다.
        private void SendPaddleInput(int dir, int pidx)
        {
            // NetWorkManager를 통해 SmartFox 인스턴스를 가져옵니다.
            var sfs = NetWorkManager.Instance.Sfs;
            if (sfs != null && sfs.LastJoinedRoom != null)
            {
                // 서버로 보낼 데이터를 SFSObject에 담습니다.
                ISFSObject data = new SFSObject();
                data.PutInt("dir", dir);
                data.PutInt("pidx", pidx);

                // ExtensionRequest를 생성하여 서버의 "paddle" 핸들러로 데이터를 UDP로 전송합니다.
                sfs.Send(new ExtensionRequest(ConstantClass.PADDLE_REQUEST, data, sfs.LastJoinedRoom, false));
            }
            else
            {
                Debug.LogWarning("Cannot send paddle input: Not connected or not in a room.");
            }
        }

        void OnLeftUp()  //a0
        {
            if (!_isInputEnabled) return;
            SendPaddleInput(-1, 0);
        }
        void OnLeftDown()  //s1
        {
            if (!_isInputEnabled) return;
            SendPaddleInput(1, 1);
        }
        void OnRightUp() //d2
        {
            if (!_isInputEnabled) return;
            SendPaddleInput(-1, 2);
        }
        void OnRightDown()  //f3
        {
            if (!_isInputEnabled) return;
            SendPaddleInput(1, 3);
        }
    }
}
