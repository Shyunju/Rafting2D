using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

namespace Rafting
{
    /// <summary>
    /// SFS2X 서버에 룸 찾기 및 입장을 요청하는 기능을 담당합니다.
    /// </summary>
    public static class FindAndJoinRoom
    {
        /// <summary>
        /// 서버에 꽉 차지 않은 게임 룸을 찾거나 새로 생성하여 입장을 요청합니다.
        /// </summary>
        public static void SendRequest()
        {
            if (!NetWorkManager.Instance.IsConnected)
            {
                Debug.LogError("FindAndJoinRoom.SendRequest Error: SFS2X is not connected.");
                return;
            }

            Debug.Log("Sending Find and Join Room request to server.");

            // 서버 확장(Extension)에 요청을 보냅니다.
            // 파라미터로 null 대신, 비어있는 SFSObject를 보냅니다.
            ISFSObject parameters = new SFSObject();
            NetWorkManager.Instance.Sfs.Send(new ExtensionRequest(ConstantClass.FINDANDJOIN, parameters));
        }
    }
}
