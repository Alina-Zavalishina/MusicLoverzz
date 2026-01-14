using System;
using System.IO;
using System.Collections.Generic;

namespace MusicShopSystem
{
    public class Client
    {

        private const int MAX_ORDER_HISTORY = 100;  // максимальное количество заказов в истории


        private string clientId;                    // уникальный идентификатор клиента
        private string clientName;                  // имя клиента
        private Order[] orderHistory;               // история заказов
        private int orderCount;                     // текущее количество заказов в истории

        // Конструктор по умолчанию
        public Client()
        {
            clientId = "USER001";
            clientName = "Пользователь";
            orderCount = 0;
            orderHistory = new Order[MAX_ORDER_HISTORY];
            InitializeArrays();  // Инициализация массива значениями по умолчанию
        }

        // Конструктор с параметрами
        public Client(string id)
        {
            clientId = id;
            clientName = "Пользователь";
            orderCount = 0;
            orderHistory = new Order[MAX_ORDER_HISTORY];
            InitializeArrays();
        }

        // Метод инициализации массивов
        private void InitializeArrays()
        {// Заполняем весь массив null значениями
            for (int i = 0; i < MAX_ORDER_HISTORY; i++)
            {
                orderHistory[i] = null;
            }
        }

        // Оставить отзыв
        public void GetReview()
        {
            Console.WriteLine("\n=== ОСТАВИТЬ ОТЗЫВ ===");

            // Ввод данных отзыва
            string productName;// Название товара
            string comment;// Текст отзыва
            int rating;// Оценка (1-5)

            // Очистка буфера ввода
            Console.ReadLine();

            // Ввод названия товара
            Console.Write("Введите название товара: ");
            productName = Console.ReadLine();// Считывание названия товара

            // Ввод отзыва
            Console.Write("Введите ваш отзыв: ");
            comment = Console.ReadLine();// Считывание текста отзыва

            // Ввод оценки 
            while (true)
            {
                Console.Write("Введите оценку (1-5, где 5 - отлично): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out rating))// Преобразование ввода в число
                {
                    if (rating >= 1 && rating <= 5)// Проверка что оценка в допустимом диапазоне (1-5)
                    {
                        break; // Выходим из цикла при корректной оценке
                    }
                    Console.WriteLine("Ошибка! Оценка должна быть от 1 до 5.");
                }
                else
                {
                    Console.WriteLine("Ошибка! Введите число от 1 до 5.");
                }
            }

            // Создание и сохранение отзыва
            Review newReview = new Review(productName, clientId, rating, comment);
            newReview.SaveToFile();// Сохранение отзыва в файл

            // Вывод подтверждения
            Console.WriteLine("\n==================================");
            Console.WriteLine("ОТЗЫВ УСПЕШНО СОХРАНЕН!");
            Console.WriteLine($"Товар: {productName}");
            Console.WriteLine($"Ваша оценка: {rating}/5");
            Console.WriteLine("Спасибо за ваш отзыв!");
            Console.WriteLine("==================================");
        }

        // Метод добавления заказа в историю
        public void GetOrderHistory(Order order)
        {
            // Проверка на переполнение истории заказов
            if (orderCount >= MAX_ORDER_HISTORY)
            {
                Console.WriteLine("История заказов переполнена!");
                return;
            }

            // Добавляем заказ в историю если он не пустой
            if (order != null)
            {
                orderHistory[orderCount] = order; // Сохраняем указатель на заказ
                orderCount++; // Увеличиваем счетчик заказов
                Console.WriteLine($"Заказ №{order.GetNumberOrder()} добавлен в историю клиента!");
            }
        }

        // Свойства для доступа к полям
        public string ClientId// Свойство для получения идентификатора клиента
        {
            get { return clientId; }// Только чтение
        }

        public string ClientName // Свойство для получения имени клиента
        {
            get { return clientName; }
        }

        public int OrderCount // Свойство для получения количества заказов в истории
        {
            get { return orderCount; }
        }

        // Метод для получения идентификатора клиента
        public string GetClientId()
        {
            return clientId;
        }

        public string GetClientName()// Метод для получения имени клиента 
        {
            return clientName;
        }

        public int GetOrderCount()// Метод для получения количества заказов
        {
            return orderCount;
        }
    }
}
