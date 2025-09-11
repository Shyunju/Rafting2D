using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace Rafting
{
    public class PaddleAI : MoveBoat
    {
        [SerializeField] float _loopTime;
        [SerializeField] float _enemySpeed;
        protected override void Start()
        {
            base.Start();
            _force = _enemySpeed;
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
