using UnityEditor;
using UnityEngine;

public class ResampleAnimationClip : MonoBehaviour
{
    public AnimationClip originalClip;
    public float newFrameRate = 30f; // ���ο� ������ ����Ʈ

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

            // �ִϸ��̼� Ŭ�� ���� (������ ����)
#if UNITY_EDITOR
            string path = "Assets/NewAnimationClip.anim";
            UnityEditor.AssetDatabase.CreateAsset(newClip, path);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}
