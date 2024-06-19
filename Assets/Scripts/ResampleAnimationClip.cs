using UnityEditor;
using UnityEngine;

public class ResampleAnimationClip : MonoBehaviour
{
    public AnimationClip originalClip;
    public float newFrameRate = 30f; // 새로운 프레임 레이트

    void Start()
    {
        if (originalClip != null)
        {
            AnimationClip newClip = new AnimationClip();
            newClip.frameRate = newFrameRate;

            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(originalClip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(originalClip, binding);
                Keyframe[] keys = curve.keys;
                Keyframe[] newKeys = new Keyframe[(int)(keys.Length * (newFrameRate / originalClip.frameRate))];

                for (int i = 0; i < newKeys.Length; i++)
                {
                    float time = (i / newFrameRate);
                    float value = curve.Evaluate(time);
                    newKeys[i] = new Keyframe(time, value);
                }

                AnimationCurve newCurve = new AnimationCurve(newKeys);
                newClip.SetCurve(binding.path, binding.type, binding.propertyName, newCurve);
            }

            // 애니메이션 클립 저장 (에디터 전용)
#if UNITY_EDITOR
            string path = "Assets/NewAnimationClip.anim";
            UnityEditor.AssetDatabase.CreateAsset(newClip, path);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}
