using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

namespace Rafting
{
    /// <summary>
    /// 서버에 게임 시작을 요청하는 기능을 담당하는 정적 클래스입니다.
    /// </summary>
    public static class StartGameSender
    {
        /// <summary>
        /// 서버에 게임 시작을 요청합니다.
        /// 이 요청은 현재 유저가 있는 방의 상태를 'playing'으로 변경합니다.
        /// </summary>
        public static void Send()
        {
            var sfs = NetWorkManager.Instance.Sfs;

            if (sfs == null || !sfs.IsConnected)
            {
                Debug.LogError("StartGameSender.Send Error: SFS2X is not connected.");
                return;
            }

            if (sfs.LastJoinedRoom == null)
            {
                Debug.LogError("StartGameSender.Send Error: User is not in a room.");
                return;
            }

            Debug.Log("Sending Start Game request to the current room.");

            // 1. 커맨드: 서버에 등록된 게임 시작 요청 핸들러 (ConstantClass.START_GAME_REQUEST)
            // 2. 파라미터: StartGameRequestHandler는 별도 파라미터를 요구하지 않으므로 빈 SFSObject를 보냅니다.
            // 3. 타겟 룸: 현재 유저가 접속해 있는 방으로 요청을 보냅니다.
            // 3. 타겟 룸: null로 보내거나 생략하면 Zone으로 요청이 보내집니다.
            sfs.Send(new ExtensionRequest(ConstantClass.START_GAME_REQUEST, new SFSObject()));
        }
    }
}
