using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rafting
{
    /// <summary>
    /// 로비 씬의 UI 상호작용을 처리합니다.
    /// </summary>
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] GameObject _titlePanel;
        [SerializeField] GameObject _selectPanel;
        [SerializeField] GameObject _waitPanel;

        /// <summary>
        /// Unity 에디터의 Start 버튼 OnClick() 이벤트에 이 함수를 연결해야 합니다.
        /// </summary>
        public void OnStartButtonClick()
        {
            _selectPanel.SetActive(true);
            _titlePanel.SetActive(false);

            Debug.Log("Start button clicked. Attempting to connect to the server...");

            // NetWorkManager는 싱글톤이므로 Instance를 통해 쉽게 접근할 수 있습니다.
            if (NetWorkManager.Instance != null)
            {
                // NetWorkManager의 Connect 메서드를 호출합니다.
                NetWorkManager.Instance.Connect();
            }
            else
            {
                Debug.LogError("NetWorkManager instance not found! NetWorkManager가 씬에 존재하는지 확인해주세요.");
            }
        }

        /// <summary>
        /// 서버에 게임 룸 입장을 요청합니다.
        /// 로그인 성공 후, '게임 찾기' 같은 버튼의 OnClick() 이벤트에 연결할 수 있습니다.
        /// </summary>
        public void OnFindAndJoinRoomClick()
        {
            _selectPanel.SetActive(false);
            _waitPanel.SetActive(true);

            Debug.Log("Find and Join Room button clicked.");

            if (NetWorkManager.Instance != null && NetWorkManager.Instance.IsConnected)
            {
                FindAndJoinRoom.SendRequest();
            }
            else
            {
                Debug.LogError("NetWorkManager is not connected. Please connect to the server first.");
            }
        }

        public void OnGameStartClick()
        {
            var sfs = NetWorkManager.Instance.Sfs;

            // SFS2X에 연결되어 있고, 방에 들어가 있는 상태인지 먼저 확인합니다.
            if (sfs == null || !sfs.IsConnected || sfs.LastJoinedRoom == null)
            {
                Debug.LogError("Cannot start game: Not connected or not in a room.");
                return;
            }

            var mySelf = sfs.MySelf;
            var room = sfs.LastJoinedRoom;

            // 방에 저장된 "ownerId" 룸 변수를 가져와 방장인지 확인합니다.
            var ownerIdVar = room.GetVariable(ConstantClass.ROOM_OWNER_ID);

            // 룸 변수가 존재하고, 그 값이 내 ID와 일치하는지 확인합니다.
            if (ownerIdVar != null && mySelf.Id == ownerIdVar.GetIntValue())
            {
                // 방장인 경우, 게임 시작 요청을 보냅니다.
                Debug.Log("You are the room owner. Sending Start Game request...");
                //StartGameSender.Send();
            }
            else
            {
                // 방장이 아닌 경우, 경고 메시지를 표시합니다.
                Debug.LogWarning("Only the room owner can start the game.");
                // TODO: 여기에 "방장만 게임을 시작할 수 있습니다." 라는 UI 텍스트를 표시하는 로직을 추가할 수 있습니다.
            }

            SceneManager.LoadScene("MainScene");
        }
    }
}
