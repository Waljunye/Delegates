using System.Collections;
using System.Collections.Generic;
using Animations;
using Pooling;
using UnityEngine;
using UnityEngine.UI;

public class Entry : MonoBehaviour
{
    [SerializeField]
    private Sprite imageSprite;
    
    [SerializeField]
    private GameObject prefab;
    private IEnumerator Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        GameObject imageGO = new GameObject("DynamicImage");
        imageGO.transform.SetParent(canvas.transform);

        Image image = imageGO.AddComponent<Image>();

        if (imageSprite != null)
        {
            image.sprite = imageSprite;
        }

        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 200);
        rectTransform.anchoredPosition = Vector2.zero;
        
        
        yield return TestImageDisappearAnonDelegate(image);
        yield return new WaitForSeconds(2.0f);
        var clr = image.color;
        clr.a = 1.0f;
        image.color = clr;
        
        yield return TestImageDisappearNamedDelegate(image);
        
        
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(10, 5, InitializePooledObject,
            GetPooledObject,
            DeinitializePooledObject, DestroyPooledGameObject);

        List<GameObject> a = new List<GameObject>();
        
        for (int i = 0; i < 10; i++)
        {
            a.Add(pool.Get());
        }
        var outOfLimit = pool.Get();
        foreach (var po in a)
        {
            pool.Release(po);
        }
        
        pool.Release(outOfLimit);
        
        StartCoroutine(TestClear(pool));
    }

    private GameObject InitializePooledObject()
    {
        return Instantiate(prefab);
    }

    private GameObject GetPooledObject(GameObject pooledObject)
    {
        pooledObject.SetActive(true);
        return pooledObject;
    }

    private void DeinitializePooledObject(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
    }

    private void DestroyPooledGameObject(GameObject pooledObject)
    {
        Destroy(pooledObject);
    }

    private IEnumerator TestClear<T>(ObjectPool<T> pool)
    {
        yield return new WaitForSeconds(5.0f);
        
        pool.Clear();
    }

    private IEnumerator TestImageDisappearAnonDelegate(Image image)
    {
        yield return UiAnimations.AnimateFadeOut(image, 2.0f, (elapsed) =>
        {
            Debug.Log($"\"Sosal\" disappeared, elapsed: {elapsed}");
        });
    }

    private IEnumerator TestImageDisappearNamedDelegate(Image image)
    {
        yield return UiAnimations.AnimateFadeOut(image, 2.0f, ActionOnDisappear);
    }

    private void ActionOnDisappear(float elapsed)
    {
        Debug.Log($"\"Sosal\" disappeared, elapsed: {elapsed}, но по-другому)))");
    }
}
