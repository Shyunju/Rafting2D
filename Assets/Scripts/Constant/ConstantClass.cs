namespace Rafting
{
    /// <summary>
    /// 서버와 클라이언트 간에 공유되는 상수들을 정의하는 클래스입니다.
    /// Java의 ConstantClass를 C#으로 변환한 것입니다.
    /// </summary>
    public static class ConstantClass
    {
        // Character
        public const string X = "19";
        public const string Y = "20";
        public const string Z = "21";

        //ROOM VARIABLE
        public const string ROOMNAME = "40";
        public const string ISPUBLIC = "41";
        public const string MAXUSERCNT = "42";
        public const string ROOMREMOVE = "43";
        public const string FINDANDJOIN = "44";

        //ROOM STATE
        public const string ROOM_OWNER_ID = "49";
        public const string ROOM_STATE = "45";
        public const string STATE_WAITING = "46";
        public const string STATE_PLAYING = "47";

        //GAMEPLAY REQUESTS
        public const string START_GAME_REQUEST = "48";
        public const string PADDLE_REQUEST = "105"; // Paddle input request

        // SERVER RESPONSES / GAME STATE UPDATES
        public const string GAME_STATE_UPDATE = "201"; // Game state update from server
        public const string PADDLE_ANIMATION = "202"; // Paddle animation trigger from server

        //HANDLER
        public const string KEEPALIVE = "1000";

        //ERROR
        public const string ERROR = "9990";
        public const string SERVER_MESSAGE = "9991";
        public const string ERROR_CODE = "9998";
        public const string ERROR_MESSAGE = "9999";
    }
}
