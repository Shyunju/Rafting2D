using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Rafting
{
    public class MapGenerator : Singleton<MapGenerator>
    {
        [Header("Map Settings")]
        [SerializeField] private List<GameObject> _rockPrefabs; // 0, 1, 2 타입의 바위 프리팹 리스트
        [SerializeField] private Transform _rockParent; // 생성된 바위들을 담을 부모 트랜스폼

        void Start()
        {
            // NetWorkManager가 맵 데이터를 가지고 있는지 확인하고, 있다면 맵 생성을 시작합니다.
            var mapData = NetWorkManager.Instance?.MapData;
            if (mapData != null)
            {
                GenerateMap(mapData);
            }
            else
            {
                Debug.LogWarning("Map data not found in NetWorkManager. Map will not be generated.");
            }
        }

        /// <summary>
        /// 서버에서 받은 맵 데이터를 기반으로 바위를 생성합니다.
        /// </summary>
        /// <param name="mapData">서버의 RoomVariable에 저장된 맵 데이터</param>
        public void GenerateMap(ISFSArray mapData)
        {
            if (_rockPrefabs == null || _rockPrefabs.Count == 0)
            {
                Debug.LogError("Rock prefabs are not assigned in MapGenerator.");
                return;
            }

            // 기존에 생성된 바위가 있다면 모두 삭제합니다.
            foreach (Transform child in _rockParent)
            {
                Destroy(child.gameObject);
            }

            // SFSArray를 순회하며 각 바위 데이터를 처리합니다.
            for (int i = 0; i < mapData.Size(); i++)
            {
                ISFSObject rockData = mapData.GetSFSObject(i);

                int type = rockData.GetInt("type");
                float x = rockData.GetFloat("x");
                float y = rockData.GetFloat("y");

                if (type < 0 || type >= _rockPrefabs.Count)
                {
                    Debug.LogError($"Invalid rock type received from server: {type}");
                    continue;
                }

                // 해당 타입의 프리팹을 지정된 위치에 생성합니다.
                GameObject rockPrefab = _rockPrefabs[type];
                Vector3 position = new Vector3(x, y, 0);
                Instantiate(rockPrefab, position, Quaternion.identity, _rockParent);
            }

            Debug.Log($"Map generated successfully with {mapData.Size()} rocks.");
        }
    }
}
