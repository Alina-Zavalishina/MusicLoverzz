using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopSystem
{

    public class Basket : IDisposable
    {
        private const int MAX_PRODUCTS = 100;
        private Product[] products = new Product[MAX_PRODUCTS];
        private int[] quantities = new int[MAX_PRODUCTS];
        private int productCount = 0;

        // Конструктор
        public Basket()
        {
            Console.WriteLine("Вызван конструктор по умолчанию Basket");
        }

        // Конструктор копирования
        public Basket(Basket other)
        {
            Console.WriteLine("Вызван конструктор копирования Basket");
            DeepCopy(other);
        }

        // Деструктор (реализация IDisposable)
        public void Dispose()
        {
            Console.WriteLine("Вызван деструктор Basket");
            ClearProducts();
        }

        // Очистка всех продуктов
        private void ClearProducts()
        {
            for (int i = 0; i < productCount; ++i)
            {
                if (products[i] != null)
                {
                    products[i].Dispose();
                    products[i] = null;
                }
                quantities[i] = 0;
            }
            productCount = 0;
        }

        // Глубокое копирование
        private void DeepCopy(Basket other)
        {
            ClearProducts();
            productCount = other.productCount;

            for (int i = 0; i < productCount; ++i)
            {
                if (other.products[i] != null)
                {
                    products[i] = new Product(other.products[i]);
                    quantities[i] = other.quantities[i];
                }
            }
        }


        public T CalculateBasketTotal<T>(Func<Product, int, T> costCalculator) where T : struct
        {
            // Ограничение на уровне компиляции для числовых типов
            if (!typeof(T).IsValueType)
            {
                throw new ArgumentException("Тип T должен быть типом значения (int, float, double и т.д.)");
            }

            dynamic total = default(T);

            for (int i = 0; i < productCount; ++i)
            {
                if (products[i] != null)
                {
                    total += costCalculator(products[i], quantities[i]);
                }
            }

            return total;
        }

        public double GetBasketCost()
        {

            return CalculateBasketTotal<double>(
                (product, quantity) => product.GetPrice() * quantity
            );
        }


        public double CalculateCostWithTax(double taxRate)
        {
            if (taxRate < 0)
            {
                Console.WriteLine("Ошибка: налог не может быть отрицательным!");
                return GetBasketCost();
            }


            return CalculateBasketTotal<double>(
                (product, quantity) =>
                {
                    double price = product.GetPrice();
                    double itemTotal = price * quantity;
                    return itemTotal * (1.0 + taxRate / 100.0);
                }
            );
        }


        public double CalculateCostWithDiscount(double discountPercent)
        {
            if (discountPercent < 0 || discountPercent > 100)
            {
                Console.WriteLine("Ошибка: скидка должна быть от 0 до 100%!");
                return GetBasketCost();
            }

            return CalculateBasketTotal<double>(
                (product, quantity) =>
                {
                    double price = product.GetPrice();
                    double itemTotal = price * quantity;
                    return itemTotal * (100.0 - discountPercent) / 100.0;
                }
            );
        }


        public int CalculateTotalQuantity()
        {

            return CalculateBasketTotal<int>(
                (product, quantity) => quantity
            );
        }


        public uint GetBasketCount()
        {
            uint totalCount = 0;
            for (int i = 0; i < productCount; ++i)
            {
                totalCount += (uint)quantities[i];
            }
            return totalCount;
        }

        // Отображение корзины
        public void DisplayBasket()
        {
            Console.WriteLine(this);
        }

        // Добавление товара в корзину
        public void AddBasket(Product product, int quantity = 1)
        {
            AddProductToBasket(this, product, quantity);
        }

        // Удаление товара из корзины
        public void DeleteBasket(int productIndex)
        {
            if (productIndex < 1 || productIndex > productCount)
            {
                Console.WriteLine("Неверный номер товара!");
                return;
            }

            int actualIndex = productIndex - 1;
            string productName = products[actualIndex].GetName();

            products[actualIndex].Dispose();

            for (int i = actualIndex; i < productCount - 1; ++i)
            {
                products[i] = products[i + 1];
                quantities[i] = quantities[i + 1];
            }

            products[productCount - 1] = null;
            quantities[productCount - 1] = 0;
            productCount--;

            Console.WriteLine($"Товар '{productName}' удален из корзины.");
        }

        // Изменение количества товара
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

        // Проверка пустоты корзины
        public bool IsEmpty()
        {
            return productCount == 0;
        }

        // Очистка корзины
        public void ClearBasket()
        {
            ClearProducts();
            Console.WriteLine("Корзина очищена!");
        }

        // Поиск индекса товара по артикулу
        public int FindProductIndex(int article)
        {
            for (int i = 0; i < productCount; ++i)
            {
                if (products[i] != null && products[i].GetArticle() == article)
                {
                    return i;
                }
            }
            return -1;
        }

        // Получение товара по индексу
        public Product GetProduct(int index)
        {
            if (index < 0 || index >= productCount)
            {
                return null;
            }
            return products[index];
        }

        // Получение количества товара по индексу
        public int GetQuantity(int index)
        {
            if (index < 0 || index >= productCount)
            {
                return 0;
            }
            return quantities[index];
        }

        // Геттер для количества товаров
        public int ProductCount => productCount;


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\n=== СОДЕРЖИМОЕ КОРЗИНЫ ===");

            if (productCount == 0)
            {
                sb.AppendLine("Корзина пуста!");
                sb.AppendLine("=========================");
                return sb.ToString();
            }

            sb.AppendLine(string.Format("{0,-3} {1,-10} {2,-20} {3,-8} {4,-10} {5,-12}",
                "№", "Артикул", "Название", "Цена", "Кол-во", "Сумма"));
            sb.AppendLine("-----------------------------------------------------------");

            double totalCost = 0.0;

            for (int i = 0; i < productCount; ++i)
            {
                if (products[i] != null)
                {
                    double itemTotal = products[i].GetPrice() * quantities[i];
                    totalCost += itemTotal;

                    string productName = products[i].GetName();
                    if (productName.Length > 18)
                    {
                        productName = productName.Substring(0, 18) + "..";
                    }

                    sb.AppendLine(string.Format("{0,-3} {1,-10} {2,-20} {3,-8:F2} {4,-10} {5,-12:F2}",
                        (i + 1),
                        products[i].GetArticle(),
                        productName,
                        products[i].GetPrice(),
                        quantities[i],
                        itemTotal));
                }
            }

            sb.AppendLine("-----------------------------------------------------------");
            sb.AppendLine($"ИТОГО: {totalCost:F2} руб.");
            sb.AppendLine($"Общее количество товаров: {GetBasketCount()} шт.");
            sb.AppendLine("=========================");

            return sb.ToString();
        }



        // Объединение двух корзин
        public static Basket MergeBaskets(Basket basket1, Basket basket2)
        {
            Basket result = new Basket();

            // Объединяем товары из первой корзины
            for (int i = 0; i < basket1.productCount; ++i)
            {
                if (basket1.products[i] != null)
                {
                    bool found = false;

                    for (int j = 0; j < result.productCount; ++j)
                    {
                        if (result.products[j] != null &&
                            result.products[j].GetArticle() == basket1.products[i].GetArticle())
                        {
                            result.quantities[j] += basket1.quantities[i];
                            found = true;
                            break;
                        }
                    }

                    if (!found && result.productCount < MAX_PRODUCTS)
                    {
                        result.products[result.productCount] = new Product(basket1.products[i]);
                        result.quantities[result.productCount] = basket1.quantities[i];
                        result.productCount++;
                    }
                }
            }

            // Объединяем товары из второй корзины
            for (int i = 0; i < basket2.productCount; ++i)
            {
                if (basket2.products[i] != null)
                {
                    bool found = false;

                    for (int j = 0; j < result.productCount; ++j)
                    {
                        if (result.products[j] != null &&
                            result.products[j].GetArticle() == basket2.products[i].GetArticle())
                        {
                            result.quantities[j] += basket2.quantities[i];
                            found = true;
                            break;
                        }
                    }

                    if (!found && result.productCount < MAX_PRODUCTS)
                    {
                        result.products[result.productCount] = new Product(basket2.products[i]);
                        result.quantities[result.productCount] = basket2.quantities[i];
                        result.productCount++;
                    }
                }
            }

            return result;
        }

        // Расчет скидки
        public static double CalculateBasketDiscount(Basket basket, double discountPercent)
        {
            double discountedCost = basket.CalculateCostWithDiscount(discountPercent);
            double originalCost = basket.GetBasketCost();
            double discountAmount = originalCost - discountedCost;

            Console.WriteLine("\n=== РАСЧЕТ СКИДКИ ===");
            Console.WriteLine($"Общая стоимость: {originalCost:F2} руб.");
            Console.WriteLine($"Процент скидки: {discountPercent}%");
            Console.WriteLine($"Сумма скидки: {discountAmount:F2} руб.");
            Console.WriteLine($"Итоговая цена: {discountedCost:F2} руб.");
            Console.WriteLine("====================");

            return discountAmount;
        }

        // Сравнение двух корзин
        public static bool AreBasketsEqual(Basket basket1, Basket basket2)
        {
            if (basket1.productCount != basket2.productCount)
            {
                return false;
            }

            for (int i = 0; i < basket1.productCount; ++i)
            {
                if (basket1.products[i] == null || basket2.products[i] == null)
                {
                    return false;
                }

                if (basket1.products[i].GetArticle() != basket2.products[i].GetArticle())
                {
                    return false;
                }

                if (basket1.quantities[i] != basket2.quantities[i])
                {
                    return false;
                }
            }

            return true;
        }

        // Добавление товара в корзину (статический метод)
        public static void AddProductToBasket(Basket basket, Product product, int quantity)
        {
            if (basket.productCount >= MAX_PRODUCTS)
            {
                Console.WriteLine($"Корзина переполнена! Максимум {MAX_PRODUCTS} товаров.");
                return;
            }

            if (quantity <= 0)
            {
                Console.WriteLine("Количество должно быть положительным числом!");
                return;
            }

            // Ищем товар с таким же артикулом
            int existingIndex = -1;
            for (int i = 0; i < basket.productCount; ++i)
            {
                if (basket.products[i] != null &&
                    basket.products[i].GetArticle() == product.GetArticle())
                {
                    existingIndex = i;
                    break;
                }
            }

            if (existingIndex != -1)
            {
                // Увеличиваем количество существующего товара
                basket.quantities[existingIndex] += quantity;
                Console.WriteLine($"Количество товара '{product.GetName()}' " +
                                 $"увеличено на {quantity}. Теперь: {basket.quantities[existingIndex]} шт.");
            }
            else
            {
                // Добавляем новый товар
                basket.products[basket.productCount] = new Product(product);
                basket.quantities[basket.productCount] = quantity;
                basket.productCount++;
                Console.WriteLine($"Товар '{product.GetName()}' " +
                                 $"добавлен в корзину в количестве {quantity} шт.");
            }
        }

        // Проверка наличия товара
        public static bool BasketContainsProduct(Basket basket, int article)
        {
            for (int i = 0; i < basket.productCount; ++i)
            {
                if (basket.products[i] != null && basket.products[i].GetArticle() == article)
                {
                    return true;
                }
            }
            return false;
        }

        // Копирование корзины с фильтром по цене
        public static Basket CopyBasketWithPriceFilter(Basket basket, int maxPrice)
        {
            Basket filteredBasket = new Basket();

            for (int i = 0; i < basket.productCount; ++i)
            {
                if (basket.products[i] != null &&
                    basket.products[i].GetPrice() <= maxPrice)
                {
                    if (filteredBasket.productCount < MAX_PRODUCTS)
                    {
                        filteredBasket.products[filteredBasket.productCount] =
                            new Product(basket.products[i]);
                        filteredBasket.quantities[filteredBasket.productCount] =
                            basket.quantities[i];
                        filteredBasket.productCount++;
                    }
                }
            }

            return filteredBasket;
        }
    }
}
