using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    // структура товара для вычислений (аналог constexpr вычислений)
    public struct SimpleProduct
    {
        public SimpleProduct(int art, double pr, int qty)
        {
            article = art;
            price = pr;
            quantity = qty;
        }

        public int article;
        public double price;
        public int quantity;

        public double GetTotal()
        {
            return price * quantity;
        }
    }

    public struct BasketConfig
    {
        public BasketConfig(int maxProd, double tax, double discount)
        {
            maxProducts = maxProd;
            defaultTaxRate = tax;
            maxDiscount = discount;
        }

        public int maxProducts;
        public double defaultTaxRate;
        public double maxDiscount;
    }

    // Внешние функции для вычислений (аналог constexpr)
    public static class BasketCalculations
    {
        public static double CalculateBasketCostCompileTime(double price, int quantity)
        {
            return price * quantity;
        }

        public static double CalculateDiscount(double price, double discountPercent)
        {
            return price * (100.0 - discountPercent) / 100.0;
        }

        public static double CalculateTax(double price, double taxRate)
        {
            return price * (1.0 + taxRate / 100.0);
        }
    }

    public class SimpleBasket
    {
        private const int MAX_PRODUCTS = 10;
        private SimpleProduct[] products;
        private int productCount;

        public SimpleBasket()
        {
            productCount = 0;
            products = new SimpleProduct[MAX_PRODUCTS];
        }

        // метод добавления товара
        public void AddProduct(SimpleProduct product)
        {
            if (productCount < MAX_PRODUCTS)
            {
                products[productCount] = product;
                productCount++;
            }
        }

        // метод вычисления общей стоимости
        public double CalculateTotal()
        {
            double total = 0.0;
            for (int i = 0; i < productCount; ++i)
            {
                total += products[i].GetTotal();
            }
            return total;
        }

        // метод вычисления стоимости со скидкой
        public double CalculateDiscountedTotal(double discountPercent)
        {
            double total = CalculateTotal();
            return total * (100.0 - discountPercent) / 100.0;
        }

        // метод вычисления стоимости с налогом
        public double CalculateTotalWithTax(double taxRate)
        {
            double total = CalculateTotal();
            return total * (1.0 + taxRate / 100.0);
        }

        // Геттеры
        public int GetProductCount() { return productCount; }
        public int GetMaxProducts() { return MAX_PRODUCTS; }
        public SimpleProduct GetProduct(int index)
        {
            return products[index];
        }
    }

    // Основной класс Basket
    public class Basket
    {
        private const int MAX_PRODUCTS = 100;
        private Product[] products;
        private int[] quantities;
        private int productCount;

        // Конфигурация корзины
        public static readonly BasketConfig basketConfig = new BasketConfig(150, 0.18, 70.0);

        // Простые const вычисления (аналог constexpr)
        public const double SAMPLE_PRICE = 100.0;
        public const int SAMPLE_QUANTITY = 3;
        public const double SAMPLE_TOTAL = SAMPLE_PRICE * SAMPLE_QUANTITY;
        public const double SAMPLE_DISCOUNT = 10.0;
        public const double SAMPLE_DISCOUNTED_TOTAL = SAMPLE_TOTAL * (100.0 - SAMPLE_DISCOUNT) / 100.0;
        public const double SAMPLE_TAX_RATE = 20.0;
        public const double SAMPLE_TOTAL_WITH_TAX = SAMPLE_TOTAL * (1.0 + SAMPLE_TAX_RATE / 100.0);

        // Конструктор
        public Basket()
        {
            productCount = 0;
            products = new Product[MAX_PRODUCTS];
            quantities = new int[MAX_PRODUCTS];
        }

        // Конструктор копирования
        public Basket(Basket other)
        {
            productCount = other.productCount;
            products = new Product[MAX_PRODUCTS];
            quantities = new int[MAX_PRODUCTS];

            for (int i = 0; i < productCount; ++i)
            {
                if (other.products[i] != null)
                {
                    products[i] = new Product(
                        other.products[i].GetArticle(),
                        other.products[i].GetName(),
                        other.products[i].GetBrand(),
                        other.products[i].GetProducerCountry(),
                        other.products[i].GetCategory(),
                        other.products[i].GetTypeProduct(),
                        other.products[i].GetDescription(),
                        other.products[i].GetQuantityStock(),
                        other.products[i].GetPrice()
                    );
                    quantities[i] = other.quantities[i];
                }
            }
        }

        // Основные методы
        public double GetBasketCost()
        {
            return CalculateBasketTotal<double>(
                (product, quantity) => product.GetPrice() * quantity
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

        public void DisplayBasket()
        {
            Console.WriteLine(ToString());
        }

        public void AddBasket(Product product, int quantity = 1)
        {
            AddProductToBasket(this, product, quantity);
        }

        public void DeleteBasket(int productIndex)
        {
            if (productIndex < 1 || productIndex > productCount)
            {
                Console.WriteLine("Неверный номер товара!");
                return;
            }

            int actualIndex = productIndex - 1;
            string productName = products[actualIndex].GetName();

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

        public bool IsEmpty()
        {
            return productCount == 0;
        }

        public void ClearBasket()
        {
            ClearProducts();
            Console.WriteLine("Корзина очищена!");
        }

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

        // Геттеры
        public int GetProductCount() { return productCount; }
        public Product GetProduct(int index)
        {
            if (index < 0 || index >= productCount)
            {
                return null;
            }
            return products[index];
        }

        public int GetQuantity(int index)
        {
            if (index < 0 || index >= productCount)
            {
                return 0;
            }
            return quantities[index];
        }

        // ШАБЛОННАЯ ФУНКЦИЯ (обобщенный метод в C#)
        public T CalculateBasketTotal<T>(Func<Product, int, T> costCalculator) where T : struct
        {
            if (!typeof(T).IsPrimitive && typeof(T) != typeof(decimal))
            {
                throw new ArgumentException("Тип T должен быть числовым (int, float, double и т.д.)");
            }

            dynamic total = default(T);
            for (int i = 0; i < productCount; ++i)
            {
                if (products[i] != null)
                {
                    total += costCalculator(products[i], quantities[i]);
                }
            }
            return (T)total;
        }

        // Методы, использующие шаблонную функцию
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

        // Переопределение ToString() (аналог operator<<)
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

            sb.AppendLine(string.Format("{0,-3}{1,-10}{2,-20}{3,-8}{4,-10}{5,-12}",
                "№", "Артикул", "Название", "Цена", "Кол-во", "Сумма"));
            sb.AppendLine("-------------------------------------------------------------");

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

                    sb.AppendLine(string.Format("{0,-3}{1,-10}{2,-20}{3,-8:F2}{4,-10}{5,-12:F2}",
                        (i + 1),
                        products[i].GetArticle(),
                        productName,
                        products[i].GetPrice(),
                        quantities[i],
                        itemTotal));
                }
            }

            sb.AppendLine("-------------------------------------------------------------");
            sb.AppendLine($"ИТОГО: {totalCost:F2} руб.");
            sb.AppendLine($"Общее количество товаров: {GetBasketCount()} шт.");
            sb.AppendLine("=========================");

            return sb.ToString();
        }

        // Статические методы (аналог дружественных функций)

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
                        result.products[result.productCount] = new Product(
                            basket1.products[i].GetArticle(),
                            basket1.products[i].GetName(),
                            basket1.products[i].GetBrand(),
                            basket1.products[i].GetProducerCountry(),
                            basket1.products[i].GetCategory(),
                            basket1.products[i].GetTypeProduct(),
                            basket1.products[i].GetDescription(),
                            basket1.products[i].GetQuantityStock(),
                            basket1.products[i].GetPrice()
                        );
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
                        result.products[result.productCount] = new Product(
                            basket2.products[i].GetArticle(),
                            basket2.products[i].GetName(),
                            basket2.products[i].GetBrand(),
                            basket2.products[i].GetProducerCountry(),
                            basket2.products[i].GetCategory(),
                            basket2.products[i].GetTypeProduct(),
                            basket2.products[i].GetDescription(),
                            basket2.products[i].GetQuantityStock(),
                            basket2.products[i].GetPrice()
                        );
                        result.quantities[result.productCount] = basket2.quantities[i];
                        result.productCount++;
                    }
                }
            }

            return result;
        }

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
                                $"увеличено на {quantity}. Теперь: " +
                                $"{basket.quantities[existingIndex]} шт.");
            }
            else
            {
                // Добавляем новый товар
                basket.products[basket.productCount] = new Product(
                    product.GetArticle(),
                    product.GetName(),
                    product.GetBrand(),
                    product.GetProducerCountry(),
                    product.GetCategory(),
                    product.GetTypeProduct(),
                    product.GetDescription(),
                    product.GetQuantityStock(),
                    product.GetPrice()
                );
                basket.quantities[basket.productCount] = quantity;
                basket.productCount++;

                Console.WriteLine($"Товар '{product.GetName()}' " +
                                $"добавлен в корзину в количестве {quantity} шт.");
            }
        }

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
                        filteredBasket.products[filteredBasket.productCount] = new Product(
                            basket.products[i].GetArticle(),
                            basket.products[i].GetName(),
                            basket.products[i].GetBrand(),
                            basket.products[i].GetProducerCountry(),
                            basket.products[i].GetCategory(),
                            basket.products[i].GetTypeProduct(),
                            basket.products[i].GetDescription(),
                            basket.products[i].GetQuantityStock(),
                            basket.products[i].GetPrice()
                        );
                        filteredBasket.quantities[filteredBasket.productCount] = basket.quantities[i];
                        filteredBasket.productCount++;
                    }
                }
            }

            return filteredBasket;
        }

        public static SimpleBasket CreateSampleBasket()
        {
            SimpleBasket basket = new SimpleBasket();
            basket.AddProduct(new SimpleProduct(1, 100.0, 2));
            basket.AddProduct(new SimpleProduct(2, 50.0, 3));
            return basket;
        }

        private void ClearProducts()
        {
            for (int i = 0; i < productCount; ++i)
            {
                products[i] = null;
                quantities[i] = 0;
            }
            productCount = 0;
        }
    }
}
