using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rafting
{
    public class PaddleInput : MoveBoat
    {

        // 동시 입력을 처리하기 위한 변수
        private readonly List<int> _pendingDirections = new List<int>();
        private Coroutine _inputProcessingCoroutine;
        private const float INPUT_COLLECTION_TIME = 0.1f; // 입력을 수집하는 시간 (초)  협력성을 고려하면 더 길게 해야할지도

        // 입력 처리 로직
        private void ProcessInput(int dir)
        {
            _pendingDirections.Add(dir);
            if (_inputProcessingCoroutine == null)
            {
                _inputProcessingCoroutine = StartCoroutine(ProcessPendingInputs());
            }
        }

        IEnumerator ProcessPendingInputs()
        {
            // 짧은 시간 동안 추가 입력을 기다림
            yield return new WaitForSeconds(INPUT_COLLECTION_TIME);

            int totalDir = 0;
            foreach (int dir in _pendingDirections)
            {
                totalDir += dir;
            }

            int inputCount = _pendingDirections.Count;

            _pendingDirections.Clear();
            _inputProcessingCoroutine = null;

            if (inputCount > 0)
            {
                // 수집된 입력을 기반으로 이동 및 회전 실행
                StartCoroutine(MoveAndRotateCo(totalDir, inputCount));
            }
        }

        void OnLeftUp()  //a0
        {
            _paddles[0].SetTrigger("PushButton");
            ProcessInput(-1);
        }
        void OnLeftDown()  //s1
        {
            _paddles[1].SetTrigger("PushButton");
            ProcessInput(1);
        }
        void OnRightUp() //d2
        {
            _paddles[2].SetTrigger("PushButton");
            ProcessInput(-1);
        }
        void OnRightDown()  //f3
        {
            _paddles[3].SetTrigger("PushButton");
            ProcessInput(1);
        }
    }
}