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
            // _paddles[0].SetTrigger("PushButton"); // Handled by server response
            SendPaddleInput(-1, 0);
        }
        void OnLeftDown()  //s1
        {
            // _paddles[1].SetTrigger("PushButton"); // Handled by server response
            SendPaddleInput(1, 1);
        }
        void OnRightUp() //d2
        {
            // _paddles[2].SetTrigger("PushButton"); // Handled by server response
            SendPaddleInput(-1, 2);
        }
        void OnRightDown()  //f3
        {
            // _paddles[3].SetTrigger("PushButton"); // Handled by server response
            SendPaddleInput(1, 3);
        }
    }
}
