using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Курсовая_работа_Музыкальный_магазин_с_
{
    public class SalesReport
    {
        private const int MAX_TOP_PRODUCTS = 10;      // максимальное количество товаров в топе продаж
        private const int MAX_ANALYSIS_ITEMS = 20;    // максимальное количество пунктов анализа

        private string period;                        // период отчета
        private double money;                         // общая выручка за период
        private int soldProducts;                     // общее количество проданных товаров

        private string[] topSales;                    // массив самых продаваемых товаров
        private int topSalesCount;                    // текущее количество товаров в топе

        private string[] analysis;                    // массив аналитических записей
        private int analysisCount;                    // текущее количество записей анализа

        // Конструктор по умолчанию
        public SalesReport()
        {
            money = 0.0;
            soldProducts = 0;
            topSalesCount = 0;
            analysisCount = 0;
            period = GetCurrentDate();
            AllocateMemory();
        }

        // Конструктор с параметрами
        public SalesReport(string reportPeriod)
        {
            period = reportPeriod;
            money = 0.0;
            soldProducts = 0;
            topSalesCount = 0;
            analysisCount = 0;
            AllocateMemory();
        }

        // Конструктор копирования (глубокое копирование)
        public SalesReport(SalesReport other)
        {
            period = other.period;
            money = other.money;
            soldProducts = other.soldProducts;
            topSalesCount = other.topSalesCount;
            analysisCount = other.analysisCount;

            // Выделяем новую память
            AllocateMemory();

            // Копируем содержимое массивов
            CopyArrays(other);
        }

        // Метод для поверхностного клонирования
        public SalesReport ShallowClone()
        {
            SalesReport clone = new SalesReport();

            // Копируем простые поля
            clone.period = this.period + " (shallow clone)";
            clone.money = this.money;
            clone.soldProducts = this.soldProducts;
            clone.topSalesCount = this.topSalesCount;
            clone.analysisCount = this.analysisCount;

            // ПОВЕРХНОСТНОЕ КОПИРОВАНИЕ: копируем ссылки на массивы
            clone.topSales = this.topSales;
            clone.analysis = this.analysis;

            return clone;
        }

        // Метод для глубокого клонирования
        public SalesReport DeepClone()
        {
            SalesReport clone = new SalesReport();

            // Копируем простые поля
            clone.period = this.period + " (deep clone)";
            clone.money = this.money;
            clone.soldProducts = this.soldProducts;
            clone.topSalesCount = this.topSalesCount;
            clone.analysisCount = this.analysisCount;

            // Глубокое копирование: копируем содержимое массивов
            for (int i = 0; i < topSalesCount; i++)
            {
                clone.topSales[i] = this.topSales[i];
            }

            for (int i = 0; i < analysisCount; i++)
            {
                clone.analysis[i] = this.analysis[i];
            }

            return clone;
        }

        // Вывод информации о отчете
        public void DisplayReportInfo()
        {
            Console.WriteLine("\n=== ИНФОРМАЦИЯ ОБ ОТЧЕТЕ ===");
            Console.WriteLine($"Период: {period}");
            Console.WriteLine($"Выручка: {money} руб.");
            Console.WriteLine($"Проданные товары: {soldProducts} шт.");
            Console.WriteLine($"Топ товаров: {topSalesCount}");
            Console.WriteLine($"Записей анализа: {analysisCount}");
        }

        // Модификация для демонстрации
        public void ModifyReportForDemo()
        {
            Console.WriteLine("\n=== МОДИФИКАЦИЯ ОТЧЕТА ДЛЯ ДЕМОНСТРАЦИИ ===");

            if (topSalesCount > 0)
            {
                topSales[0] = "МОДИФИЦИРОВАННЫЙ ТОВАР";
                Console.WriteLine($"Первый товар в топе изменен на: {topSales[0]}");
            }

            money += 1000.0;
            Console.WriteLine("Выручка увеличена на 1000 руб.");
        }

        // Выделение памяти для массивов
        private void AllocateMemory()
        {
            topSales = new string[MAX_TOP_PRODUCTS];
            analysis = new string[MAX_ANALYSIS_ITEMS];
        }

        // Копирование массивов (глубокое копирование)
        private void CopyArrays(SalesReport other)
        {
            // Копируем топ товаров
            for (int i = 0; i < other.topSalesCount; i++)
            {
                topSales[i] = other.topSales[i];
            }

            // Копируем анализ
            for (int i = 0; i < other.analysisCount; i++)
            {
                analysis[i] = other.analysis[i];
            }
        }

        // Основные методы
        public void GetReport()
        {
            Console.WriteLine("\n=== ОФОРМЛЕНИЕ ОТЧЕТА ПО ПРОДАЖАМ ===");
            Console.WriteLine($"Период отчета: {period}");
            InputReportData();
            SaveToFile();
            Console.WriteLine("Отчет успешно оформлен и сохранен!");
        }

        public void RemakeReport()
        {
            Console.WriteLine("\n=== ИЗМЕНЕНИЕ ОТЧЕТА ПО ПРОДАЖАМ ===");
            string filename = "sales_report_" + period + ".txt";

            if (!LoadFromFile(filename))
            {
                Console.WriteLine($"Отчет за период '{period}' не найден!");
                return;
            }

            Console.WriteLine("Текущие данные отчета:");
            DisplayReport();
            Console.WriteLine("\nВведите новые данные:");
            InputReportData();
            SaveToFile();
            Console.WriteLine("Отчет успешно изменен!");
        }

        private void InputReportData()
        {
            // Ввод выручки
            while (true)
            {
                Console.Write("Введите выручку: ");
                if (double.TryParse(Console.ReadLine(), out money) && money >= 0)
                {
                    break;
                }
                Console.WriteLine("Ошибка! Введите положительное число.");
            }

            // Ввод количества проданных товаров
            while (true)
            {
                Console.Write("Введите количество проданных товаров: ");
                if (int.TryParse(Console.ReadLine(), out soldProducts) && soldProducts >= 0)
                {
                    break;
                }
                Console.WriteLine("Ошибка! Введите целое положительное число.");
            }

            // Ввод топа товаров
            Console.WriteLine("\n=== ТОП ТОВАРОВ ПО ПРОДАЖАМ ===");
            topSalesCount = 0;

            for (int i = 0; i < MAX_TOP_PRODUCTS; i++)
            {
                Console.Write($"Введите товар №{i + 1} (или 'stop' для завершения): ");
                string product = Console.ReadLine();

                if (product == "stop" || string.IsNullOrEmpty(product))
                {
                    break;
                }

                topSales[topSalesCount++] = product;
            }

            // Ввод аналитики
            Console.WriteLine("\n=== АНАЛИТИКА ПРОДАЖ ===");
            analysisCount = 0;

            for (int i = 0; i < MAX_ANALYSIS_ITEMS; i++)
            {
                Console.Write($"Введите пункт аналитики №{i + 1} (или 'stop' для завершения): ");
                string analysisItem = Console.ReadLine();

                if (analysisItem == "stop" || string.IsNullOrEmpty(analysisItem))
                {
                    break;
                }

                analysis[analysisCount++] = analysisItem;
            }
        }

        public void SaveToFile()
        {
            string filename = "sales_report_" + period + ".txt";

            using (StreamWriter file = new StreamWriter(filename))
            {
                file.WriteLine(period);
                file.WriteLine(money);
                file.WriteLine(soldProducts);
                file.WriteLine(topSalesCount);

                for (int i = 0; i < topSalesCount; i++)
                {
                    file.WriteLine(topSales[i]);
                }

                file.WriteLine(analysisCount);

                for (int i = 0; i < analysisCount; i++)
                {
                    file.WriteLine(analysis[i]);
                }
            }
        }

        public bool LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }

            string[] lines = File.ReadAllLines(filename);
            if (lines.Length < 5) return false;

            period = lines[0];
            money = double.Parse(lines[1]);
            soldProducts = int.Parse(lines[2]);
            topSalesCount = int.Parse(lines[3]);

            int index = 4;
            for (int i = 0; i < topSalesCount; i++)
            {
                if (index < lines.Length)
                {
                    topSales[i] = lines[index++];
                }
            }

            if (index < lines.Length)
            {
                analysisCount = int.Parse(lines[index++]);
                for (int i = 0; i < analysisCount; i++)
                {
                    if (index < lines.Length)
                    {
                        analysis[i] = lines[index++];
                    }
                }
            }

            return true;
        }

        public void DisplayReport()
        {
            Console.WriteLine("\n=== ОТЧЕТ ПО ПРОДАЖАМ ===");
            Console.WriteLine($"Период: {period}");
            Console.WriteLine($"Выручка: {money:F2} руб.");
            Console.WriteLine($"Проданные товары: {soldProducts} шт.");

            if (topSalesCount > 0)
            {
                Console.WriteLine("\nТоп товаров по продажам:");
                for (int i = 0; i < topSalesCount; i++)
                {
                    Console.WriteLine($"{i + 1}. {topSales[i]}");
                }
            }

            if (analysisCount > 0)
            {
                Console.WriteLine("\nАналитика продаж:");
                for (int i = 0; i < analysisCount; i++)
                {
                    Console.WriteLine($"- {analysis[i]}");
                }
            }

            Console.WriteLine("==========================");
        }

        private string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy");
        }

        // Геттеры
        public string GetPeriod() { return period; }
        public double GetMoney() { return money; }
        public int GetSoldProducts() { return soldProducts; }
        public int GetTopSalesCount() { return topSalesCount; }
        public int GetAnalysisCount() { return analysisCount; }
    }
}