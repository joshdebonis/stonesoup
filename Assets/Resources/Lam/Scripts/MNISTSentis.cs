using UnityEngine;

using System.Linq;
using Unity.InferenceEngine;
using Random = UnityEngine.Random;

public class MNISTSentis : MonoBehaviour
{
    public ModelAsset modelAsset;

    private Model runtimeModel;
    private Worker worker;
    
    public Texture2D inputTexture;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = new Worker(runtimeModel, BackendType.CPU);
    }

    public void RunInference(Texture2D texture)
    {
        inputTexture = texture;
        if (inputTexture == null)
        {
            Debug.LogError("没有输入Texture！");
            return;
        }

        if (inputTexture.width != 28 || inputTexture.height != 28)
        {
            Debug.LogError("Texture必须是28x28！");
            return;
        }

        var input = new Tensor<float>(new TensorShape(1, 1, 28, 28));

        Color[] pixels = inputTexture.GetPixels();

        for (int y = 0; y < 28; y++)
        {
            for (int x = 0; x < 28; x++)
            {
                int index = y * 28 + x;

                // 转灰度（MNIST是单通道）
                float gray = pixels[index].grayscale;

                // ⚠️ 如果你背景是黑字是白，不用改
                // ⚠️ 如果背景白字黑，要用 1f - gray

                input[0, 0, y, x] = gray;
            }
        }

        worker.Schedule(input);

        var output = worker.PeekOutput() as Tensor<float>;
        float[] data = output.DownloadToArray();

        int predicted = 0;
        float maxValue = float.MinValue;

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] > maxValue)
            {
                maxValue = data[i];
                predicted = i;
            }
        }

        Debug.Log("预测结果: " + predicted);

        input.Dispose();
        output.Dispose();
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}