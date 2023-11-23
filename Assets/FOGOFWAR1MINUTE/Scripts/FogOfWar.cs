using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogOfWar : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public RawImage rawImage;

    public Texture2D fogOfWarTexture;
    public SpriteMask spriteMask;

    private Vector2 worldScale;
    private Vector2Int pixelScale;

    Color xx = new Color(0, 0, 0, 0.5f);

    [System.Serializable]
    public struct UnitIN
    {
        public GameObject obj;
        public float holeRadius;
    }
    [System.Serializable]
    public struct UnitOUT
    {
        public Vector2 position;
        public float holeRadius;
    }

    public List<UnitIN> holes = new List<UnitIN>();
    public UnitOUT[] units;

    public void Start()
    {
        renderTexture = new RenderTexture(256, 256, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        rawImage.texture = toTexture2D(renderTexture);
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    public void Awake()
    {
        pixelScale.x = fogOfWarTexture.width;
        pixelScale.y = fogOfWarTexture.height;
        worldScale.x = pixelScale.x / 100f * transform.localScale.x;
        worldScale.y = pixelScale.y / 100f * transform.localScale.y;
        for (int i = 0; i < pixelScale.x; i++)
        {
            for (int j = 0; j < pixelScale.y; j++)
            {
                fogOfWarTexture.SetPixel(i, j, Color.clear);
            }
        }
    }

    private Vector2Int WorldToPixel(Vector2 position) {
        Vector2Int pixelPosition = Vector2Int.zero;

        float dx = position.x - transform.position.x;
        float dy = position.y - transform.position.y;

        pixelPosition.x = Mathf.RoundToInt(.5f * pixelScale.x + dx * (pixelScale.x / worldScale.x));
        pixelPosition.y = Mathf.RoundToInt(.5f * pixelScale.y + dy * (pixelScale.y / worldScale.y));
        return pixelPosition;

    }

    public void MakeHole(Vector2 position, float holeRadius) {
        Vector2Int pixelPosition = WorldToPixel(position);
        int radius = Mathf.RoundToInt(holeRadius * pixelScale.x / worldScale.x);
        int px, nx, py, ny, distance;
        for (int i = 0; i < radius; i++) {
            distance = Mathf.RoundToInt(Mathf.Sqrt(radius * radius - i * i));
            for (int j = 0; j < distance; j++) {
                px = Mathf.Clamp(pixelPosition.x + i, 0, pixelScale.x);
                nx = Mathf.Clamp(pixelPosition.x - i, 0, pixelScale.x);
                py = Mathf.Clamp(pixelPosition.y + j, 0, pixelScale.y);
                ny = Mathf.Clamp(pixelPosition.y - j, 0, pixelScale.y);

                fogOfWarTexture.SetPixel(px, py, xx);
                fogOfWarTexture.SetPixel(nx, py, xx);
                fogOfWarTexture.SetPixel(px, ny, xx);
                fogOfWarTexture.SetPixel(nx, ny, xx);
            }
        }
        fogOfWarTexture.Apply();
        CreateSprite();
    }

    public void AddUnit(GameObject _obj, float _holeRadius)
    {
        holes.Add(new UnitIN { obj = _obj, holeRadius = _holeRadius });
    }
    private void CreateSprite() {
        spriteMask.sprite = Sprite.Create(fogOfWarTexture, new Rect(0, 0, fogOfWarTexture.width, fogOfWarTexture.height), Vector2.one * .5f, 100);
    }
}
