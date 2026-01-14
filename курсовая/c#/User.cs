using System;
using System.IO;
using System.Text;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class User
    {
        protected string id;
        protected string name;
        protected string email;
        protected string phone;
        protected string registrationDate;

        // Статический счетчик для генерации ID
        private static int counter = 0;

        // Конструкторы
        public User()
        {
            id = "";
            name = "";
            email = "";
            phone = "";
            registrationDate = "";

            if (string.IsNullOrEmpty(id))
            {
                id = GenerateUserId("USR");
            }
        }

        public User(string id, string name, string email = "", string phone = "")
        {
            this.id = string.IsNullOrEmpty(id) ? GenerateUserId("USR") : id;
            this.name = name;
            this.email = email;
            this.phone = phone;
            this.registrationDate = GetCurrentDate();
        }

        // Конструктор копирования
        public User(User other)
        {
            this.id = other.id + "_COPY";
            this.name = other.name;
            this.email = other.email;
            this.phone = other.phone;
            this.registrationDate = other.registrationDate;

            if (!id.Contains("_COPY"))
            {
                id += "_COPY";
            }
        }

        // Виртуальная функция для переопределения
        public virtual void DisplayInfo()
        {
            Console.WriteLine(this.ToString());
        }

        public void ShowBasicInfo()
        {
            Console.WriteLine("\n=== БАЗОВАЯ ИНФОРМАЦИЯ ===");
            Console.WriteLine($"ID: {id}");
            Console.WriteLine($"Имя: {name}");
            Console.WriteLine($"Email: {(string.IsNullOrEmpty(email) ? "не указан" : email)}");
            Console.WriteLine($"Телефон: {(string.IsNullOrEmpty(phone) ? "не указан" : phone)}");
            Console.WriteLine($"Дата регистрации: {registrationDate}");
            Console.WriteLine("================================");
        }

        // Виртуальный метод для расчета скидки
        public virtual double CalculateDiscount()
        {
            return 0.0;
        }

        // Перегрузка операторов
        public static bool operator ==(User left, User right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.id == right.id && left.email == right.email;
        }

        public static bool operator !=(User left, User right)
        {
            return !(left == right);
        }

        // Метод Equals
        public override bool Equals(object obj)
        {
            if (obj is User other)
                return this == other;
            return false;
        }

        public override int GetHashCode()
        {
            return (id + email).GetHashCode();
        }

        // Геттеры
        public string GetId() => id;
        public string GetName() => name;
        public string GetEmail() => email;
        public string GetPhone() => phone;
        public string GetRegistrationDate() => registrationDate;

        // Сеттеры
        public void SetName(string newName) => name = newName;
        public void SetEmail(string newEmail) => email = newEmail;
        public void SetPhone(string newPhone) => phone = newPhone;

        // Статический метод для генерации ID
        public static string GenerateUserId(string prefix)
        {
            counter++;
            return $"{prefix}{counter:D4}";
        }

        // Protected метод для инициализации
        protected void InitializeUser(string id, string name, string email, string phone)
        {
            this.id = string.IsNullOrEmpty(id) ? GenerateUserId("USR") : id;
            this.name = name;
            this.email = email;
            this.phone = phone;
            this.registrationDate = GetCurrentDate();
        }

        // Метод для получения текущей даты
        private static string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // Переопределение метода ToString
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("\n=== ИНФОРМАЦИЯ О ПОЛЬЗОВАТЕЛЕ ===");
            sb.AppendLine($"ID: {id}");
            sb.AppendLine($"Имя: {name}");
            sb.AppendLine($"Email: {(string.IsNullOrEmpty(email) ? "не указан" : email)}");
            sb.AppendLine($"Телефон: {(string.IsNullOrEmpty(phone) ? "не указан" : phone)}");
            sb.AppendLine($"Дата регистрации: {registrationDate}");
            sb.AppendLine("Тип: Базовый пользователь");
            sb.AppendLine("================================");
            return sb.ToString();
        }
    }
}