namespace Rafting
{
    public static class ConstantClass
    {
        public const string GAME_TYPE = "10";

        //ROOM VARIABLE
        public const string ROOMNAME = "20";
        public const string ISPUBLIC = "21";
        public const string MAXUSERCNT = "22";
        public const string ROOMREMOVE = "23";
        public const string FINDANDJOIN = "24";

        //ROOM STATE
        public const string ROOM_OWNER_ID = "25";
        public const string ROOM_STATE = "26";
        public const string STATE_WAITING = "27";
        public const string STATE_PLAYING = "28";

        //GAMEPLAY REQUESTS
        public const string START_GAME_REQUEST = "30";
        public const string COUNTDOWN_RESPONSE = "30";


        // Rafting Game
        public const string PADDLE_REQUEST = "40";
        public const string PADDLE_ANIMATION = "41";
        public const string PADDLE_AI = "42";
        public const string BOAT_SYNC = "43";

        // WAITING ROOM
        public const string UPDATE_USER_LIST = "50";

        // MAP GENERATION
        public const string GENERATE_MAP_REQUEST = "60";
        public const string MAP_DATA = "61";

        //HANDLER
        public const string KEEPALIVE = "1000";

        //ERROR
        public const string ERROR = "9990";
        public const string SERVER_MESSAGE = "9991";
        public const string ERROR_CODE = "9998";
        public const string ERROR_MESSAGE = "9999";
    }
}