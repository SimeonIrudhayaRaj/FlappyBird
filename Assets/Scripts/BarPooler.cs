using System.Collections;
using System;
using System.Collections.Generic;
// using System.Collections.Generic.IEnumerable<Int>;
using UnityEngine;
using System.Linq;

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
    int[,] barColorNums; 
    void Awake() {
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
        Time.timeScale = (float) Time.timeScale * 1.00001f;
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
            poolObjects[i] = new PoolObject(go);
        }
        barColorNums = new int[200,5];
        // var list = new List<int> { 0, 1, 2, 3, 4 };
        // var result = GetPermutations(list, 4);
        // int k = 0;
        // foreach (var perm in result) {
        //     int j = 0;
        //     foreach (var c in perm) {
        //         barColorNums[k,j] = c;
        //         Debug.Log(c);
        //         j++;
        //     }
        //     k++;
        // }
        var num = 5;
 
        var numberOfPerm = 0;
        var elements = Enumerable.Range(0, num).ToArray();
        var workArr = Enumerable.Range(0, elements.Length + 1).ToArray();
        int m = 0;
        var index = 1;
        while (index < elements.Length)
        {
            workArr[index]--;
            var j = 0;
            if (index % 2 == 1)
            {
                j = workArr[index];
            }
 
            SwapInts(ref elements[j], ref elements[index]);
            index = 1;
            while (workArr[index] == 0)
            {
                workArr[index] = index;
                index++;
            }
 
            int k = 0;
            foreach (var e in elements)
            {
                barColorNums[numberOfPerm,k] = e;
                k++;
            }
            numberOfPerm++;
            m++;
        }
 
    }
    void Spawn() {
        float x =(defaultSpawnPos.x * Camera.main.aspect)/ targetAspect;
        float y = -4f;
        float height = 2.7f;
        int rand = new System.Random().Next(0, 24);
        int rand1 = new System.Random().Next(0, 3);

        BallController.nextColor = barColorNums[rand,rand1];
        for (int i = 0; i < 4; i++) {
            Color barColor;
            PoolObject bar = GetPoolObject();
            var barenderer = bar.gameObject.GetComponent<Renderer>();
            string tag;
                switch (barColorNums[rand,i])
                {
                    case 0:
                        barColor = Color.red;
                        tag = "redBar";
                        break;

                    case 1:
                        barColor = Color.green;
                        tag = "greenBar";
                        break;

                    case 2:
                        barColor = Color.yellow;
                        tag = "yellowBar";
                        break;

                    case 3:
                        barColor = Color.magenta;
                        tag = "magentaBar";
                        break;

                    default:
                        barColor = Color.red;
                        tag = "redBar";
                        break;
                }
                
            barenderer.material.SetColor("_Color", barColor);
            bar.gameObject.tag = tag;
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

    // IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count) {
    //     int i = 0;
    //     foreach(var item in items)
    //     {
    //         if(count == 1)
    //             yield return new T[] { item };
    //         else
    //         {
    //             foreach(var result in GetPermutations(items.Skip(i + 1), count - 1))
    //                 yield return new T[] { item }.Concat(result);
    //         }

    //         ++i;
    //     }
    // }
    private void SwapInts(ref int a, ref int b)
    {
        a ^= b;
        b ^= a;
        a ^= b;
    }

    private static void PrintPerm(int[] elements)
    {
        Console.WriteLine(string.Join(", ", elements));
    }
}
