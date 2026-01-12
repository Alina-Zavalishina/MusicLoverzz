using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicStore
{
    public class Admin : User
    {
        // Поля класса
        private string department;
        private string position;
        private List<string> permissions;
        private bool isSuperAdmin;

        // Конструкторы с вызовом конструктора базового класса
        public Admin(string name, string department = "Администрация", string position = "Администратор")
            : base(GenerateUserId("ADM"), name, "", "")  // Вызов конструктора базового класса
        {
            InitializeAdminData(department, position, false);
        }

        public Admin(string id, string name, string email, string phone,
                    string department, string position, bool isSuperAdmin = false)
            : base(id, name, email, phone)  // Вызов конструктора базового класса
        {
            InitializeAdminData(department, position, isSuperAdmin);
        }

        //ПЕРЕГРУЗКА МЕТОДОВ БАЗОВОГО КЛАССА

        // 1. ПЕРЕОПРЕДЕЛЕНИЕ метода DisplayInfo С ВЫЗОВОМ базового метода
        public override void DisplayInfo()
        {
            Console.WriteLine("\n=== ИНФОРМАЦИЯ ОБ АДМИНИСТРАТОРЕ ===");

            
            base.DisplayInfo();  

            Console.WriteLine("\n--- СЛУЖЕБНАЯ ИНФОРМАЦИЯ ---");
            Console.WriteLine($"Отдел: {department}");
            Console.WriteLine($"Должность: {position}");
            Console.WriteLine($"Тип: {(isSuperAdmin ? "Супер-администратор" : "Обычный администратор")}");
            Console.WriteLine($"Количество прав доступа: {permissions.Count}");

            if (permissions.Count > 0)
            {
                Console.WriteLine($"Права доступа: {string.Join(", ", permissions)}");
            }
            Console.WriteLine($"Скидка: {CalculateDiscount():F1}%");
            Console.WriteLine("==================================");
        }

        // 2. ПЕРЕГРУЗКА DisplayInfo с параметром bool - С ВЫЗОВОМ БАЗОВОГО МЕТОДА
        public void DisplayInfo(bool showTechnicalDetails)
        {
            Console.WriteLine("\n=== ТЕХНИЧЕСКАЯ ИНФОРМАЦИЯ АДМИНИСТРАТОРА ===");

            // ВЫЗОВ БАЗОВОГО МЕТОДА (задание 6)
            base.DisplayInfo();  // Базовый метод выводит ToString()

            Console.WriteLine($"Отдел: {department}");
            Console.WriteLine($"Должность: {position}");

            if (showTechnicalDetails)
            {
                Console.WriteLine("\n--- ТЕХНИЧЕСКИЕ ДАННЫЕ ---");
                Console.WriteLine($"Супер-админ: {(isSuperAdmin ? "Да" : "Нет")}");
                Console.WriteLine($"Количество прав: {permissions.Count}");
                Console.WriteLine($"Размер объекта: приблизительно {System.Runtime.InteropServices.Marshal.SizeOf(this) + permissions.Sum(p => p.Length * 2)} байт");
            }
            Console.WriteLine("==================================");
        }

        // 3. ПЕРЕГРУЗКА DisplayInfo БЕЗ ВЫЗОВА БАЗОВОГО МЕТОДА
        public void DisplayInfo(string reportFormat)
        {
            

            if (reportFormat == "short")
            {
                Console.WriteLine($"[КРАТКИЙ ОТЧЕТ] Админ: {Name} ({department}, {position})");
                Console.WriteLine($"Прав доступа: {permissions.Count}");
            }
            else if (reportFormat == "json")
            {
                Console.WriteLine("{");
                Console.WriteLine("  \"admin\": {");
                Console.WriteLine($"    \"id\": \"{Id}\",");
                Console.WriteLine($"    \"name\": \"{Name}\",");
                Console.WriteLine($"    \"department\": \"{department}\",");
                Console.WriteLine($"    \"position\": \"{position}\",");
                Console.WriteLine($"    \"is_super_admin\": {(isSuperAdmin ? "true" : "false")},");
                Console.WriteLine($"    \"permissions_count\": {permissions.Count}");
                Console.WriteLine("  }");
                Console.WriteLine("}");
            }
            else
            {
                Console.WriteLine("=== ОТЧЕТ ДЛЯ АДМИНИСТРАТОРА ===");
                Console.WriteLine($"Имя: {Name}");
                Console.WriteLine($"Отдел: {department}");
                Console.WriteLine($"Должность: {position}");
                Console.WriteLine($"Тип: {(isSuperAdmin ? "Супер-администратор" : "Обычный администратор")}");
                Console.WriteLine($"Количество прав: {permissions.Count}");
            }
        }

        // 4. ПЕРЕОПРЕДЕЛЕНИЕ метода CalculateDiscount БЕЗ вызова базового метода
        public override double CalculateDiscount()
        {
            
            return 15.0;
        }

        // 5. ПЕРЕГРУЗКА метода CalculateDiscount с параметром bool
        public double CalculateDiscount(bool applyBonus)
        {
            // ПЕРЕГРУЗКА метода (задание 6)

            if (applyBonus)
            {
                if (isSuperAdmin)
                {
                    return 25.0;
                }
                else
                {
                    return 20.0;
                }
            }
            return 15.0;
        }

        // 6. Перегрузка оператора присваивания
        public Admin AssignFromUser(User user)
        {
            if (!ReferenceEquals(this, user))
            {
                // Копируем данные из базового класса
                CopyPropertiesFromUser(user);

                // Устанавливаем значения по умолчанию для специфичных полей Admin
                department = "Администрация (присвоено)";
                position = "Администратор (присвоено)";
                isSuperAdmin = false;

                // Очищаем права доступа и добавляем базовые
                permissions.Clear();
                permissions.Add("assigned_from_user");
            }
            return this;
        }

        // Перегрузка для присваивания от другого Admin
        public Admin AssignFromAdmin(Admin admin)
        {
            if (!ReferenceEquals(this, admin))
            {
                // Копируем данные из базового класса
                CopyPropertiesFromUser(admin);

                // Копируем специфичные поля Admin
                department = admin.department + " (копия)";
                position = admin.position + " (копия)";
                isSuperAdmin = admin.isSuperAdmin;
                permissions = new List<string>(admin.permissions);
            }
            return this;
        }


        // Методы администратора
        public void AddPermission(string permission)
        {
            if (!HasPermission(permission))
            {
                permissions.Add(permission);
                Console.WriteLine($"Право доступа добавлено: {permission}");
            }
        }

        public void RemovePermission(string permission)
        {
            if (permissions.Remove(permission))
            {
                Console.WriteLine($"Право доступа удалено: {permission}");
            }
        }

        public bool HasPermission(string permission)
        {
            return permissions.Contains(permission);
        }

        public void ShowPermissions()
        {
            if (permissions.Count == 0)
            {
                Console.WriteLine("Нет прав доступа!");
                return;
            }

            Console.WriteLine("\n=== ПРАВА ДОСТУПА ===");
            for (int i = 0; i < permissions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {permissions[i]}");
            }
            Console.WriteLine("=====================");
        }

        private void InitializeAdminData(string department, string position, bool isSuperAdmin)
        {
            this.department = department;
            this.position = position;
            this.isSuperAdmin = isSuperAdmin;
            permissions = new List<string>();

            if (isSuperAdmin)
            {
                permissions.AddRange(new[]
                {
                    "view_products", "edit_products", "delete_products",
                    "view_orders", "manage_orders", "view_users",
                    "manage_users", "system_settings"
                });
            }
            else
            {
                permissions.AddRange(new[]
                {
                    "view_products", "edit_products",
                    "view_orders", "manage_orders"
                });
            }
        }

        private void CopyPropertiesFromUser(User user)
        {
            // Используем отражение для копирования protected полей
            Type userType = typeof(User);
            var fields = userType.GetFields(System.Reflection.BindingFlags.NonPublic |
                                           System.Reflection.BindingFlags.Instance);

            foreach (var field in fields)
            {
                object value = field.GetValue(user);
                field.SetValue(this, value);
            }
        }

        // Геттеры
        public string Department => department;
        public string Position => position;
        public bool IsSuperAdmin => isSuperAdmin;
        public List<string> Permissions => new List<string>(permissions); // Возвращаем копию

        // Сеттеры
        public void SetDepartment(string newDept) => department = newDept;
        public void SetPosition(string newPos) => position = newPos;
        public void SetIsSuperAdmin(bool superAdmin) => isSuperAdmin = superAdmin;

        public override string ToString()
        {
            return base.ToString() +
                   $"\n--- ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ (Admin) ---\n" +
                   $"Отдел: {department}\n" +
                   $"Должность: {position}\n" +
                   $"Тип: {(isSuperAdmin ? "Супер-администратор" : "Обычный администратор")}\n" +
                   $"Количество прав: {permissions.Count}\n" +
                   $"Скидка: {CalculateDiscount():F1}%\n" +
                   "==================================\n";
        }
    }
}