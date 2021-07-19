using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace ClasePlayfab
{
    public class ShopManager : Singleton<ShopManager>
    {
        public List<CatalogItem> CatalogItems;
        public List<StoreItem> StoreItems;

        private void Awake()
        {
            PlayFabManager.Instance.OnPlayFabLogin += LoadCatalog;
            PlayFabManager.Instance.OnPlayFabLogin += LoadStore;
            PlayFabManager.Instance.OnPlayFabLogin += ShowPlayerInventory;
        }

        private void OnDestroy()
        {
            PlayFabManager.Instance.OnPlayFabLogin -= LoadCatalog;
            PlayFabManager.Instance.OnPlayFabLogin -= LoadStore;
            PlayFabManager.Instance.OnPlayFabLogin -= ShowPlayerInventory;
        }

        private void Start()
        {
            //Invoke("MakeTestPurchase", 5);
        }

        public void LoadCatalog()
        {
            PlayFabManager.Instance.GetCatalogItems(
                (catalog) =>
                {
                    CatalogItems = catalog;
                    ShowCatalog();
                });
        }

        public void ShowCatalog()
        {
            foreach(CatalogItem item in CatalogItems)
            {
                Debug.Log("[Catalog]: " + item.DisplayName);
            }
        }

        public void LoadStore()
        {
            PlayFabManager.Instance.GetStoreItems(
                (store) =>
                {
                    StoreItems = store;
                });
        }

        public void ShowStore()
        {
            foreach (StoreItem item in StoreItems)
            {
                Debug.Log("[Store]: " + item.ItemId);
            }
        }

        public void MakeTestPurchase()
        {
            PlayFabManager.Instance.PurchaseItem(StoreItems[0],
                (items) =>
                {
                    foreach(ItemInstance item in items)
                    {
                        Debug.Log(item.ItemId);
                    }
                });
        }

        public void ShowPlayerInventory()
        {
            PlayFabManager.Instance.GetInventory(
                (inventory) =>
                {
                    foreach(ItemInstance item in inventory.Inventory)
                    {
                        Debug.Log("Player owns " + item.ItemId);
                    }

                    Debug.Log("Player Diamonds are: " + inventory.VirtualCurrency["DI"]);
                });
        }
    }
}