using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using System;
using System.Threading.Tasks;

public class SaveSystem : MonoBehaviour
{
    static readonly string KEY = "Tut-turu";

    private const string _time = "time";
    private const string _gold = "gold";
    private const string _spawnerData = "spawnerData";
    private const string _journeySaveState = "journeySaveState";

    Content newestLoadedContent;

    [Inject] Vault vault;

    async Task MaybeInitialize()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (AuthenticationService.Instance.IsExpired ||
            !AuthenticationService.Instance.IsAuthorized)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void Save()
    {
        await MaybeInitialize();

        object sdf = (1, 2, "sdf");

        Dictionary<string, object> content = new ()
        {
            {_time, DateTime.UtcNow},
            {_gold, vault.gold.value.Value},
        };

        var saved =
            await CloudSaveService.Instance.Data.Player.SaveAsync(content);

        Debug.Log("Saved: " + string.Join(",", saved));
    }

    public async Task<Content> Load()
    {
        return null;

        /// TODO: make switch cases on all tasks

        await MaybeInitialize();

        if (newestLoadedContent == null)
        {
            newestLoadedContent = await Load("*PLACEHOLDER*");
        }
        /// TODO: if you saved after last loading than newestLoadedContent needs to be reloaded

        return newestLoadedContent;
    }

    async Task<Content> Load(string json)
    {
        HashSet<string> keys = new HashSet<string>() {
            _time,
            _gold
        };

        var loaded =
            await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

        vault.gold.value.Value = loaded[_gold].Value.GetAs<int>();


        Debug.Log("Loaded: " + string.Join(",", loaded));

        return new Content
        {
            journeySaveState = loaded[_journeySaveState].Value.GetAs<Journey.SaveState>()
        };
    }

    public class Content
    {
        public Journey.SaveState journeySaveState;
    }
}
