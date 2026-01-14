using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class Client : User
    {
        private List<Order> orderHistory;
        private double totalSpent;
        private int loyaltyPoints;
        private string address;
        private bool newsletterSubscription;

        // Конструкторы
        // вызов конструктора базового класса с параметрами
        public Client(string name, string email = "", string phone = "", string address = "")
            : base(GenerateUserId("CLT"), name, email, phone) // ВЫЗОВ КОНСТРУКТОРА БАЗОВОГО КЛАССА User с параметрами
        {
            this.totalSpent = 0.0;
            this.loyaltyPoints = 0;
            this.address = address;
            this.newsletterSubscription = true;
            this.orderHistory = new List<Order>();
        }

        public Client(string id, string name, string email, string phone,
            string address, double totalSpent = 0.0, int loyaltyPoints = 0, bool newsletter = true)
            : base(id, name, email, phone)
        {
            InitializeClientData(totalSpent, loyaltyPoints, address, newsletter);
        }

        // Переопределение метода базового класса
        public override void DisplayInfo()
        {
            ShowBasicInfo();

            Console.WriteLine("\n--- ИНФОРМАЦИЯ О КЛИЕНТЕ ---");
            Console.WriteLine($"Адрес: {(string.IsNullOrEmpty(address) ? "не указан" : address)}");
            Console.WriteLine($"Общая сумма покупок: {totalSpent:F2} руб.");
            Console.WriteLine($"Баллы лояльности: {loyaltyPoints}");
            Console.WriteLine($"Количество заказов: {orderHistory.Count}");
            Console.WriteLine($"Подписка на рассылку: {(newsletterSubscription ? "Да" : "Нет")}");
            Console.WriteLine($"Текущая скидка: {CalculateDiscount()}%");
            Console.WriteLine("================================");
        }

        // Переопределение метода базового класса
        public override double CalculateDiscount()
        {
            if (totalSpent >= 10000) return 10.0;
            if (totalSpent >= 5000) return 5.0;
            if (totalSpent >= 1000) return 2.0;
            return 0.0;
        }

        // Методы клиента
        public void AddOrder(Order order)
        {
            if (order != null)
            {
                orderHistory.Add(order);
                UpdateTotalSpent(order.GetCostOrder());
                UpdateLoyaltyPoints((int)(order.GetCostOrder() / 100));
                Console.WriteLine($"Заказ добавлен в историю клиента: {GetName()}");
            }
        }

        public void UpdateTotalSpent(double amount)
        {
            if (amount > 0)
            {
                totalSpent += amount;
                Console.WriteLine($"Общая сумма покупок обновлена: {totalSpent:F2} руб.");
            }
        }

        public void UpdateLoyaltyPoints(int points)
        {
            if (points > 0)
            {
                loyaltyPoints += points;
                Console.WriteLine($"Баллы лояльности обновлены: {loyaltyPoints} баллов");
            }
        }

        public void ShowOrderHistory()
        {
            if (orderHistory.Count == 0)
            {
                Console.WriteLine("\nИстория заказов пуста!");
                return;
            }

            Console.WriteLine("\n=== ИСТОРИЯ ЗАКАЗОВ ===");
            for (int i = 0; i < orderHistory.Count; i++)
            {
                Console.WriteLine($"Заказ {i + 1}: №{orderHistory[i].GetNumberOrder()}, " +
                    $"Сумма: {orderHistory[i].GetCostOrder():F2} руб.");
            }
            Console.WriteLine("======================");
        }

        // Геттеры
        public double GetTotalSpent() => totalSpent;
        public int GetLoyaltyPoints() => loyaltyPoints;
        public string GetAddress() => address;
        public bool GetNewsletterSubscription() => newsletterSubscription;
        public int GetOrderCount() => orderHistory.Count;

        // Сеттеры
        public void SetAddress(string newAddress) => address = newAddress;
        public void SetNewsletterSubscription(bool subscribe) => newsletterSubscription = subscribe;

        private void InitializeClientData(double totalSpent, int loyaltyPoints,
            string address, bool newsletter)
        {
            this.totalSpent = totalSpent;
            this.loyaltyPoints = loyaltyPoints;
            this.address = address;
            this.newsletterSubscription = newsletter;
            this.orderHistory = new List<Order>();
        }

        // Переопределение ToString
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());

            // Добавляем специфичную информацию клиента
            sb.AppendLine("\n--- ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ (Client) ---");
            sb.AppendLine($"Адрес: {(string.IsNullOrEmpty(address) ? "не указан" : address)}");
            sb.AppendLine($"Общая сумма покупок: {totalSpent:F2} руб.");
            sb.AppendLine($"Баллы лояльности: {loyaltyPoints}");
            sb.AppendLine($"Количество заказов: {orderHistory.Count}");
            sb.AppendLine($"Подписка на рассылку: {(newsletterSubscription ? "Да" : "Нет")}");
            sb.AppendLine($"Скидка: {CalculateDiscount()}%");
            sb.AppendLine("==================================");

            return sb.ToString();
        }
    }
}