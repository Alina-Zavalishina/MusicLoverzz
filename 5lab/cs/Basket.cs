using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicShopSystem
{
    public class Basket
    {
        // Константы
        private const int MAX_PRODUCTS = 100; // максимальное количество товаров в корзине

        // Поля класса
        private Product[] products; // массив товаров
        private int[] quantities; // массив количеств
        private int productCount; // текущее количество товаров в корзине

        // Конструктор
        public Basket()
        {
            this.productCount = 0;
            this.products = new Product[MAX_PRODUCTS];
            this.quantities = new int[MAX_PRODUCTS];

            // Инициализация массивов
            for (int i = 0; i < MAX_PRODUCTS; i++)
            {
                this.products[i] = null;
                this.quantities[i] = 0;
            }
        }

        // Метод для расчета общей стоимости всех товаров в корзине
        public double GetBasketCost()
        {
            double totalCost = 0.0;

            // Проход по всем товарам в корзине
            for (int i = 0; i < this.productCount; i++)
            {
                if (this.products[i] != null)
                {
                    // Добавляем стоимость товара: цена × количество
                    totalCost += this.products[i].GetPrice() * this.quantities[i];
                }
            }

            return totalCost;
        }

        // Метод для получения общего количества всех товаров в корзине
        public uint GetBasketCount()
        {
            uint totalCount = 0;

            for (int i = 0; i < this.productCount; i++)
            {
                totalCount += (uint)this.quantities[i];
            }

            return totalCount;
        }

        // Метод отображения содержимого корзины
        public void DisplayBasket()
        {
            if (this.productCount == 0)
            {
                Console.WriteLine("\n=== СОДЕРЖИМОЕ КОРЗИНЫ ===");
                Console.WriteLine("Корзина пуста!");
                Console.WriteLine("=========================");
                return;
            }

            Console.WriteLine("\n=== СОДЕРЖИМОЕ КОРЗИНЫ ===");
            Console.WriteLine($"{"№",-3} {"Артикул",-10} {"Название",-20} {"Цена",-8} {"Кол-во",-10} {"Сумма",-12}");
            Console.WriteLine(new string('-', 70));

            double totalCost = 0.0; // Переменная для общей стоимости

            for (int i = 0; i < this.productCount; i++)
            {
                if (this.products[i] == null)
                {
                    throw new InvalidOperationException($"Товар с индексом {i} в корзине равен null");
                }

                // Расчет стоимости позиции: цена × количество
                double itemTotal = this.products[i].GetPrice() * this.quantities[i];
                totalCost += itemTotal; // Добавление к общей сумме

                // Обработка слишком длинных названий (обрезаем если > 18 символов)
                string productName = this.products[i].GetName();
                if (productName.Length > 18)
                {
                    productName = productName.Substring(0, 18) + "..";
                }

                // Вывод строки с информацией о товаре
                Console.WriteLine($"{i + 1,-3} {this.products[i].GetArticle(),-10} {productName,-20} " +
                                  $"{this.products[i].GetPrice(),-8} {this.quantities[i],-10} {itemTotal,-12:F2}");
            }

            Console.WriteLine(new string('-', 70));
            Console.WriteLine($"ИТОГО: {totalCost:F2} руб.");
            Console.WriteLine($"Общее количество товаров: {this.GetBasketCount()}");
            Console.WriteLine("=========================");
        }

        // Метод добавления товара в корзину
        public void AddBasket(Product product, int quantity = 1)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Товар не может быть null");
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity),
                    "Количество должно быть положительным числом");
            }

            // Проверка на переполнение корзины
            if (this.productCount >= MAX_PRODUCTS)
            {
                throw new InvalidOperationException($"Корзина переполнена! Максимум {MAX_PRODUCTS} товаров.");
            }

            // Проверяем, есть ли уже такой товар в корзине
            int existingIndex = this.FindProductIndex(product.GetArticle());

            // Если товар найден, увеличиваем количество
            if (existingIndex != -1)
            {
                this.quantities[existingIndex] += quantity;
                Console.WriteLine($"Количество товара '{product.GetName()}' увеличено на {quantity}. " +
                                  $"Теперь: {this.quantities[existingIndex]} шт.");
            }
            else
            {
                // Добавляем новый товар
                this.products[this.productCount] = product;
                this.quantities[this.productCount] = quantity;
                this.productCount++;
                Console.WriteLine($"Товар '{product.GetName()}' добавлен в корзину в количестве {quantity} шт.");
            }
        }

        // Метод удаления товара из корзины по индексу
        public void DeleteBasket(int productIndex)
        {
            // Проверка корректности индекса (пользователь вводит с 1, а не с 0)
            if (productIndex < 1 || productIndex > this.productCount)
            {
                throw new ArgumentOutOfRangeException(nameof(productIndex),
                    $"Неверный номер товара! Допустимый диапазон: 1-{this.productCount}");
            }

            int actualIndex = productIndex - 1; // Преобразуем в индекс массива

            if (this.products[actualIndex] == null)
            {
                throw new InvalidOperationException($"Товар с индексом {actualIndex} в корзине равен null");
            }

            string productName = this.products[actualIndex].GetName(); // название товара

            // Сдвигаем элементы массива влево, начиная с удаляемого элемента
            for (int i = actualIndex; i < this.productCount - 1; i++)
            {
                this.products[i] = this.products[i + 1]; // сдвиг указателей на товары
                this.quantities[i] = this.quantities[i + 1]; // сдвиг количеств
            }

            // Очищаем последнюю ячейку
            this.products[this.productCount - 1] = null;
            this.quantities[this.productCount - 1] = 0;
            this.productCount--;

            Console.WriteLine($"Товар '{productName}' удален из корзины.");
        }

        // Метод обновления количества товара по индексу
        public int UpdateQuantity(int productIndex, int newQuantity)
        {
            if (productIndex < 1 || productIndex > this.productCount)
            {
                throw new ArgumentOutOfRangeException(nameof(productIndex),
                    $"Неверный номер товара! Допустимый диапазон: 1-{this.productCount}");
            }

            if (newQuantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newQuantity),
                    "Количество должно быть положительным числом");
            }

            int actualIndex = productIndex - 1;

            if (this.products[actualIndex] == null)
            {
                throw new InvalidOperationException($"Товар с индексом {actualIndex} в корзине равен null");
            }

            int oldQuantity = this.quantities[actualIndex];
            this.quantities[actualIndex] = newQuantity;

            Console.WriteLine($"Количество товара '{this.products[actualIndex].GetName()}' " +
                              $"изменено с {oldQuantity} на {newQuantity} шт.");

            return newQuantity;
        }

        // Метод проверки, пуста ли корзина
        public bool IsEmpty()
        {
            return this.productCount == 0;
        }

        // Метод полной очистки корзины
        public void ClearBasket()
        {
            for (int i = 0; i < this.productCount; i++)
            {
                this.products[i] = null;
                this.quantities[i] = 0;
            }
            this.productCount = 0;
        }

        // Метод поиска индекса товара в корзине по артикулу
        public int FindProductIndex(int article)
        {
            for (int i = 0; i < this.productCount; i++)
            {
                if (this.products[i] != null && this.products[i].GetArticle() == article)
                {
                    return i;
                }
            }
            return -1;
        }

        // Метод получения количества различных товаров в корзине
        public int GetProductCount()
        {
            return this.productCount;
        }

        // Метод получения товара по индексу с проверкой границ массива
        public Product GetProduct(int index)
        {
            if (index < 0 || index >= this.productCount)
            {
                throw new IndexOutOfRangeException($"Индекс {index} выходит за пределы корзины [0, {this.productCount - 1}]");
            }

            if (this.products[index] == null)
            {
                throw new InvalidOperationException($"Товар с индексом {index} в корзине равен null");
            }

            return this.products[index];
        }

        // Метод получения количества товара по индексу с проверкой границ массива
        public int GetQuantity(int index)
        {
            if (index < 0 || index >= this.productCount)
            {
                throw new IndexOutOfRangeException($"Индекс {index} выходит за пределы корзины [0, {this.productCount - 1}]");
            }

            return this.quantities[index];
        }

        // Свойства для доступа к полям
        public int ProductCountProperty
        {
            get { return this.productCount; }
        }

        ~Basket()
        {
            this.ClearBasket();
        }
    }
}
