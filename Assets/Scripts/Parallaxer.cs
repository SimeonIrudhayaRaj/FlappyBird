using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    class PoolObject {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform transform) {
            this.transform = transform;
        }
        public void use() {
            inUse = true;
        }
        public void dispose() {
            inUse = false;
        }
    }
    [System.Serializable]
    public struct YSpawnRange {
        public float min, max;
    }


    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate;
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;
    GameManager game;

    void Awake() {
                print("transform: waw");
        Configure();
    }

    void Start() {
        game = GameManager.instance;
    }

    void OnEnable() {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable() {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed() {
        for (int i = 0; i<poolObjects.Length; i++) {
            poolObjects[i].dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }

        if (spawnImmediate) {
            SpawnImmediate();
        }
    }

    void Update() {
        if (game.isGameOver()) {
            return;
        }

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate) {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure() {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i<poolObjects.Length; i++) {
            GameObject go = Instantiate(Prefab) as GameObject;
            go.SetActive(true);
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = new Vector3(10000,100000,0);
            go.transform.position = t.position;
            poolObjects[i] = new PoolObject(t);
        }
        if (spawnImmediate) {
            SpawnImmediate();
        }
    }

    void SpawnImmediate() {
        Transform t = GetPoolObject();
        if (t == null ) {
            return;
        }
        float y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        float x =(immediateSpawnPos.x * Camera.main.aspect)/ targetAspect;
        Spawn();
    }

    void Spawn() {
        Transform t = GetPoolObject();
        if (t == null ) {
            return;
        }
        float x =(defaultSpawnPos.x * Camera.main.aspect)/ targetAspect;
        float y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = new Vector3( x, y, 0);    
    }

    void Shift() {
        for (int i = 0; i<poolObjects.Length; i++) {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }
    void CheckDisposeObject(PoolObject poolObject) {
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect)/ targetAspect) {
            poolObject.dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject() {
        for(int i = 0; i< poolObjects.Length; i++) {
            if(!poolObjects[i].inUse) {
                poolObjects[i].use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}
