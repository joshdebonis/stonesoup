using UnityEngine;

namespace Lam
{
    public class TestPencil : MonoBehaviour
    {
        public LineRendererTo28x28 LineRendererTo28x28;
        public MNISTSentis MNISTSentis;
        public PencilDrawer PencilDrawer;
        public Camera MainCamera; // 如果没有赋值，可以在Start里用 Camera.main
        
        void Update()
        {
            // 1️⃣ 获取鼠标屏幕坐标
            Vector2 mouseScreenPos = Input.mousePosition;

            // 2️⃣ 转换成世界坐标
            // 注意：ScreenToWorldPoint需要一个 Vector3，z是相对于摄像机的距离
            Vector3 mouseWorldPos = MainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPos.x, 
                mouseScreenPos.y, 
                MainCamera.nearClipPlane // 或者你想要的平面距离
            ));

            // 3️⃣ 如果是2D场景，通常z为0
            mouseWorldPos.z = 0;

            if (Input.GetMouseButton(0))
            {
                PencilDrawer.Draw(mouseWorldPos);
            }
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                MNISTSentis.RunInference(LineRendererTo28x28.ConvertToTexture());
            }
        }
    }
}
