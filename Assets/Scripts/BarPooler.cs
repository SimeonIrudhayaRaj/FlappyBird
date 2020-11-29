using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarPooler : MonoBehaviour
{
    class PoolObject {
        public GameObject gameObject;
        public bool inUse;
        public PoolObject(GameObject gameObject) {
            this.gameObject = gameObject;
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
            poolObjects[i].gameObject.transform.position = Vector3.one * 1000;
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
            go.transform.position = new Vector3(10000,100000,0);
            poolObjects[i] = new PoolObject(go);
        }
    }
    void Spawn() {
        float x =(defaultSpawnPos.x * Camera.main.aspect)/ targetAspect;
        float y = -4f;
        float height = 2.7f;
        for (int i = 0; i < 4; i++) {
            PoolObject bar = GetPoolObject();
            Debug.Log(height);
            Debug.Log(x);
            var barenderer = bar.gameObject.GetComponent<Renderer>();
            Random rnd = new Random();
            float f = Random.Range(0f, 3f);
            Debug.Log((int)f);
            Debug.Log(f);
            switch ((int)f)
            {
                case 0:
                    barenderer.material.SetColor("_Color", Color.red);
                    break;
                case 1:
                    barenderer.material.SetColor("_Color", Color.green);
                    break;
                case 2:
                    barenderer.material.SetColor("_Color", Color.yellow);
                    break;
                case 3:
                    barenderer.material.SetColor("_Color", Color.magenta);
                    break;
                default:
                    barenderer.material.SetColor("_Color", Color.red);
                    break;
            }
            bar.gameObject.transform.position = new Vector3( x, y + (height * i) , 0);    
            // bar.gameObject.transform.position = new Vector3( x, y + (height * i), 0);    
            
        }
      
    }

    void Shift() {
        for (int i = 0; i<poolObjects.Length; i++) {
            poolObjects[i].gameObject.transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }
    void CheckDisposeObject(PoolObject poolObject) {
        if (poolObject.gameObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect)/ targetAspect) {
            poolObject.dispose();
            poolObject.gameObject.transform.position = Vector3.one * 1000;
        }
    }

    PoolObject GetPoolObject() {
        for(int i = 0; i< poolObjects.Length; i++) {
            if(!poolObjects[i].inUse) {
                poolObjects[i].use();
                return poolObjects[i];
            }
        }
        return null;
    }
}
