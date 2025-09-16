using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using Sfs2X.Core;

namespace Rafting
{
    public class PaddleAI : MoveBoat
    {
        static PaddleAI _instance;

        public static PaddleAI Instance { get { return _instance; } }

        void Awake()
        {
            _instance = this;
            this.boatId = "aiBoat"; // Set the boatId for this AI boat
        }

        protected override void OnEnable()
        {
            base.OnEnable(); // Call base class OnEnable for boat registration
            AddSmartFoxListeners();
        }

        protected override void OnDisable()
        {
            base.OnDisable(); // Call base class OnDisable for boat unregistration
            RemoveSmartFoxListeners();
        }

        private void AddSmartFoxListeners()
        {
            if (NetWorkManager.Instance != null && NetWorkManager.Instance.Sfs != null)
            {
                NetWorkManager.Instance.Sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            }
        }

        private void RemoveSmartFoxListeners()
        {
            if (NetWorkManager.Instance != null && NetWorkManager.Instance.Sfs != null)
            {
                NetWorkManager.Instance.Sfs.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            }
        }

        private void OnExtensionResponse(BaseEvent evt)
        {
            string cmd = (string)evt.Params["cmd"];
            ISFSObject data = (ISFSObject)evt.Params["params"];

            if (cmd == ConstantClass.PADDLE_AI)
            {
                int pIdx = data.GetInt("pIdx");
                TriggerPaddleAnimation(pIdx);
            }
        }
    }
}
