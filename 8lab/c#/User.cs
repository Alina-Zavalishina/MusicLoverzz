using System;
using System.IO;
using System.Text;

namespace MusicStore
{
    public class User : ICloneable, IEquatable<User>
    {
        protected string id;
        protected string name;
        protected string email;
        protected string phone;
        protected string registrationDate;

        // Статический счетчик для генерации ID
        private static int counter = 0;

        // Конструктор по умолчанию
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

        // Конструктор с параметрами
        public User(string id, string name, string email = "", string phone = "")
        {
            this.id = string.IsNullOrEmpty(id) ? GenerateUserId("USR") : id;
            this.name = name;
            this.email = email;
            this.phone = phone;
            this.registrationDate = GetCurrentDateTime();
        }

        // Конструктор копирования
        public User(User other)
        {
            id = other.id + "_COPY";
            name = other.name;
            email = other.email;
            phone = other.phone;
            registrationDate = other.registrationDate;

            if (!id.Contains("_COPY"))
            {
                id += "_COPY";
            }
        }

        // Виртуальная функция для переопределения
        public virtual void DisplayInfo()
        {
            Console.WriteLine(this);
        }

        // Невиртуальная функция
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

            if (left is null || right is null)
                return false;

            return left.id == right.id && left.email == right.email;
        }

        public static bool operator !=(User left, User right)
        {
            return !(left == right);
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public bool Equals(User other)
        {
            return other != null && id == other.id && email == other.email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, email);
        }


        public virtual object Clone()
        {
            return new User(this);
        }

        // Геттеры
        public string Id => id;
        public string Name => name;
        public string Email => email;
        public string Phone => phone;
        public string RegistrationDate => registrationDate;

        // Сеттеры
        public void SetName(string newName) { name = newName; }
        public void SetEmail(string newEmail) { email = newEmail; }
        public void SetPhone(string newPhone) { phone = newPhone; }

        // Статический метод для генерации ID
        public static string GenerateUserId(string prefix)
        {
            counter++;
            return $"{prefix}{counter:D4}";
        }


        protected void InitializeUser(string id, string name, string email, string phone)
        {
            this.id = string.IsNullOrEmpty(id) ? GenerateUserId("USR") : id;
            this.name = name;
            this.email = email;
            this.phone = phone;
            this.registrationDate = GetCurrentDateTime();
        }


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


        public void PrintToConsole()
        {
            Console.WriteLine(this);
        }

        // Вспомогательный метод для получения текущей даты
        private string GetCurrentDateTime()
        {
            return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // Метод для тестирования оператора присваивания 
        public void AssignFrom(User other)
        {
            if (this != other)
            {
                id = other.id + "_ASSIGNED";
                name = other.name;
                email = other.email;
                phone = other.phone;
            }
        }
    }
}