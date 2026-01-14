using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusicStore
{
    public class SalesReport : ICloneable
    {
        private const int MAX_TOP_PRODUCTS = 10;       // максимальное количество товаров в топе продаж
        private const int MAX_ANALYSIS_ITEMS = 20;     // максимальное количество пунктов анализа

        private string period;                         // период отчета
        private double money;                          // общая выручка за период
        private int soldProducts;                      // общее количество проданных товаров

        private string[] topSales;                     // массив самых продаваемых товаров
        private int topSalesCount;                     // текущее количество товаров в топе

        private string[] analysis;                     // массив аналитических записей
        private int analysisCount;                     // текущее количество записей анализа

        public SalesReport()
        {
            period = GetCurrentDate();
            money = 0.0;
            soldProducts = 0;
            topSalesCount = 0;
            analysisCount = 0;
            AllocateMemory();
        }

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
        protected SalesReport(SalesReport other, bool deepCopy)
        {
            period = other.period;
            money = other.money;
            soldProducts = other.soldProducts;
            topSalesCount = other.topSalesCount;
            analysisCount = other.analysisCount;

            AllocateMemory();

            if (deepCopy)
            {
                // Глубокое копирование
                for (int i = 0; i < other.topSalesCount; i++)
                    topSales[i] = other.topSales[i];

                for (int i = 0; i < other.analysisCount; i++)
                    analysis[i] = other.analysis[i];
            }
            else
            {
                // Поверхностное копирование (используем те же массивы)
                topSales = other.topSales;
                analysis = other.analysis;
            }
        }

        // Методы клонирования
        public SalesReport ShallowClone()
        {
            return new SalesReport(this, false)
            {
                period = this.period + " (shallow clone)"
            };
        }

        public SalesReport DeepClone()
        {
            return new SalesReport(this, true)
            {
                period = this.period + " (deep clone)"
            };
        }

        // Реализация ICloneable
        public object Clone()
        {
            return DeepClone(); // по умолчанию глубокое клонирование
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
            string filename = $"sales_report_{period}.txt";

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

        public void SaveToFile()
        {
            string filename = $"sales_report_{period}.txt";
            try
            {
                using (StreamWriter file = new StreamWriter(filename))
                {
                    file.WriteLine(period);
                    file.WriteLine(money);
                    file.WriteLine(soldProducts);
                    file.WriteLine(topSalesCount);

                    for (int i = 0; i < topSalesCount; i++)
                        file.WriteLine(topSales[i]);

                    file.WriteLine(analysisCount);

                    for (int i = 0; i < analysisCount; i++)
                        file.WriteLine(analysis[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения отчета: {ex.Message}");
            }
        }

        public bool LoadFromFile(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    return false;

                using (StreamReader file = new StreamReader(filename))
                {
                    period = file.ReadLine();
                    money = double.Parse(file.ReadLine());
                    soldProducts = int.Parse(file.ReadLine());
                    topSalesCount = int.Parse(file.ReadLine());

                    for (int i = 0; i < topSalesCount; i++)
                        topSales[i] = file.ReadLine();

                    analysisCount = int.Parse(file.ReadLine());

                    for (int i = 0; i < analysisCount; i++)
                        analysis[i] = file.ReadLine();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Методы для демонстрации клонирования
        public void DisplayReportInfo()
        {
            Console.WriteLine("\n=== ИНФОРМАЦИЯ ОБ ОТЧЕТЕ ===");
            Console.WriteLine($"Период: {period}");
            Console.WriteLine($"Выручка: {money:F2} руб.");
            Console.WriteLine($"Проданные товары: {soldProducts} шт.");
            Console.WriteLine($"Топ товаров: {topSalesCount}");
            Console.WriteLine($"Записей анализа: {analysisCount}");
        }

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
                    Console.WriteLine($" {i + 1}. {topSales[i]}");
            }

            if (analysisCount > 0)
            {
                Console.WriteLine("\nАналитика продаж:");
                for (int i = 0; i < analysisCount; i++)
                    Console.WriteLine($"- {analysis[i]}");
            }

            Console.WriteLine("==========================");
        }

        // Вспомогательные методы
        private void InputReportData()
        {
            // Ввод выручки
            while (true)
            {
                Console.Write("Введите выручку: ");
                if (double.TryParse(Console.ReadLine(), out money) && money >= 0)
                    break;
                Console.WriteLine("Ошибка! Введите положительное число.");
            }

            // Ввод количества проданных товаров
            while (true)
            {
                Console.Write("Введите количество проданных товаров: ");
                if (int.TryParse(Console.ReadLine(), out soldProducts) && soldProducts >= 0)
                    break;
                Console.WriteLine("Ошибка! Введите целое положительное число.");
            }

            // Ввод топа товаров
            Console.WriteLine("\n=== ТОП ТОВАРОВ ПО ПРОДАЖАМ ===");
            topSalesCount = 0;

            for (int i = 0; i < MAX_TOP_PRODUCTS; i++)
            {
                Console.Write($"Введите товар №{i + 1} (или 'stop' для завершения): ");
                string product = Console.ReadLine();

                if (product.ToLower() == "stop" || string.IsNullOrEmpty(product))
                    break;

                topSales[topSalesCount++] = product;
            }

            // Ввод аналитики
            Console.WriteLine("\n=== АНАЛИТИКА ПРОДАЖ ===");
            analysisCount = 0;

            for (int i = 0; i < MAX_ANALYSIS_ITEMS; i++)
            {
                Console.Write($"Введите пункт аналитики №{i + 1} (или 'stop' для завершения): ");
                string analysisItem = Console.ReadLine();

                if (analysisItem.ToLower() == "stop" || string.IsNullOrEmpty(analysisItem))
                    break;

                analysis[analysisCount++] = analysisItem;
            }
        }

        private void AllocateMemory()
        {
            topSales = new string[MAX_TOP_PRODUCTS];
            analysis = new string[MAX_ANALYSIS_ITEMS];
        }

        private string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy");
        }

        // Геттеры
        public string Period => period;
        public double Money => money;
        public int SoldProducts => soldProducts;
        public int TopSalesCount => topSalesCount;
        public int AnalysisCount => analysisCount;
    }
}