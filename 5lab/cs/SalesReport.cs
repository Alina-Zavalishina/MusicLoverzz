using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace MusicShopSystem
{
    public class SalesReport
    {
        // Константы
        private const int MAX_TOP_PRODUCTS = 10;        // максимальное количество товаров в топе продаж
        private const int MAX_ANALYSIS_ITEMS = 20;      // максимальное количество пунктов анализа

        // Поля класса
        private string period;                          // период отчета
        private double money;                           // общая выручка за период
        private int soldProducts;                       // общее количество проданных товаров
        private string[] topSales;                      // массив самых продаваемых товаров
        private int topSalesCount;                      // текущее количество товаров в топе
        private string[] analysis;                      // массив аналитических записей
        private int analysisCount;                      // текущее количество записей анализа

        // Конструктор по умолчанию
        public SalesReport()
        {
            money = 0.0;
            soldProducts = 0;
            topSalesCount = 0;
            analysisCount = 0;
            period = GetCurrentDate();
            topSales = new string[MAX_TOP_PRODUCTS];
            analysis = new string[MAX_ANALYSIS_ITEMS];
        }

        // Конструктор с параметрами
        public SalesReport(string reportPeriod)
        {
            period = reportPeriod;
            money = 0.0;
            soldProducts = 0;
            topSalesCount = 0;
            analysisCount = 0;
            topSales = new string[MAX_TOP_PRODUCTS];
            analysis = new string[MAX_ANALYSIS_ITEMS];
        }

        // Метод создания нового отчета
        public void GetReport()
        {
            Console.WriteLine("\n=== ОФОРМЛЕНИЕ ОТЧЕТА ПО ПРОДАЖАМ ===");
            Console.WriteLine($"Период отчета: {period}");
            InputReportData();
            SaveToFile();
            Console.WriteLine("Отчет успешно оформлен и сохранен!");
        }

        // Метод пересчета существующего отчета
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

        // Метод ввода данных для отчета
        private void InputReportData()
        {
            // Ввод выручки
            while (true)
            {
                Console.Write("Введите выручку: ");
                if (double.TryParse(Console.ReadLine(), out double inputMoney) && inputMoney >= 0)
                {
                    money = inputMoney;
                    break;
                }
                Console.WriteLine("Ошибка! Введите положительное число.");
            }

            // Ввод количества проданных товаров
            while (true)
            {
                Console.Write("Введите количество проданных товаров: ");
                if (int.TryParse(Console.ReadLine(), out int inputSoldProducts) && inputSoldProducts >= 0)
                {
                    soldProducts = inputSoldProducts;//сохранение количества
                    break;
                }
                Console.WriteLine("Ошибка! Введите целое положительное число.");
            }

            Console.ReadLine(); // Очистка буфера

            // Ввод топа товаров
            Console.WriteLine("\n=== ТОП ТОВАРОВ ПО ПРОДАЖАМ ===");
            topSalesCount = 0;// Сброс счетчика топ товаров

            for (int i = 0; i < MAX_TOP_PRODUCTS; i++)
            {
                Console.Write($"Введите товар №{i + 1} (или 'stop' для завершения): ");
                string product = Console.ReadLine();//чтение название товаров

                if (product.ToLower() == "stop" || string.IsNullOrEmpty(product))
                {
                    break;
                }

                topSales[topSalesCount++] = product;// Добавление товара в массив и увеличение счетчика
            }

            // Ввод аналитики
            Console.WriteLine("\n=== АНАЛИТИКА ПРОДАЖ ===");
            analysisCount = 0;// Сброс счетчика аналитических записей

            for (int i = 0; i < MAX_ANALYSIS_ITEMS; i++)
            {
                Console.Write($"Введите пункт аналитики №{i + 1} (или 'stop' для завершения): ");
                string analysisItem = Console.ReadLine();

                if (analysisItem.ToLower() == "stop" || string.IsNullOrEmpty(analysisItem))
                {
                    break;
                }

                analysis[analysisCount++] = analysisItem;// Добавление записи в массив и увеличение счетчика
            }
        }

        // Метод сохранения отчета в файл
        public void SaveToFile()
        {
            string filename = "sales_report_" + period + ".txt"; // Формирование имени файла

            try
            { // Создание файла для записи (перезаписывает существующий)
                using (StreamWriter file = new StreamWriter(filename))
                {
                    file.WriteLine(period);// Период отчета
                    file.WriteLine(money.ToString(CultureInfo.InvariantCulture)); // Выручка
                    file.WriteLine(soldProducts); // Количество проданных товаров
                    file.WriteLine(topSalesCount);// Количество позиций в топе
                    // Запись топ товаров
                    for (int i = 0; i < topSalesCount; i++)
                    {
                        file.WriteLine(topSales[i]);
                    }

                    file.WriteLine(analysisCount);// Количество аналитических записей

                    for (int i = 0; i < analysisCount; i++)
                    {
                        file.WriteLine(analysis[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения отчета: {ex.Message}");
                throw;
            }
        }

        // Метод загрузки отчета из файла
        public bool LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }

            try
            {
                using (StreamReader file = new StreamReader(filename))
                {
                    period = file.ReadLine();

                    if (!double.TryParse(file.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out money))
                        return false;// Чтение выручки 

                    if (!int.TryParse(file.ReadLine(), out soldProducts))
                        return false;// Чтение количества проданных товаров

                    if (!int.TryParse(file.ReadLine(), out topSalesCount))
                        return false;// Чтение количества позиций в топе

                    for (int i = 0; i < topSalesCount; i++) // Чтение топ товаров
                    {
                        topSales[i] = file.ReadLine();
                    }

                    if (!int.TryParse(file.ReadLine(), out analysisCount))// Чтение количества аналитических записей
                        return false;

                    for (int i = 0; i < analysisCount; i++)// Чтение аналитических записей
                    {
                        analysis[i] = file.ReadLine();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Метод вывода отчета на экран
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

        // Метод получения текущей даты
        private string GetCurrentDate()
        {
            return DateTime.Now.ToString("dd.MM.yyyy");
        }

        // Свойства для доступа к полям
        public string Period
        {
            get { return period; }
        }

        public double Money
        {
            get { return money; }
        }

        public int SoldProducts
        {
            get { return soldProducts; }
        }


        public string GetPeriod()
        {
            return period;
        }

        public double GetMoney()
        {
            return money;
        }

        public int GetSoldProducts()
        {
            return soldProducts;
        }
    }
}
