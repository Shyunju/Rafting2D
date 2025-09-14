using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;

namespace Rafting
{
    public class PaddleAI : MoveBoat
    {
        static PaddleAI _instance;

        public static PaddleAI Instance { get { return _instance; } }

        void Awake()
        {
            _instance = this;
        }
    }
}
