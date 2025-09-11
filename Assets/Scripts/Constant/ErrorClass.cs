using System.Collections.Generic;

namespace Rafting
{
    /// <summary>
    /// 서버와 클라이언트 간에 공유되는 에러 코드 및 메시지를 정의하는 클래스입니다.
    /// Java의 ErrorClass를 C#으로 변환한 것입니다.
    /// </summary>
    public static class ErrorClass
    {
        public const string ALLOK = "E000";
        public const string GAME_SERVER_FULL = "E700";
        public const string ROOM_NOT_AVAILABLE = "E701";
        public const string ROOM_ALREADY_FULL = "E702";
        public const string ROOM_ALREADY_EXIST = "E703";
        public const string DATA_EMPTY = "E797";
        public const string DATA_MISMATCH = "E798";
        public const string SERVER_UNKNOWN_ERROR = "E799";

        /// <summary>
        /// 에러 코드에 해당하는 다국어 메시지를 반환합니다.
        /// </summary>
        /// <param name="errorCode">에러 코드</param>
        /// <returns>0: 영어 메시지, 1: 한국어 메시지</returns>
        public static Dictionary<string, string> GetErrorMessage(string errorCode)
        {
            var messages = new Dictionary<string, string>();

            switch (errorCode)
            {
                case ALLOK:
                    messages["en"] = "OK";
                    messages["ko"] = "-";
                    break;
                case GAME_SERVER_FULL:
                    messages["en"] = "Unknown Error has been occured.";
                    messages["ko"] = "-";
                    break;
                case ROOM_NOT_AVAILABLE:
                    messages["en"] = "The room is not available. Please try to enter another room.";
                    messages["ko"] = "-";
                    break;
                case ROOM_ALREADY_FULL:
                    messages["en"] = "The room is already full. Please try to enter another room.";
                    messages["ko"] = "-";
                    break;
                case ROOM_ALREADY_EXIST:
                    messages["en"] = "The room already exist.";
                    messages["ko"] = "-";
                    break;
                case DATA_EMPTY:
                    messages["en"] = "Data search result is empty.";
                    messages["ko"] = "-";
                    break;
                case DATA_MISMATCH:
                    messages["en"] = "Some wrong data have been found. You need to restart the game.";
                    messages["ko"] = "-";
                    break;
                case SERVER_UNKNOWN_ERROR:
                    messages["en"] = "Unknown Error has been occured.";
                    messages["ko"] = "-";
                    break;
                default:
                    messages["en"] = "-";
                    messages["ko"] = "-";
                    break;
            }
            return messages;
        }
    }
}
