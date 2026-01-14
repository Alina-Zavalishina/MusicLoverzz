using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class Admin : User
    {
        private string department;
        private string position;
        private List<string> permissions;
        private bool isSuperAdmin;

        // Конструкторы
        public Admin(string name, string department = "Администрация", string position = "Администратор")
            : base(GenerateUserId("ADM"), name, "", "")
        {
            this.department = department;
            this.position = position;
            this.isSuperAdmin = false;
            this.permissions = new List<string>();

            // Инициализация прав доступа
            permissions.Add("view_products");
            permissions.Add("view_orders");
            permissions.Add("edit_products");
        }

        public Admin(string id, string name, string email, string phone,
            string department, string position, bool isSuperAdmin = false)
            : base(id, name, email, phone)
        {
            InitializeAdminData(department, position, isSuperAdmin);
        }

        // 1. ПЕРЕОПРЕДЕЛЕНИЕ (override) метода базового класса
        public override void DisplayInfo()
        {
            // Сначала выводим базовую информацию через вызов базового метода
            Console.WriteLine("\n=== ИНФОРМАЦИЯ ОБ АДМИНИСТРАТОРЕ ===");
            ShowBasicInfo();  // ← ВЫЗОВ БАЗОВОГО МЕТОДА

            Console.WriteLine("\n--- СЛУЖЕБНАЯ ИНФОРМАЦИЯ ---");
            Console.WriteLine($"Отдел: {department}");
            Console.WriteLine($"Должность: {position}");
            Console.WriteLine($"Тип: {(isSuperAdmin ? "Супер-администратор" : "Обычный администратор")}");
            Console.WriteLine($"Количество прав доступа: {permissions.Count}");

            if (permissions.Count > 0)
            {
                Console.Write("Права доступа: ");
                for (int i = 0; i < permissions.Count; i++)
                {
                    Console.Write(permissions[i]);
                    if (i < permissions.Count - 1) Console.Write(", ");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Скидка: {CalculateDiscount()}%");
            Console.WriteLine("==================================");
        }

        // 2. ПЕРЕГРУЗКИ (overload) метода базового класса
        // С вызовом метода базового класса
        public void DisplayInfo(bool showTechnicalDetails)
        {
            Console.WriteLine("\n=== ТЕХНИЧЕСКАЯ ИНФОРМАЦИЯ АДМИНИСТРАТОРА ===");

            base.DisplayInfo();

            Console.WriteLine($"Отдел: {department}");
            Console.WriteLine($"Должность: {position}");

            if (showTechnicalDetails)
            {
                Console.WriteLine("\n--- ТЕХНИЧЕСКИЕ ДАННЫЕ ---");
                Console.WriteLine($"Супер-админ: {(isSuperAdmin ? "Да" : "Нет")}");
                Console.WriteLine($"Количество прав: {permissions.Count}");
                // В C# нет прямого аналога sizeof для управляемых объектов
                Console.WriteLine($"Размер объекта: (недоступно в C#)");
            }
            Console.WriteLine("==================================");
        }

        // Без вызова метода базового класса
        public void DisplayInfo(string reportFormat)
        {
            if (reportFormat == "short")
            {
                Console.WriteLine($"[КРАТКИЙ ОТЧЕТ] Админ: {GetName()} ({department}, {position})");
                Console.WriteLine($"Прав доступа: {permissions.Count}");
            }
            else if (reportFormat == "json")
            {
                Console.WriteLine("{");
                Console.WriteLine("  \"admin\": {");
                Console.WriteLine($"    \"id\": \"{GetId()}\",");
                Console.WriteLine($"    \"name\": \"{GetName()}\",");
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
                Console.WriteLine($"Имя: {GetName()}");
                Console.WriteLine($"Отдел: {department}");
                Console.WriteLine($"Должность: {position}");
                Console.WriteLine($"Тип: {(isSuperAdmin ? "Супер-администратор" : "Обычный администратор")}");
                Console.WriteLine($"Количество прав: {permissions.Count}");
            }
        }

        // 3. ПЕРЕОПРЕДЕЛЕНИЕ метода calculateDiscount
        public override double CalculateDiscount()
        {
            return 15.0;
        }

        // 4. ПЕРЕГРУЗКА метода calculateDiscount
        public double CalculateDiscount(bool applyBonus)
        {
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

        // Геттеры
        public string GetDepartment() => department;
        public string GetPosition() => position;
        public bool GetIsSuperAdmin() => isSuperAdmin;
        public List<string> GetPermissions() => permissions;

        // Сеттеры
        public void SetDepartment(string newDept) => department = newDept;
        public void SetPosition(string newPos) => position = newPos;
        public void SetIsSuperAdmin(bool superAdmin) => isSuperAdmin = superAdmin;

        private void InitializeAdminData(string department, string position, bool isSuperAdmin)
        {
            this.department = department;
            this.position = position;
            this.isSuperAdmin = isSuperAdmin;
            this.permissions = new List<string>();

            if (isSuperAdmin)
            {
                permissions.AddRange(new string[] {
                    "view_products", "edit_products", "delete_products",
                    "view_orders", "manage_orders", "view_users",
                    "manage_users", "system_settings"
                });
            }
            else
            {
                permissions.AddRange(new string[] {
                    "view_products", "edit_products", "view_orders", "manage_orders"
                });
            }
        }

        // Переопределение ToString
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(base.ToString());

            sb.AppendLine("\n--- ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ (Admin) ---");
            sb.AppendLine($"Отдел: {department}");
            sb.AppendLine($"Должность: {position}");
            sb.AppendLine($"Тип: {(isSuperAdmin ? "Супер-администратор" : "Обычный администратор")}");
            sb.AppendLine($"Количество прав: {permissions.Count}");
            sb.AppendLine($"Скидка: {CalculateDiscount()}%");
            sb.AppendLine("==================================");

            return sb.ToString();
        }
    }
}