using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace YandexGameSdk
{
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    [DisallowMultipleComponent]
    public class YandexGamePayments : YandexGameService
    {
        [DllImport("__Internal")] private static extern string YSDKInitPayments();
        [DllImport("__Internal")] private static extern string YSDKLoadPlayerPurchases();
        [DllImport("__Internal")] private static extern string YSDKStartPurchase(string productID);
        [DllImport("__Internal")] private static extern string YSDKConsumePurchase(string purchaseToken);
        [DllImport("__Internal")] private static extern string YSDKLoadCatalog();


        private List<ProductConfig> allProducts;

        // --- Init
        public enum PaymentInitResult
        {
            OK,
            Error
        }

        private TaskCompletionSource<PaymentInitResult> initTask;
        public async Task<PaymentInitResult> Init(List<ProductConfig> allProducts)
        {
            this.allProducts = allProducts;
            initTask = new TaskCompletionSource<PaymentInitResult>();

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKInitPayments();
                return await initTask.Task;
            }
            else
            {
                DebugMessage("Yandex payments inited! (Editor)");
                initTask.SetResult(PaymentInitResult.OK);
                return await initTask.Task;
            }
        }

        private void OnPaymentInit()
        {
            initTask.SetResult(PaymentInitResult.OK);
            DebugMessage("Yandex payments inited!");
        }

        private void OnPaymentError()
        {
            initTask.SetResult(PaymentInitResult.Error);
            DebugMessage("Yandex payments error!");
        }

        //----

        //---- Load Purchase
        private TaskCompletionSource<Purchase[]> loadAllPurchasesTask;

        public async Task<Purchase[]> LoadPlayerPurchases()
        {
            loadAllPurchasesTask = new TaskCompletionSource<Purchase[]>();

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKLoadPlayerPurchases();
                return await loadAllPurchasesTask.Task;

            }
            else
            {
               
                DebugMessage("Player purchases loaded.");
                loadAllPurchasesTask.SetResult(new Purchase[0]);
                return await loadAllPurchasesTask.Task;
            }
        }

        private void OnRecivePlayerPurchases(string playerPurchasesJSON)
        {
            DebugMessage("Get purchases from JS:" + playerPurchasesJSON);

            if(playerPurchasesJSON == "[]")
            {
                DebugMessage("Player purchases is empty!");
                loadAllPurchasesTask.SetResult(new Purchase[0]);
                return;
            }

            string arrayJSON = "{ \"Items\":" + playerPurchasesJSON + "}";

            Purchase[] purchases = JsonHelper.FromJson<Purchase>(arrayJSON);

            DebugMessage("Player purchases loaded."); 

            for(int i = 0; i < purchases.Length; i++)
            {
                DebugMessage("Purchas " + (i + 1).ToString() + " " + purchases[i].productID + " " + purchases[i].purchaseToken + " " + purchases[i].developerPayload);
            }

            loadAllPurchasesTask.SetResult(purchases);
        }

        private void OnRecivePlayerPurchasesError()
        {
            DebugMessage("Player purchases loaded error!");

            loadAllPurchasesTask.SetResult(new Purchase[0]);
        }

        //---- Purchase
        private Action<Purchase> cachedPurchased;
        private Action cachedPurchasedError;

        public void StartPurchase(string productID, Action<Purchase> onPurchased = null, Action onError = null)
        {
            cachedPurchased = onPurchased;
            cachedPurchasedError = onError;

            ProductConfig productConfig = allProducts.Find( (x) => x.ProductID == productID);

            if(productConfig == null)
            {
                DebugMessage("Poroduct with id " + productID + " not exists in purchase list!");
                return;
            }

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKStartPurchase(productID);
            }
            else
            {
                DebugMessage("Poroduct with id " + productID + "purchased!");
                onPurchased?.Invoke(new Purchase());

                cachedPurchased = null;
                cachedPurchasedError = null;
            }
        }

        private void OnPurchaseSuccses(string purchaseJSON)
        {
            Purchase purchase = JsonUtility.FromJson<Purchase>(purchaseJSON);

            DebugMessage("OnPurchaseSuccses " + purchaseJSON);

            ProductConfig productConfig = GetProductConfigByPurchase(purchase);

            if(productConfig != null)
            {
                productConfig.ApplyEffect();

                if (productConfig.Type == ProductType.Consumable)
                {
                    ConsumePurchase(purchase.purchaseToken);
                }
            }

            cachedPurchased.Invoke(purchase);
            cachedPurchased = null;
            cachedPurchasedError = null;
        }

        private void OnPurchaseError(string error)
        {
            cachedPurchasedError?.Invoke();
            cachedPurchasedError = null;

            DebugMessage("OnPurchaseError " + error);
        }

        //----Consume
        private TaskCompletionSource<bool> consumePurchaseTask;
        public async Task<bool> ConsumePurchase(string purchaseToken)
        {
            consumePurchaseTask = new TaskCompletionSource<bool>();

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKConsumePurchase(purchaseToken);
                return await consumePurchaseTask.Task;
            }
            else
            {
                DebugMessage("Purchase sonsumed");
                consumePurchaseTask.SetResult(true);
                return await consumePurchaseTask.Task;
            }
        }

        private void OnPurchaseConsumed()
        {
            DebugMessage("OnPurchaseConsumed ");
            consumePurchaseTask.SetResult(true);
        }

        private void OnPurchaseConsumedError(string error)
        {
            DebugMessage("OnPurchaseConsumedError " + error);
            consumePurchaseTask.SetResult(false);
        }

        //------Consume all purchase
        private TaskCompletionSource<bool> consumePlayerPurchaseTask;

        public async Task<bool> ConsumePlayerPurchase(bool forceAll = false)
        {
            consumePlayerPurchaseTask = new TaskCompletionSource<bool>();

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                Purchase[] playerPurchases = await LoadPlayerPurchases();

                for (int i = 0; i < playerPurchases.Length; i++)
                {
                    ProductConfig productConfig = GetProductConfigByPurchase(playerPurchases[i]);

                    if (productConfig == null) continue;

                    productConfig.ApplyEffect();

                    if (productConfig.Type == ProductType.Consumable || forceAll == true)
                    {
                        await ConsumePurchase(playerPurchases[i].purchaseToken); 
                    }
                }

                consumePlayerPurchaseTask.SetResult(true);
                return await consumePlayerPurchaseTask.Task;

            }
            else
            {
                consumePlayerPurchaseTask.SetResult(true);
                return await consumePlayerPurchaseTask.Task;
            }
        }

        //-------

        //---- Load catalog
        private TaskCompletionSource<ProductList> loadCatalogTask;
        public async Task<ProductList> LoadCatalog()
        {
            loadCatalogTask = new TaskCompletionSource<ProductList>();

            if (Application.platform == RuntimePlatform.WebGLPlayer && config.BuildForYandexGame == true)
            {
                YSDKLoadCatalog();
                return await loadCatalogTask.Task;
            }
            else
            {
                loadCatalogTask.SetResult(new ProductList());
                return await loadCatalogTask.Task;
            }
        }

        public async Task<Product> GetProductById(string pruductID)
        {
            ProductList productList = await LoadCatalog();

            for(int i = 0; i < productList.Count; i++)
            {
                if (productList[i].id == pruductID)
                    return productList[i];
            }

            return null;
        }
        
        private void OnCatalogLoaded(string catalogJSON)
        {
            if (catalogJSON == "[]")
            {
                DebugMessage("Catalock is empty!");
                loadCatalogTask.SetResult(new ProductList());
                return;
            }

            string arrayJSON = "{ \"Products\":" + catalogJSON + "}";

            ProductList productList = JsonUtility.FromJson<ProductList>(arrayJSON);

            DebugMessage(productList);

            for(int i = 0; i < productList.Count; i++)
                DebugMessage("Продукт    -   -   -" + productList[i].priceCurrencyCode);
                

            loadCatalogTask.SetResult(productList);
        }




        //-----

        public async Task<bool> IsPurchased(string productId)
        {
            Purchase[] playerPurchases = await LoadPlayerPurchases();

            if (playerPurchases.Length == 0) return false;

            for(int i = 0; i < playerPurchases.Length; i++)
            {
                Debug.Log(playerPurchases[i].productID + "    --  " + productId);
                if (playerPurchases[i].productID == productId)
                    return true;
            }
            return false;
        }

        public ProductConfig GetProductConfigByPurchase(Purchase purchase)
        {
            for (int j = 0; j < allProducts.Count; j++)
            {
                if (allProducts[j].ProductID == purchase.productID)
                {
                    return allProducts[j];
                }
            }

            return null;
        }

    }
}