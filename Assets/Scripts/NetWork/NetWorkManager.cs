using UnityEngine;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;

namespace Rafting
{
    /// <summary>
    /// SFS2X 서버와의 통신을 관리하는 싱글톤 클래스입니다.
    /// </summary>
    public class NetWorkManager : Singleton<NetWorkManager>
    {
        // --- SFS2X Instance ---
        private SmartFox _sfs;
        /// <summary>
        /// SmartFox 인스턴스를 외부에서 안전하게 읽기 전용으로 접근할 수 있도록 합니다.
        /// </summary>
        public SmartFox Sfs => _sfs;
        /// <summary>
        /// 서버에 연결되어 있는지 여부를 반환합니다.
        /// </summary>
        public bool IsConnected => _sfs != null && _sfs.IsConnected;

        [Header("SFS2X Connection Settings")]
        [Tooltip("SFS2X 서버의 IP 주소 또는 도메인 이름")]
        [SerializeField] private string _host = "127.0.0.1";
        [Tooltip("SFS2X 서버의 TCP 포트")]
        [SerializeField] private int _port = 9933;
        [Tooltip("접속할 SFS2X Zone 이름")]
        [SerializeField] private string _zone = "BasicExamples";
        [Tooltip("SFS2X 디버그 메시지를 콘솔에 출력합니다.")]
        [SerializeField] private bool _debug = true;

        protected override void Awake()
        {
            // 부모 클래스의 Awake()가 실행되기 전에 DontDestroy 속성을 설정합니다.
            _isDontDestroy = true;
            base.Awake();
        }

        void Start()
        {
            // SFS2X 클라이언트 인스턴스 생성 (이미 연결된 상태가 아니라면)
            if (_sfs == null)
            {
                _sfs = new SmartFox(_debug);
                AddEventListeners();
            }
        }

        void FixedUpdate()
        {
            // SFS2X는 수신된 메시지를 처리하기 위해 주기적인 호출이 필요합니다.
            if (_sfs != null)
            {
                _sfs.ProcessEvents();
            }
        }
        
        private void AddEventListeners()
        {
            _sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
            _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            _sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
            _sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
            _sfs.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnRoomVariablesUpdate);
            _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            //_sfs.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
            //_sfs.AddEventListener(SFSEvent.UDP_INIT_ERROR, OnUdpInitError);
        }

        public void Connect()
        {
            if (IsConnected)
            {
                Debug.LogWarning("Already connected. Ready to log in.");
                // 이미 연결되어 있다면 바로 로그인 시도
                _sfs.Send(new LoginRequest(""));
                return;
            }

            Debug.Log($"Connecting to SFS2X server at {_host}:{_port}");
            ConfigData cfg = new ConfigData();
            cfg.Host = _host;
            cfg.Port = _port;
            cfg.Zone = _zone;
            _sfs.Connect(cfg);
        }


        // --- SFS2X Event Handlers ---
        private void OnConnection(BaseEvent evt)
        {
            bool success = (bool)evt.Params["success"];
            if (success)
            {
                Debug.Log("Successfully connected to SFS2X. Logging in...");
                _sfs.Send(new LoginRequest(""));
            }
            else
            {
                Debug.LogError("Connection to SFS2X failed: " + (string)evt.Params["errorMessage"]);
            }
        }

        private void OnConnectionLost(BaseEvent evt)
        {
            Debug.LogError("Connection lost. Reason: " + (string)evt.Params["reason"]);
        }

        private void OnLogin(BaseEvent evt)
        {
            User user = (User)evt.Params["user"];
            Debug.Log($"Successfully logged in as {user.Name} (ID: {user.Id}) to zone {_sfs.CurrentZone}");
            // Initialize UDP
            //_sfs.InitUDP();
        }

        private void OnLoginError(BaseEvent evt)
        {
            Debug.LogError("Login failed: " + (string)evt.Params["errorMessage"]);
        }

        private void OnRoomVariablesUpdate(BaseEvent evt)
        {
            // 이벤트 파라미터에 필요한 키들이 모두 존재하는지 안전하게 확인합니다.
            if (!evt.Params.ContainsKey("room") || !evt.Params.ContainsKey("changedVars"))
            {
                Debug.LogWarning("ROOM_VARIABLES_UPDATE event is missing 'room' or 'changedVars' key.");
                return;
            }

            Room room = (Room)evt.Params["room"];

            // API가 변경된 변수의 '이름'을 List<string>으로 보내주는 것을 확인했습니다.
            var changedVarKeys = (System.Collections.Generic.List<string>)evt.Params["changedVars"];

            // 변경된 변수들의 이름 목록을 순회합니다.
            foreach (string key in changedVarKeys)
            {
                // 우리가 관심있는 변수(게임 상태)가 변경되었는지 확인합니다.
                if (key == ConstantClass.ROOM_STATE)
                {
                    // 방 객체에서 키를 이용해 실제 변수 객체와 값을 가져옵니다.
                    var stateVar = room.GetVariable(ConstantClass.ROOM_STATE);
                    if (stateVar != null)
                    {
                        string newState = stateVar.GetStringValue();
                        Debug.Log(string.Format("Room '{0}' state changed to: {1}", room.Name, newState));

                        if (newState == ConstantClass.STATE_PLAYING)
                        {
                            // TODO: 게임 시작에 따른 클라이언트 로직을 여기에 구현합니다.
                            // 예: '게임 시작' 버튼 비활성화, 카운트다운 시작 등
                            LobbyUI.Instance?.RaftingGameScene();
                        }
                    }
                }
            }
        }

        private void OnExtensionResponse(BaseEvent evt)
        {
            string cmd = (string)evt.Params["cmd"];
            ISFSObject data = (ISFSObject)evt.Params["params"];

            switch (cmd)
            {
                case ConstantClass.PADDLE_ANIMATION: // Paddle Animation
                    if (PaddleInput.Instance != null)
                    {
                        Debug.Log("Received paddle animation command from server.");
                        int paddleIndex = data.GetInt("pIdx");
                        int direction = data.GetInt("dir");
                        PaddleInput.Instance.TriggerPaddleAnimation(paddleIndex);
                        PaddleInput.Instance.ProcessInput(direction);
                    }
                    break;

                case ConstantClass.COUNTDOWN_RESPONSE:
                    if (GameUIManager.Instance != null)
                    {
                        GameUIManager.Instance.UpdateCountdown(data);
                    }
                    break;

                case ConstantClass.UPDATE_USER_LIST:
                    if (LobbyUI.Instance != null)
                    {
                        ISFSArray userListArray = data.GetSFSArray("userList");
                        string[] userNames = new string[userListArray.Size()];
                        for (int i = 0; i < userListArray.Size(); i++)
                        {
                            userNames[i] = userListArray.GetUtfString(i);
                        }
                        LobbyUI.Instance.UpdatePlayerSlots(userNames);
                    }
                    break;
            }
        }

        //private void OnUdpInit(BaseEvent evt)
        //{
        //    bool success = (bool)evt.Params["success"];
        //    if (success)
        //    {
        //        Debug.Log("UDP initialization successful!");
        //    }
        //    else
        //    {
        //        Debug.LogError("UDP initialization failed: " + (string)evt.Params["errorMessage"]);
        //    }
        //}

        //private void OnUdpInitError(BaseEvent evt)
        //{
        //    Debug.LogError("UDP initialization error: " + (string)evt.Params["errorMessage"]);
        //}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_sfs != null && _sfs.IsConnected)
            {
                _sfs.Disconnect();
            }
        }
    }
}
