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

    [Inject] Vault vault;

    async Task Initialize()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.LogWarning($"Does this work?");
    }

    public async void Save()
    {
        await Initialize();

        Dictionary<string, object> content = new ()
        {
            {_time, DateTime.UtcNow},
            {_gold, vault.gold.value.Value}
        };

        var saved =
            await CloudSaveService.Instance.Data.Player.SaveAsync(content);

        Debug.Log("Saved: " + string.Join(",", saved));
    }

    public async void Load(string json)
    {
        await Initialize();

        HashSet<string> keys = new HashSet<string>() {
            _time,
            _gold
        };

        var loaded =
            await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

        vault.gold.value.Value = loaded[_gold].Value.GetAs<int>();
    }
}
