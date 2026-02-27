using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YandexGameSdk
{
    [Serializable]
    public class Product
    {
        public string id;                   // Идентификатор товара
        public string title;                // Название
        public string description;          // Описание
        public string imageURI;             // URL изображения
        public string price;                // Стоимость (<цена> <код валюты>)
        public string priceValue;           // Стоимость в числовом формате
        public string priceCurrencyCode;    // Код валюты (например, "USD", "EUR")
    }

    [Serializable]
    public class ProductList
    {
        public List<Product> Products; // Список доступных товаров

        public ProductList()
        {
            Products = new List<Product>();
        }

        // Индексатор для доступа к элементам по индексу
        public Product this[int index]
        {
            get
            {
                if (index < 0 || index >= Products.Count)
                    throw new IndexOutOfRangeException($"Индекс {index} вне диапазона (0 - {Products.Count - 1})");
                return Products[index];
            }
            set
            {
                if (index < 0 || index >= Products.Count)
                    throw new IndexOutOfRangeException($"Индекс {index} вне диапазона (0 - {Products.Count - 1})");
                Products[index] = value;
            }
        }

        // Дополнительно можно добавить свойство Count для удобства
        public int Count => Products.Count;
    }
}
