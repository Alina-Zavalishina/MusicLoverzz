#ifndef SALESREPORT_HPP
#define SALESREPORT_HPP

#include <string>
#include <iostream>
#include <fstream>
#include <ctime>

class SalesReport {
private:
    static const int MAX_TOP_PRODUCTS = 10;      // максимальное количество товаров в топе продаж
    static const int MAX_ANALYSIS_ITEMS = 20;    // максимальное количество пунктов анализа

    std::string period;                          // период отчета
    double money;                               // общая выручка за период
    int soldProducts;                           // общее количество проданных товаров

    std::string* topSales;                      // динамический массив самых продаваемых товаров
    int topSalesCount;                          // текущее количество товаров в топе

    std::string* analysis;                      // динамический массив аналитических записей
    int analysisCount;                          // текущее количество записей анализа

public:
    // Конструкторы
    SalesReport();
    SalesReport(const std::string& reportPeriod);

    // Конструктор копирования (глубокое копирование)
    SalesReport(const SalesReport& other);

    // Деструктор
    ~SalesReport();

    // Оператор присваивания (глубокое копирование)
    SalesReport& operator=(const SalesReport& other);

    // Методы клонирования
    SalesReport* shallowClone() const;          // Поверхностное клонирование
    SalesReport* deepClone() const;             // Глубокое клонирование

    // Основные методы
    void GetReport();                           // создание нового отчета
    void RemakeReport();                        // пересчет существующего отчета
    void SaveToFile();                          // сохранение отчета в файл
    bool LoadFromFile(const std::string& filename); // загрузка отчета из файла

    // Геттеры
    std::string getPeriod() const { return period; }
    double getMoney() const { return money; }
    int getSoldProducts() const { return soldProducts; }
    int getTopSalesCount() const { return topSalesCount; }
    int getAnalysisCount() const { return analysisCount; }

    // Методы для демонстрации клонирования
    void displayReportInfo() const;             // вывод информации о отчете
    void modifyReportForDemo();                 // модификация для демонстрации

    void DisplayReport() const;                 // вывод отчета на экран

private:
    // Вспомогательные методы
    void inputReportData();                     // ввод данных для отчета
    std::string getCurrentDate();               // получение текущей даты

    // Методы для работы с динамической памятью
    void allocateMemory();                      // выделение памяти
    void deallocateMemory();                    // освобождение памяти
    void copyArrays(const SalesReport& other);  // копирование массивов

    // Вспомогательные методы для клонирования
    void shallowCopyArrays(const SalesReport& other); // поверхностное копирование массивов
};

#endif
#pragma once
