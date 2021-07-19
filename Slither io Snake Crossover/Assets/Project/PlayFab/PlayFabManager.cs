using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using Utils;

namespace ClasePlayfab
{
    public class PlayFabManager : Singleton<PlayFabManager>
    {
        public bool IsTest = false;

        public static string PLAYFAB_TITLE_TEST = "E3E96";
        public static string PLAYFAB_TITLE_RELEASE = "9441B";

        public Action OnPlayFabLogin;

        private void Awake()
        {
            OnPlayFabLogin += OnLogin;
        }

        private void OnDestroy()
        {
            OnPlayFabLogin -= OnLogin;
        }

        void Start()
        {
            Login();
        }

        #region Login

        private void Login()
        {
            SetupPlayFabServer();
            PlayFabLogin();
        }

        private void SetupPlayFabServer()
        {
            string titleId = (IsTest ? PLAYFAB_TITLE_TEST : PLAYFAB_TITLE_RELEASE);
            PlayFabSettings.TitleId = titleId;
        }

        private void PlayFabLogin()
        {
            var request = new LoginWithCustomIDRequest
            {
                CreateAccount = true,
                CustomId = SystemInfo.deviceUniqueIdentifier
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailed);

            /*
            PlayFabClientAPI.LoginWithCustomID(request, 
                (result) => 
                { 
                    OnLoginSuccess(result); 
                }, 
                (error) =>
                {
                    OnLoginFailed(error);
                }
            );
            */
        }

        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Player Logged: " + result.PlayFabId);

            if (OnPlayFabLogin != null)
            {
                OnPlayFabLogin();
            }
        }

        private void OnLoginFailed(PlayFabError error)
        {
            Debug.LogError("Login failed: " + error.GenerateErrorReport());
        }


        public void OnLogin()
        {
            GetTitleData();
            //SetPlayerData();
            GetPlayerData();
        }

        #endregion

        #region TitleData

        public void GetTitleData()
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
                (result) =>
                {
                    GameManager.Instance.LoadGameSetup(result.Data);
                },
                (error) =>
                {
                    Debug.LogError("TitleData Error: " + error.GenerateErrorReport());
                });
        }

        #endregion

        #region PlayerData
        
        public void SetPlayerData()
        {
            PlayerModel player = GameManager.Instance.PlayerData;

            var request = new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {"City", player.City},
                    {"CurrentHealth", player.CurrentHealth.ToString()},
                    {"Strength", player.Strength.ToString()}
                },
            };

            PlayFabClientAPI.UpdateUserData(request,
                (result) =>
                {
                    Debug.Log("Successfully updated user data!");
                }, (error) =>
                {
                    Debug.LogError("Error updating user data: " + error.GenerateErrorReport());
                });
        }

        public void GetPlayerData()
        {
            var request = new GetUserDataRequest()
            {
                //PlayFabId = otherPlayerId,
                //Keys = Health, City...
            };

            PlayFabClientAPI.GetUserData(request,
                (result) =>
                {
                    GameManager.Instance.PlayerData = PlayerModel.LoadPlayerData(result.Data);
                },
                (error) =>
                {
                    Debug.LogError("Error getting user data: " + error.GenerateErrorReport());
                });
        }

        #endregion

        #region Catalog

        public void GetCatalogItems(Action<List<CatalogItem>> onSuccess, Action onError = null)
        {
            var request = new GetCatalogItemsRequest()
            {
                CatalogVersion = "MainCatalog"
            };

            PlayFabClientAPI.GetCatalogItems(request,
                (result) =>
                {
                    onSuccess(result.Catalog);
                },
                (error) =>
                {
                    onError?.Invoke();
                    // if (onError != null) onError();
                });
        }

        public void GetStoreItems(Action<List<StoreItem>> onSuccess, Action onError = null)
        {
            var request = new GetStoreItemsRequest()
            {
                CatalogVersion = "MainCatalog",
                StoreId = "mainstore"
            };

            PlayFabClientAPI.GetStoreItems(request,
                (result) =>
                {
                    onSuccess(result.Store);
                },
                (error) =>
                {
                    onError?.Invoke();
                });
        }

        public void PurchaseItem(StoreItem item, Action<List<ItemInstance>> onSuccess, Action onError = null)
        {
            var request = new PurchaseItemRequest()
            {
                CatalogVersion = "MainCatalog",
                StoreId = "mainstore",
                VirtualCurrency = "DI",
                Price = (int) item.VirtualCurrencyPrices["DI"],
                ItemId = item.ItemId
            };

            PlayFabClientAPI.PurchaseItem(request,
                (result) =>
                {
                    onSuccess(result.Items);
                },
                (error) =>
                {
                    onError?.Invoke();
                });
        }

        #endregion

        #region Inventory

        public void GetInventory(Action<GetUserInventoryResult> onSuccess, Action onError = null)
        {
            var request = new GetUserInventoryRequest()
            {

            };

            PlayFabClientAPI.GetUserInventory(request,
                (result) =>
                {
                    onSuccess(result);
                },
                (error) =>
                {
                    onError?.Invoke();
                });
        }

        #endregion

        #region Cloudscript Examples

        public void ServerHelloWorld()
        {
            var request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "helloWorld",
                FunctionParameter = new { inputValue = "here is my input!" },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (result) =>
                {
                    Debug.Log(result.FunctionResult.ToString());
                },
                (error) =>
                {
                });
        }

        public void ServerHelloWorldV2()
        {
            var request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "helloWorld",
                FunctionParameter = new { inputValue = "here is my input!" },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript(request,
                (result) =>
                {
                    HelloWorldResult myCustomResult = new HelloWorldResult();

                    JsonUtility.FromJsonOverwrite(result.FunctionResult.ToString(), myCustomResult);

                    Debug.Log(myCustomResult.messageValue);
                },
                (error) =>
                {
                });
        }

        public void ServerHelloWorldV3()
        {
            var request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "helloWorld",
                FunctionParameter = new { inputValue = "here is my input!" },
                GeneratePlayStreamEvent = true
            };

            PlayFabClientAPI.ExecuteCloudScript<HelloWorldResult>(request,
                (result) =>
                {
                    // HelloWorldResult myCustomResult = (HelloWorldResult)result.FunctionResult;
                    HelloWorldResult myCustomResult = result.FunctionResult as HelloWorldResult;

                    Debug.Log(myCustomResult.messageValue);
                },
                (error) =>
                {
                });
        }

        #endregion
    }

    [Serializable]
    public class CloudResult
    {
        public int Result;
    }

    [Serializable]
    public class HelloWorldResult : CloudResult
    {
        public string messageValue;
    }
}