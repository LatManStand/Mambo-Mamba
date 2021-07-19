using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace ClasePlayfab
{
    public class GameManager : Singleton<GameManager>
    {
        public string GameVersion;
        public EconomyModel ServerEconomy;

        public PlayerModel PlayerData;

        private void Awake()
        {
            PlayFabManager.Instance.OnPlayFabLogin += HelloWorld;
        }

        private void OnDestroy()
        {
            PlayFabManager.Instance.OnPlayFabLogin -= HelloWorld;
        }

        public void LoadGameSetup(Dictionary<string,string> data)
        {
            SetPlayFabVersion(data["ClientVersion"]);
            SetPlayFabEconomyModel(data["EconomySetup"]);
        }

        private void SetPlayFabVersion(string version)
        {
            GameVersion = version;
        }

        private void SetPlayFabEconomyModel(string economyJson)
        {
            JsonUtility.FromJsonOverwrite(economyJson, ServerEconomy);
        }

        public void HelloWorld()
        {
            PlayFabManager.Instance.ServerHelloWorldV2();
        }
    }
}