using UnityEngine;

public class LineRendererTo28x28 : MonoBehaviour
{
    public Camera renderCamera;        // 专门用于渲染的相机
    public RenderTexture renderTexture; // 28x28 RT

    public Texture2D ConvertToTexture()
    {
        // 1️⃣ 让相机渲染到RT
        renderCamera.targetTexture = renderTexture;
        renderCamera.Render();

        // 2️⃣ 读取RT
        RenderTexture.active = renderTexture;

        Texture2D tex = new Texture2D(28, 28, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, 28, 28), 0, 0);
        tex.Apply();

        // 3️⃣ 清理
        renderCamera.targetTexture = null;
        RenderTexture.active = null;

        return tex;
    }
}