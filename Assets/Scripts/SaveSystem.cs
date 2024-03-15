using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudSave.Models;

public class SaveSystem : MonoBehaviour
{
    static readonly string KEY = "Tut-turu";

    Action makeSaveDict;
    Action processLoadDict;
    Action afterLoad;

    HashSet<string> keys = new();
    Dictionary<string, object> saveDict = new();
    Dictionary<string, Unity.Services.CloudSave.Models.Item> loadDict = new();

    public void Register(string key, Func<object> getter,
                         Action<Unity.Services.CloudSave.Internal.Http.IDeserializable> setter,
                         Action afterLoad)
    {
        Register(key, getter, setter);

        this.afterLoad += afterLoad;
    }

    public void Register(string key, Func<object> getter,
                         Action<Unity.Services.CloudSave.Internal.Http.IDeserializable> setter)
    {
        Debug.Log($"Register {key}");

        keys.Add(key);

        makeSaveDict += () =>
        {
            saveDict.Add(key, getter.Invoke());
        };

        processLoadDict += () =>
        {
            if (loadDict.TryGetValue(key, out Item item))
                setter.Invoke(item.Value);
        };
    }

    async UniTask MaybeInitialize()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;
       
        if (UnityServices.State == ServicesInitializationState.Initializing)
        {
            Debug.Log($"Initializing...");
            await UniTask.WaitUntil(() =>
                                    UnityServices.State == ServicesInitializationState.Initialized);
            return;
        }

        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log($"Start Initialization");

            await UnityServices.InitializeAsync();
        }

        if (AuthenticationService.Instance.IsExpired ||
            !AuthenticationService.Instance.IsAuthorized)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async UniTask Save()
    {
        // await MaybeInitialize();

        saveDict = new();
        makeSaveDict.Invoke();

        // var saved = await CloudSaveService.Instance.Data.Player.SaveAsync(saveDict);

        // Debug.Log("Saved: " + string.Join(",", saved));
    }

    public async UniTask Load()
    {
        // await MaybeInitialize();

        // loadDict = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

        Debug.Log("Loaded: " + string.Join(",", loadDict));

        processLoadDict.Invoke();

        afterLoad?.Invoke();
    }
}
