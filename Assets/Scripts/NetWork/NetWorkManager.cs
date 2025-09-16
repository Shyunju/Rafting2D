using UnityEngine;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // Added for Dictionary

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

        // --- Game Data Properties ---
        public ISFSArray MapData { get; private set; }

        // --- Boat Management ---
        private Dictionary<string, MoveBoat> _boats = new Dictionary<string, MoveBoat>();

        public void RegisterBoat(string boatId, MoveBoat boat)
        {
            if (!_boats.ContainsKey(boatId))
            {
                _boats.Add(boatId, boat);
                Debug.Log($"Registered boat: {boatId}");
            }
            else
            {
                Debug.LogWarning($"Boat with ID {boatId} already registered.");
            }
        }

        public void UnregisterBoat(string boatId)
        {
            if (_boats.ContainsKey(boatId))
            {
                _boats.Remove(boatId);
                Debug.Log($"Unregistered boat: {boatId}");
            }
        }

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
            _sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
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

        public void LeaveRoom()
        {
            if (IsConnected && _sfs.LastJoinedRoom != null)
            {
                _sfs.Send(new LeaveRoomRequest());
            }
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

            // 이벤트 리스너를 모두 제거합니다.
            _sfs.RemoveAllEventListeners();

            // 로비 씬으로 돌아갑니다.
            SceneManager.LoadScene("LobbyScene");
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
                    var stateVar = room.GetVariable(ConstantClass.ROOM_STATE);
                    if (stateVar != null)
                    {
                        string newState = stateVar.GetStringValue();
                        Debug.Log(string.Format("Room '{0}' state changed to: {1}", room.Name, newState));

                        if (newState == ConstantClass.STATE_PLAYING)
                        {
                            LobbyUI.Instance?.RaftingGameScene();
                        }
                    }
                }
                else if (key == ConstantClass.MAP_DATA)
                {
                    var mapDataVar = room.GetVariable(ConstantClass.MAP_DATA);
                    if (mapDataVar != null)
                    {
                        MapData = mapDataVar.GetSFSArrayValue();
                        Debug.Log("Map data received and stored in NetWorkManager.");
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
                        //Debug.Log("Received paddle animation command from server.");
                        int paddleIndex = data.GetInt("pIdx");
                        PaddleInput.Instance.TriggerPaddleAnimation(paddleIndex);
                    }
                    break;

                case ConstantClass.BOAT_SYNC:
                    string boatId = data.GetUtfString("boatId"); // Get the boat ID
                    if (_boats.TryGetValue(boatId, out MoveBoat boatToUpdate))
                    {
                        float x = data.GetFloat("x");
                        float y = data.GetFloat("y");
                        float rot = data.GetFloat("rot");
                        boatToUpdate.UpdateStateFromServer(x, y, rot);
                    }
                    else
                    {
                        Debug.LogWarning($"Received BOAT_SYNC for unknown boatId: {boatId}");
                    }
                    break;

                case ConstantClass.PADDLE_AI: // AI Paddle
                    if (PaddleAI.Instance != null)
                    {
                        //Debug.Log("Received AI paddle command from server.");
                        int paddleIndex = data.GetInt("pIdx");
                        int direction = data.GetInt("dir");
                        PaddleAI.Instance.TriggerPaddleAnimation(paddleIndex);
                        //PaddleAI.Instance.ProcessInput(direction);
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

        private void OnUserExitRoom(BaseEvent evt)
        {
            Room room = (Room)evt.Params["room"];
            User user = (User)evt.Params["user"];
            Debug.Log($"User {user.Name} has left the room {room.Name}");

            // 내가 나간 경우, LobbyUI에서 이미 UI 전환을 처리했으므로 아무것도 하지 않습니다.
            if (user.IsItMe)
            {
                return;
            }

            // 다른 사람이 나갔을 경우, 현재 씬에 따라 적절한 UI 매니저를 업데이트합니다.
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "LobbyScene")
            {
                if (LobbyUI.Instance != null)
                {
                    var userList = room.UserList;
                    string[] userNames = new string[userList.Count];
                    for (int i = 0; i < userList.Count; i++)
                    {
                        userNames[i] = userList[i].Name;
                    }
                    LobbyUI.Instance.UpdatePlayerSlots(userNames);
                }
            }
            else if (currentScene == "MainScene")
            {
                if (GameUIManager.Instance != null)
                {
                    GameUIManager.Instance.UpdatePlayerList(room.UserList);
                }
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
