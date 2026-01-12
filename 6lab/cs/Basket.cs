
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicStore;

namespace MusicStore
{
    public class Basket
    {
        // Константы
        private const int MAX_PRODUCTS = 100; // максимальное количество товаров в корзине

        // Поля класса
        private Product[] products;           // массив товаров
        private int[] quantities;             // массив количеств
        private int productCount;             // текущее количество товаров в корзине

        // Конструктор
        public Basket()
        {
            productCount = 0;
            products = new Product[MAX_PRODUCTS];
            quantities = new int[MAX_PRODUCTS];

            // Инициализация массивов
            for (int i = 0; i < MAX_PRODUCTS; i++)
            {
                products[i] = null;
                quantities[i] = 0;
            }
        }

        // Метод для расчета общей стоимости всех товаров в корзине
        public double GetBasketCost()
        {
            double totalCost = 0.0;
            // Проход по всем товарам в корзине
            for (int i = 0; i < productCount; i++)
            {
                if (products[i] != null)
                {// Добавляем стоимость товара: цена × количество
                    totalCost += products[i].GetPrice() * quantities[i];
                }
            }

            return totalCost;
        }

        // Метод для получения общего количества всех товаров в корзине
        public uint GetBasketCount()
        {
            uint totalCount = 0;

            for (int i = 0; i < productCount; i++)
            {
                totalCount += (uint)quantities[i];
            }

            return totalCount;
        }

        // Метод отображения содержимого корзины
        public void DisplayBasket()
        {
            Console.WriteLine("\n=== СОДЕРЖИМОЕ КОРЗИНЫ ===");

            if (productCount == 0)
            {
                Console.WriteLine("Корзина пуста!");
                Console.WriteLine("=========================");
                return;
            }

            Console.WriteLine($"{"№",-3} {"Артикул",-10} {"Название",-20} {"Цена",-8} {"Кол-во",-10} {"Сумма",-12}");
            Console.WriteLine(new string('-', 70));

            double totalCost = 0.0;// Переменная для общей стоимости

            for (int i = 0; i < productCount; i++)
            {
                if (products[i] != null)
                {// Расчет стоимости позиции: цена × количество
                    double itemTotal = products[i].GetPrice() * quantities[i];
                    totalCost += itemTotal;// Добавление к общей сумме
                    // Обработка слишком длинных названий (обрезаем если > 18 символов)
                    string productName = products[i].GetName();
                    if (productName.Length > 18)
                    {
                        productName = productName.Substring(0, 18) + "..";
                    }
                    // Вывод строки с информацией о товаре
                    Console.WriteLine($"{i + 1,-3} {products[i].GetArticle(),-10} {productName,-20} " +
                                      $"{products[i].GetPrice(),-8} {quantities[i],-10} {itemTotal,-12:F2}");
                }
            }

            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"ИТОГО: {totalCost:F2} руб.");
            Console.WriteLine($"Общее количество товаров: {GetBasketCount()}");
            Console.WriteLine("=========================");
        }

        // Метод добавления товара в корзину
        public void AddBasket(Product product, int quantity = 1)
        {
            // Проверка на переполнение корзины
            if (productCount >= MAX_PRODUCTS)
            {
                Console.WriteLine($"Корзина переполнена! Максимум {MAX_PRODUCTS} товаров.");
                return;
            }

            // Проверка корректности количества
            if (quantity <= 0)
            {
                Console.WriteLine("Количество должно быть положительным числом!");
                return;
            }

            // Проверяем, есть ли уже такой товар в корзине
            int existingIndex = FindProductIndex(product.GetArticle());

            // Если товар найден, увеличиваем количество
            if (existingIndex != -1)
            {
                quantities[existingIndex] += quantity;
                Console.WriteLine($"Количество товара '{product.GetName()}' увеличено на {quantity}. " +
                                 $"Теперь: {quantities[existingIndex]} шт.");
            }
            else
            {
                // Добавляем новый товар
                products[productCount] = product;
                quantities[productCount] = quantity;
                productCount++;
                Console.WriteLine($"Товар '{product.GetName()}' добавлен в корзину в количестве {quantity} шт.");
            }
        }

        // Метод удаления товара из корзины по индексу
        public void DeleteBasket(int productIndex)
        {
            // Проверка корректности индекса (пользователь вводит с 1, а не с 0)
            if (productIndex < 1 || productIndex > productCount)
            {
                Console.WriteLine("Неверный номер товара!");
                return;
            }

            int actualIndex = productIndex - 1;// Преобразуем в индекс массива
            string productName = products[actualIndex].GetName();//название

            // Сдвигаем элементы массива влево, начиная с удаляемого элемента
            for (int i = actualIndex; i < productCount - 1; i++)
            {
                products[i] = products[i + 1];  // сдвиг указателей на товары
                quantities[i] = quantities[i + 1];  // сдвиг количеств
            }

            // Очищаем последнюю ячейку
            products[productCount - 1] = null;
            quantities[productCount - 1] = 0;
            productCount--;

            Console.WriteLine($"Товар '{productName}' удален из корзины.");
        }

        // Метод обновления количества товара по индексу
        public int UpdateQuantity(int productIndex, int newQuantity)
        {
            if (productIndex < 1 || productIndex > productCount)
            {
                Console.WriteLine("Неверный номер товара!");
                return -1;
            }

            if (newQuantity <= 0)
            {
                Console.WriteLine("Количество должно быть положительным числом!");
                return -1;
            }

            int actualIndex = productIndex - 1;
            int oldQuantity = quantities[actualIndex];
            quantities[actualIndex] = newQuantity;

            Console.WriteLine($"Количество товара '{products[actualIndex].GetName()}' " +
                             $"изменено с {oldQuantity} на {newQuantity} шт.");

            return newQuantity;
        }

        // Метод проверки, пуста ли корзина
        public bool IsEmpty()
        {
            return productCount == 0;
        }

        // Метод полной очистки корзины
        public void ClearBasket()
        {
            for (int i = 0; i < productCount; i++)
            {
                products[i] = null;
                quantities[i] = 0;
            }
            productCount = 0;
        }

        // Метод поиска индекса товара в корзине по артикулу
        public int FindProductIndex(int article)
        {
            for (int i = 0; i < productCount; i++)
            {
                if (products[i] != null && products[i].GetArticle() == article)
                {
                    return i;
                }
            }
            return -1;
        }

        // Метод получения количества различных товаров в корзине
        public int GetProductCount()
        {
            return productCount;
        }

        // Метод получения товара по индексу с проверкой границ массива
        public Product GetProduct(int index)
        {
            if (index < 0 || index >= productCount)
            {
                return null;
            }
            return products[index];
        }

        // Метод получения количества товара по индексу с проверкой границ массива
        public int GetQuantity(int index)
        {
            if (index < 0 || index >= productCount)
            {
                return 0;
            }
            return quantities[index];
        }

        // Свойства для доступа к полям
        public int ProductCount
        {
            get { return productCount; }
        }


        ~Basket()
        {
            ClearBasket();
        }
    }
}