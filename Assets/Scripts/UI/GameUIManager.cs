using TMPro;
using UnityEngine;

namespace Rafting
{
    public class GameUIManager : Singleton<GameUIManager>
    {
        [SerializeField] GameObject _finishText;
        [SerializeField] TMP_Text _resultText;

        public GameObject FinishText {get { return _finishText; } set { _finishText = value; } }
        public TMP_Text ResultText { get { return _resultText; } set { _resultText = value; } }
    }
}
