using UnityEngine;
using Unity.InferenceEngine;

public class ONNXRunner : MonoBehaviour
{
    public ModelAsset modelAsset;

    private Worker worker;

    void Start()
    {
        // 加载模型
        var model = ModelLoader.Load(modelAsset);

        // 创建 worker
        worker = new Worker(model, BackendType.GPUCompute);

        // 创建输入 tensor
        var input = new Tensor<float>(new TensorShape(1, 3, 224, 224));

        // 填充数据
        input[0] = 1f;

        // 执行推理
        worker.Schedule(input);

        // 获取输出
        var output = worker.PeekOutput() as Tensor<float>;

        Debug.Log(output[0]);

        input.Dispose();
        output.Dispose();
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}