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
        }
        void OnLeftDown()  //s1
        {
        }
        void OnRightUp() //d2
        {
        }
        void OnRightDown()  //f3
        {
        }
    }
}
