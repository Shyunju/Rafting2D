using UnityEngine;
using UnityEngine.InputSystem;

namespace Rafting
{
    public class PaddleInput : MonoBehaviour
    {
        Rigidbody2D _rigidbody;
        [SerializeField] Animator[] _paddles;
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }
        void OnLeftUp()  //a0
        {
            _paddles[0].SetTrigger("PushButton");
        }
        void OnLeftDown()  //s1
        {
            _paddles[1].SetTrigger("PushButton");
        }
        void OnRightUp() //d2
        {
            _paddles[2].SetTrigger("PushButton");
        }
        void OnRightDown()  //f3
        {
            _paddles[3].SetTrigger("PushButton");
        }
        void MoveBoat(Vector2 dir)
        {

        }
    }
}
