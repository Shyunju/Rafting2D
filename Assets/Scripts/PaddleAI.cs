using UnityEngine;
using System.Collections;

namespace Rafting
{
    public class PaddleAI : MoveBoat
    {
        void Start()
        {
            base.Start();
            StartCoroutine(SetDirectionCo());
        }

        IEnumerator SetDirectionCo()
        {
            _paddleIdx = Random.Range(0, 4);
            _paddles[_paddleIdx].SetTrigger("PushButton");
            _dir = _paddleIdx % 2 == 0 ? -1 : 1;
            StartCoroutine(MoveAndRotateCo(_dir, 1));
            yield return new WaitForSeconds(_loopTime);
            StartCoroutine(SetDirectionCo());
        }
    }
}
