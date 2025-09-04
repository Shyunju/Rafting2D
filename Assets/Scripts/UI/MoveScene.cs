using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rafting
{
    public class MoveScene : MonoBehaviour
    {
        /// <summary>
        /// "MainScene"으로 씬을 이동시킵니다.
        /// Unity 에디터의 UI Button의 OnClick() 이벤트에 연결할 수 있습니다.
        /// </summary>
        public void GoToMainScene()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}