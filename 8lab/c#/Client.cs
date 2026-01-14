using System;
using System.Collections.Generic;

namespace MusicStore
{
    public class Client : User
    {
        // Поля класса
        private List<Order> orderHistory;
        private double totalSpent;
        private int loyaltyPoints;
        private string address;
        private bool newsletterSubscription;


        // Конструктор 1
        public Client(string name, string email = "", string phone = "", string address = "")
            : base(GenerateUserId("CLT"), name, email, phone)  // Вызов конструктора базового класса
        {
            InitializeClientData(0.0, 0, address, true);
        }

        // Конструктор 2
        public Client(string id, string name, string email, string phone,
                     string address, double totalSpent = 0.0,
                     int loyaltyPoints = 0, bool newsletter = true)
            : base(id, name, email, phone)  // Вызов конструктора базового класса
        {
            InitializeClientData(totalSpent, loyaltyPoints, address, newsletter);
        }

        // Конструктор 3
        public Client(string name, string address)
            : this(name, "", "", address)  // Вызов другого конструктора этого же класса
        {
        }

        public override void DisplayInfo()
        {

            Console.WriteLine("\n=== ИНФОРМАЦИЯ О КЛИЕНТЕ ===");
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Имя: {Name}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Телефон: {Phone}");
            Console.WriteLine($"Адрес: {(string.IsNullOrEmpty(address) ? "не указан" : address)}");
            Console.WriteLine($"Общая сумма покупок: {totalSpent:F2} руб.");
            Console.WriteLine($"Баллы лояльности: {loyaltyPoints}");
            Console.WriteLine($"Количество заказов: {orderHistory.Count}");
            Console.WriteLine($"Подписка на рассылку: {(newsletterSubscription ? "Да" : "Нет")}");
            Console.WriteLine($"Текущая скидка: {CalculateDiscount():F1}%");
            Console.WriteLine("================================");
        }


        public override double CalculateDiscount()
        {
            if (totalSpent >= 10000) return 10.0;
            if (totalSpent >= 5000) return 5.0;
            if (totalSpent >= 1000) return 2.0;
            return 0.0;
        }

        // Перегрузка оператора присваивания 
        public Client AssignFromUser(User user)
        {
            if (this != user)
            {
                
                CopyPropertiesFromUser(user);

                // Устанавливаем значения по умолчанию для Client
                totalSpent = 0.0;
                loyaltyPoints = 0;
                address = "Адрес не указан (присвоено)";
                newsletterSubscription = true;

                // Очищаем историю заказов
                orderHistory.Clear();
            }
            return this;
        }

        // Вспомогательный метод для копирования свойств из User
        private void CopyPropertiesFromUser(User user)
        {
            
        }

        // Методы клиента
        public void AddOrder(Order order)
        {
            if (order != null)
            {
                orderHistory.Add(order);
                UpdateTotalSpent(order.CostOrder);
                UpdateLoyaltyPoints((int)(order.CostOrder / 100));
                Console.WriteLine($"Заказ добавлен в историю клиента: {Name}");
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
                Console.WriteLine($"Заказ {i + 1}: №{orderHistory[i].NumberOrder}, " +
                                $"Сумма: {orderHistory[i].CostOrder:F2} руб.");
            }
            Console.WriteLine("======================");
        }

        private void InitializeClientData(double totalSpent, int loyaltyPoints,
                                         string address, bool newsletter)
        {
            this.totalSpent = totalSpent;
            this.loyaltyPoints = loyaltyPoints;
            this.address = address;
            this.newsletterSubscription = newsletter;
            orderHistory = new List<Order>();
        }

        // Геттеры
        public double TotalSpent => totalSpent;
        public int LoyaltyPoints => loyaltyPoints;
        public string Address => address;
        public bool NewsletterSubscription => newsletterSubscription;
        public int OrderCount => orderHistory.Count;

        // Сеттеры
        public void SetAddress(string newAddress) => address = newAddress;
        public void SetNewsletterSubscription(bool subscribe) => newsletterSubscription = subscribe;


        public override string ToString()
        {
            return base.ToString() +
                   $"\n--- ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ (Client) ---\n" +
                   $"Адрес: {(string.IsNullOrEmpty(address) ? "не указан" : address)}\n" +
                   $"Общая сумма покупок: {totalSpent:F2} руб.\n" +
                   $"Баллы лояльности: {loyaltyPoints}\n" +
                   $"Количество заказов: {orderHistory.Count}\n" +
                   $"Подписка на рассылку: {(newsletterSubscription ? "Да" : "Нет")}\n" +
                   $"Скидка: {CalculateDiscount():F1}%\n" +
                   "==================================\n";
        }
    }
}