using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YandexGameSdk
{
    [System.Serializable]
    public class ProductConfig
    {
        public string ProductID;
        public ProductType Type;

        private Action ApplyConsumableEffect;
        private Action ApplyNonConsumableEffect;

        public ProductConfig(string productID, ProductType type, Action effect)
        {
            ProductID = productID;
            Type = type;

            if (type == ProductType.Consumable)
                ApplyConsumableEffect = effect;

            if (type == ProductType.NonConsumable)
                ApplyNonConsumableEffect = effect;
        }

        public void ApplyEffect()
        {
            if (Type == ProductType.Consumable)
                ApplyConsumableEffect?.Invoke();

            if (Type == ProductType.NonConsumable)
                ApplyNonConsumableEffect?.Invoke();
        }
    }
}
