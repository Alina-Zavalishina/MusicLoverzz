#include "SalesReport.h"
#include <iostream>
#include <iomanip>
#include <limits>
#include <sstream>

using namespace std;

SalesReport::SalesReport() : money(0.0), soldProducts(0), topSalesCount(0), analysisCount(0) {
    period = getCurrentDate();
}

SalesReport::SalesReport(const std::string& reportPeriod) :
    period(reportPeriod), money(0.0), soldProducts(0), topSalesCount(0), analysisCount(0) {}

void SalesReport::GetReport() {
    cout << "\n=== ОФОРМЛЕНИЕ ОТЧЕТА ПО ПРОДАЖАМ ===" << endl;
    cout << "Период отчета: " << period << endl;

    inputReportData();
    SaveToFile();

    cout << "Отчет успешно оформлен и сохранен!" << endl;
}

void SalesReport::RemakeReport() {
    cout << "\n=== ИЗМЕНЕНИЕ ОТЧЕТА ПО ПРОДАЖАМ ===" << endl;

    string filename = "sales_report_" + period + ".txt";
    if (!LoadFromFile(filename)) {
        cout << "Отчет за период '" << period << "' не найден!" << endl;
        return;
    }

    cout << "Текущие данные отчета:" << endl;
    DisplayReport();

    cout << "\nВведите новые данные:" << endl;
    inputReportData();
    SaveToFile();

    cout << "Отчет успешно изменен!" << endl;
}
// ввод данных для отчета
void SalesReport::inputReportData() {
    // Ввод выручки
    while (true) {
        cout << "Введите выручку: ";
        cin >> money;
        if (cin.fail() || money < 0) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите положительное число." << endl;
            continue;
        }
        break;
    }

    // Ввод количества проданных товаров
    while (true) {
        cout << "Введите количество проданных товаров: ";
        cin >> soldProducts;
        if (cin.fail() || soldProducts < 0) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите целое положительное число." << endl;
            continue;
        }
        break;
    }

    cin.ignore(numeric_limits<streamsize>::max(), '\n');

    // Ввод топа товаров
    cout << "\n=== ТОП ТОВАРОВ ПО ПРОДАЖАМ ===" << endl;
    topSalesCount = 0;
    for (int i = 0; i < MAX_TOP_PRODUCTS; i++) {
        string product;
        cout << "Введите товар №" << (i + 1) << " (или 'stop' для завершения): ";
        getline(cin, product);

        if (product == "stop" || product.empty()) {
            break;
        }

        topSales[topSalesCount++] = product;
    }

    // Ввод аналитики
    cout << "\n=== АНАЛИТИКА ПРОДАЖ ===" << endl;
    analysisCount = 0;
    for (int i = 0; i < MAX_ANALYSIS_ITEMS; i++) {
        string analysisItem;
        cout << "Введите пункт аналитики №" << (i + 1) << " (или 'stop' для завершения): ";
        getline(cin, analysisItem);

        if (analysisItem == "stop" || analysisItem.empty()) {
            break;
        }

        analysis[analysisCount++] = analysisItem;
    }
}

void SalesReport::SaveToFile() {
    string filename = "sales_report_" + period + ".txt";
    ofstream file(filename);

    if (file.is_open()) {
        file << period << endl;
        file << money << endl;
        file << soldProducts << endl;
        file << topSalesCount << endl;

        for (int i = 0; i < topSalesCount; i++) {
            file << topSales[i] << endl;
        }

        file << analysisCount << endl;
        for (int i = 0; i < analysisCount; i++) {
            file << analysis[i] << endl;
        }

        file.close();
    }
}

bool SalesReport::LoadFromFile(const std::string& filename) {
    ifstream file(filename);

    if (!file.is_open()) {
        return false;
    }

    getline(file, period);
    file >> money;
    file >> soldProducts;
    file >> topSalesCount;
    file.ignore();

    for (int i = 0; i < topSalesCount; i++) {
        getline(file, topSales[i]);
    }

    file >> analysisCount;
    file.ignore();

    for (int i = 0; i < analysisCount; i++) {
        getline(file, analysis[i]);
    }

    file.close();
    return true;
}

void SalesReport::DisplayReport() const {
    cout << "\n=== ОТЧЕТ ПО ПРОДАЖАМ ===" << endl;
    cout << "Период: " << period << endl;
    cout << "Выручка: " << fixed << setprecision(2) << money << " руб." << endl;
    cout << "Проданные товары: " << soldProducts << " шт." << endl;

    if (topSalesCount > 0) {
        cout << "\nТоп товаров по продажам:" << endl;
        for (int i = 0; i < topSalesCount; i++) {
            cout << (i + 1) << ". " << topSales[i] << endl;
        }
    }

    if (analysisCount > 0) {
        cout << "\nАналитика продаж:" << endl;
        for (int i = 0; i < analysisCount; i++) {
            cout << "- " << analysis[i] << endl;
        }
    }
    cout << "==========================" << endl;
}

std::string SalesReport::getCurrentDate() {
    time_t now = time(0);

#ifdef _WIN32
    struct tm timeinfo;
    localtime_s(&timeinfo, &now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y", &timeinfo);
#else
    tm* localTime = localtime(&now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y", localTime);
#endif

    return string(buffer);
}