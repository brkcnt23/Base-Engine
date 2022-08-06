using System;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Pool;

#if UNITY_EDITOR

#endif

namespace Base {
    public abstract class B_PoolerBase : MonoBehaviour {
        [HideLabel]
        public List<ObjectsToPool> PoolsList;

        //Distance from spawn içerisine bir sayı girilmesi lazım
        private float distanceFromSpawn, spawnOffset;

        private Vector3 firstSpawnPoint = new Vector3(8000, 7000, 9000);
        public Dictionary<string, ObjectPool<GameObject>> PoolsDictionary;

        public void InitiatePooller() {
            PoolsDictionary = new Dictionary<string, ObjectPool<GameObject>>();
            foreach (var Pools in PoolsList) {
                Pools.PoolName = Pools.ObjectPrefab.name;
                var objPool = new ObjectPool<GameObject>(() => Instantiate(Pools.ObjectPrefab, Vector3.zero, Quaternion.identity), obj => {
                    obj.SetActive(true);
                    obj.transform.position = Vector3.zero;
                    obj.transform.rotation = Quaternion.identity;
                }, obj => {
                    obj.SetActive(false);
                    obj.transform.position = Vector3.zero;
                    obj.transform.rotation = Quaternion.identity;
                }, Destroy,
                    true,
                    Pools.DesiredCount,
                    Pools.MaxCount);
                
                PoolsDictionary.Add(Pools.PoolName, objPool);
            }
        }

        public virtual void InitiatePooller(Transform parent) {
            PoolsDictionary = new Dictionary<string, ObjectPool<GameObject>>();
            foreach (var Pools in PoolsList) {
                Pools.PoolName = Pools.ObjectPrefab.name;
                var objPool = new ObjectPool<GameObject>(() => Instantiate(Pools.ObjectPrefab, Vector3.zero, Quaternion.identity, parent), obj => {
                        obj.SetActive(true);
                        obj.transform.position = Vector3.zero;
                        obj.transform.rotation = Quaternion.identity;
                    }, obj => {
                        obj.SetActive(false);
                        obj.transform.position = Vector3.zero;
                        obj.transform.rotation = Quaternion.identity;
                    }, Destroy,
                    true,
                    Pools.DesiredCount,
                    Pools.MaxCount);
                
                PoolsDictionary.Add(Pools.PoolName, objPool);
            }
        }

        public void AddPool(GameObject objPrefab, string prefabName, int desiredCount, int maxCount) {
            if (PoolsDictionary.ContainsKey(objPrefab.name)) return;
            var newPool = new ObjectsToPool();
            newPool.PoolName = objPrefab.name;
            newPool.ObjectPrefab = objPrefab;
            newPool.DesiredCount = desiredCount;
            PoolsList.Add(newPool);
            
            newPool.PoolName = newPool.ObjectPrefab.name;
            var objPool = new ObjectPool<GameObject>(() => Instantiate(newPool.ObjectPrefab, Vector3.zero, Quaternion.identity), obj => {
                    obj.SetActive(true);
                    obj.transform.position = Vector3.zero;
                    obj.transform.rotation = Quaternion.identity;
                }, obj => {
                    obj.SetActive(false);
                    obj.transform.position = Vector3.zero;
                    obj.transform.rotation = Quaternion.identity;
                }, Destroy,
                true,
                newPool.DesiredCount,
                newPool.MaxCount);

            PoolsDictionary.Add(newPool.PoolName, objPool);
        }
#if UNITY_EDITOR

        protected void AddPoolInEditor(GameObject objPrefab, string prefabName, int desiredCount, int maxCount) {
            if (PoolsList == null) PoolsList = new List<ObjectsToPool>();
            var newPool = new ObjectsToPool();
            newPool.PoolName = objPrefab.name;
            newPool.ObjectPrefab = objPrefab;
            newPool.DesiredCount = desiredCount;
            newPool.MaxCount = maxCount;
            PoolsList.Add(newPool);
        }

#endif

        public GameObject SpawnObjFromPool(string objectPoolName, Vector3 spawnPosition) {
            if (!PoolsDictionary.ContainsKey(objectPoolName)) return null;

            var objectToSpawn = PoolsDictionary[objectPoolName].Get();
            
            return objectToSpawn;
        }

        public GameObject SpawnObjFromPool(string objectPoolName, Vector3 spawnPosition, Quaternion spawnRotation) {
            if (!PoolsDictionary.ContainsKey(objectPoolName)) return null;

            var objectToSpawn = PoolsDictionary[objectPoolName].Get();;
            objectToSpawn.transform.position = spawnPosition;
            objectToSpawn.transform.rotation = spawnRotation;
            return objectToSpawn;
        }
        
        public GameObject SpawnObjFromPool(string objectPoolName, Vector3 spawnPosition, Quaternion spawnRotation, Transform spawnParent) {
            if (!PoolsDictionary.ContainsKey(objectPoolName)) return null;

            var objectToSpawn = PoolsDictionary[objectPoolName].Get();
            objectToSpawn.transform.position = spawnPosition;
            objectToSpawn.transform.rotation = spawnRotation;
            objectToSpawn.transform.SetParent(spawnParent);
            return objectToSpawn;
        }

        public ObjectsToPool GetObjectPool(string poolName) {
            return PoolsList.Find(t => t.PoolName == poolName);
        }

        public ObjectsToPool GetObjectPool(object poolName) {
            return PoolsList.Find(t => t.PoolName == poolName.ToString());
        }
        
        [Serializable]
        public class ObjectsToPool {
            public string PoolName;
            [HideInInspector] public ObjectPool<GameObject> ObjectPool;
#if UNITY_EDITOR

            [AssetsOnly]
#endif
            public GameObject ObjectPrefab;

            public int DesiredCount;
            public int MaxCount;
        }
    }
}