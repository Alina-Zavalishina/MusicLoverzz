#ifndef SALESREPORT_HPP
#define SALESREPORT_HPP

#include <string>
#include <iostream>
#include <fstream>
#include <ctime>

class SalesReport {
private:
    static const int MAX_TOP_PRODUCTS = 10; // максимальное количество товаров в топе продаж
    static const int MAX_ANALYSIS_ITEMS = 20;// максимальное количество пунктов анализа

    std::string period;// период отчета
    double money; // общая выручка за период
    int soldProducts;// общее количество проданных товаров
    std::string topSales[MAX_TOP_PRODUCTS];// массив самых продаваемых товаров
    int topSalesCount;  // текущее количество товаров в топе
    std::string analysis[MAX_ANALYSIS_ITEMS];// массив аналитических записей
    int analysisCount; // текущее количество записей анализа

public:
    SalesReport();
    SalesReport(const std::string& reportPeriod);

    void GetReport();// создание нового отчета
    void RemakeReport();// пересчет существующего отчета

    void SaveToFile();// сохранение отчета в файл
    bool LoadFromFile(const std::string& filename);// загрузка отчета из файла

    std::string getPeriod() const { return period; }// получить период отчета
    double getMoney() const { return money; } // получить общую выручку
    int getSoldProducts() const { return soldProducts; }// получить количество проданных товаров

    void DisplayReport() const;// вывод отчета на экран

private:
    void inputReportData();// ввод данных для отчета
    std::string getCurrentDate();// получение текущей даты
};

#endif
#pragma once

