using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableInitializer : MonoBehaviour
{
    void Awake()
    {
        // Addressables √ ±‚»≠
        Addressables.InitializeAsync().Completed += OnAddressablesInitialized;
    }

    void OnAddressablesInitialized(AsyncOperationHandle<IResourceLocator> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Addressables successfully initialized.");
        }
        else
        {
            Debug.LogError("Addressables initialization failed.");
        }
    }
}
