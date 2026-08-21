using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
public class Destroyer
{
    public List<GameObject> garbage = new List<GameObject>();
    private MonoBehaviour monoBeh;
    
    public Destroyer(MonoBehaviour monoBeh)
    {
        G.GC = this;
        this.monoBeh = monoBeh;
        this.monoBeh.StartCoroutine(Destroy());
    }

    public IEnumerator Destroy()
    {
        while (true)
        {
            while (garbage.Count == 0)
            {
                yield return new WaitForEndOfFrame();
            }

            for (int i = 0; i < 2; i++)
            {
                if (garbage.Count > 0)
                {
                    try
                    {
                        var item = garbage[0];

                        if (item == null)
                        {
                            garbage.RemoveAt(0);
                            continue;
                        }

                        try
                        {
                            item.transform.SetParent(null);
                            GameObject.Destroy(item);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Ошибка удаления объекта: {e.Message}");
                        }

                        garbage.RemoveAt(0);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}