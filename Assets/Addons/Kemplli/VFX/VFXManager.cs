using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Android;

public class VFXManager : MonoBehaviour
{
    public List<VFX> all = new List<VFX>();

    public static bool initialized;

    private Dictionary<string, float> limitedFrequencyVFX = new Dictionary<string, float>();
    private Dictionary<string,CustomPool<VFX>> pools = new Dictionary<string, CustomPool<VFX>>();

    private void Awake()
    {
        G.vfx = this;

        initialized = false;

        DontDestroyOnLoad(gameObject);

        if (!initialized)
        {
            Init();
        }
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    PlayOneTimeVFX("Explosion", GameMath.MousePos2D());
        //}
    }

    public GameObject PlayOneTimeVFX(string id, Vector3 position, Color color, Color secondColor = default, bool selfDestroy = true, float rotation = 0f, int burstCount = 0, float speed = 0,
        Gradient gradient = null,Vector2 scale = new Vector2())
    {
        var vfxObj = pools[id].Get(); vfxObj.transform.position = position; if(rotation == 0f) vfxObj.transform.rotation = Quaternion.identity; else vfxObj.transform.rotation = Quaternion.Euler(0,0,rotation); if (scale != Vector2.zero) vfxObj.transform.localScale = scale;        vfxObj.gameObject.name = id;
        scale = vfxObj.transform.localScale;
        var ps = vfxObj.GetComponent<ParticleSystem>();
        if (color != Color.white)
        {
            ps.startColor = color;
            if (secondColor != default)
            {
                var main = ps.main;
                ParticleSystem.MinMaxGradient randomColor = new ParticleSystem.MinMaxGradient(color, secondColor);
                main.startColor = randomColor;
            }
        }
        if (gradient != null) { var main = ps.main; main.startColor = new ParticleSystem.MinMaxGradient(gradient); }
        if (burstCount != 0) { var emission = ps.emission; var burst = emission.GetBurst(0); burst.count = burstCount; emission.SetBurst(0, burst); }
        if(speed != 0) ps.startSpeed = speed;
        vfxObj.OnSpawn?.Invoke();

        if (selfDestroy)
        {
            CustomCoroutine.WaitThenExecute(vfxObj.GetComponent<ParticleSystem>().main.duration, () =>
            {
                vfxObj.gameObject.SetActive(false);
            });
        }
        return vfxObj.gameObject;
    }

    public void PlayOneTimeVFX(string id, Vector3 position, Vector3 scale)
    {
        var vfx = all.Find(x => x.id == id);
        var vfxObj = Instantiate(vfx.gameObject, position, Quaternion.identity);
        vfxObj.transform.localScale = scale;
        vfxObj.gameObject.name = id;

        CustomCoroutine.WaitThenExecute(vfxObj.GetComponent<ParticleSystem>().main.duration, () =>
        {
            Destroy(vfxObj);
        });
    }

    //public void PlayOneTimeVFX(string id, Vector3 position, Color color,int rate = 0)
    //{
    //    var vfx = all.Find(x => x.id == id);
    //    var vfxObj = Instantiate(vfx.gameObject, position, Quaternion.identity);
    //    vfxObj.gameObject.name = id;
    //    var ps = vfxObj.GetComponent<ParticleSystem>();
    //    ps.startColor = color;
    //    if(rate != 0) ps.emissionRate = rate;

    //    CustomCoroutine.WaitThenExecute(vfxObj.GetComponent<ParticleSystem>().main.duration, () =>
    //    {
    //        Destroy(vfxObj);
    //    });
    //}

    //If neccessary
    private bool RepetitionIsTooFrequent(string vfxId, float frequencyMin, string entrySuffix = "")
    {
        float time = Time.unscaledTime;
        string vfxKey = vfxId + entrySuffix;

        if (limitedFrequencyVFX.ContainsKey(vfxKey))
        {
            if (time - frequencyMin > limitedFrequencyVFX[vfxKey])
            {
                limitedFrequencyVFX[vfxKey] = time;
                return false;
            }
        }
        else
        {
            limitedFrequencyVFX.Add(vfxKey, time);
            return false;
        }

        return true;
    }

    //ROR
    private void OnDestroy()
    {
        initialized = false;
    }

    private void Init()
    {
        all = Resources.LoadAll<VFX>("Prefabs/VFX").ToList();

        foreach (var vfx in all)
        {
            if(!vfx.repeating) continue;

            Debug.Log(vfx.id);
            CustomPool<VFX> pool = new CustomPool<VFX>(vfx,50);
            pools.Add(vfx.id, pool);
        }

        initialized = true;
    }
}

public class CustomPool<T> where T : MonoBehaviour
{
    private T prefab;
    private List<T> pool = new List<T>();

    private Transform parent;

    public CustomPool(T prefab, int prewarmObjects)
    {
        this.prefab = prefab;
        parent = new GameObject(prefab.name).transform;
        for (int i = 0; i < prewarmObjects; i++)
        {
            var obj = GameObject.Instantiate(prefab);
            obj.transform.parent = parent;
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    public T Get()
    {
        var obj = pool.FirstOrDefault(x => !x.isActiveAndEnabled);

        if (obj == null)
        {
            obj = Create();
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    private T Create()
    {
        var obj = GameObject.Instantiate(prefab);
        obj.gameObject.SetActive(false);
        obj.transform.parent = parent.transform;
        pool.Add(obj);
        return obj;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
    }
}
