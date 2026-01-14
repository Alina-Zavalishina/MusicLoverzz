#include "ProviderOrder.h"
#include "Shop.h"
#include <iostream>
#include <fstream>
#include <sstream>
#include <cstdlib>
#include <ctime>
#include <limits>
#include <vector>
#include <string>

using namespace std;

// Объявление внешней переменной - глобальный объект магазина
extern Shop musicShop;

// Конструктор класса 
ProviderOrder::ProviderOrder() : itemCount(0) {
    // Инициализация массивов артикулов и количеств
    for (int i = 0; i < MAX_ORDER_ITEMS; i++) {
        articles[i] = 0;// обнуляем все артикулы
        quantities[i] = 0;// обнуляем все количества
    }
    // Установка базовой информации о заказе поставщику
    numberProviderOrder = generateOrderNumber();// генерация номера заказа
    status = "Создан";// начальный статус
    deliveryAddress = "Склад магазина";// адрес доставки
    provider = "Основной поставщик";  // поставщик
}
// Метод добавления заказа поставщику на основе клиентских заказов
void ProviderOrder::AddProvider() {
    cout << "\n=== ДОБАВЛЕНИЕ ТОВАРА В ЗАКАЗ ПОСТАВЩИКУ ===" << endl;
    displayClientOrders();

    ifstream file("orders.txt");
    if (!file.is_open()) {
        cout << "Файл заказов клиентов не найден!" << endl;
        return;
    }
    // Подсчитываем количество заказов в файле
    int orderCount = 0;
    string line;
    while (getline(file, line)) {
        if (!line.empty()) orderCount++;
    }
    file.close();

    if (orderCount == 0) {
        cout << "Нет заказов клиентов!" << endl;
        return;
    }

    int selectedOrder;
    cout << "\nВыберите номер заказа для отправки поставщику (1-" << orderCount << "): ";
    cin >> selectedOrder;

    if (cin.fail() || selectedOrder < 1 || selectedOrder > orderCount) {
        cout << "Неверный выбор!" << endl;
        cin.clear();
        cin.ignore(numeric_limits<streamsize>::max(), '\n');
        return;
    }
    // Добавляем выбранный заказ к поставщику
    if (addOrderToProvider(selectedOrder - 1)) {
        cout << "Заказ успешно добавлен к поставщику!" << endl;
        cout << "Номер заказа поставщика: " << numberProviderOrder << endl;
    }
    else {
        cout << "Ошибка при добавлении заказа!" << endl;
    }
}
// Метод добавления конкретного заказа к поставщику
bool ProviderOrder::addOrderToProvider(int orderIndex) {
    ifstream file("orders.txt");
    if (!file.is_open()) {
        return false;
    }

    string line;
    int currentIndex = 0;

    while (getline(file, line)) {
        if (line.empty()) continue;
        if (currentIndex == orderIndex) {// нашли нужный заказ
            stringstream ss(line);
            string token;
            string tokens[20];// массив для хранения всех полей заказа
            int tokenCount = 0;
            // Разбиваем строку по разделителю '|'
            while (getline(ss, token, '|') && tokenCount < 20) {
                tokens[tokenCount++] = token;
            }
            // Проверяем что заказ содержит достаточно данных
            if (tokenCount >= 5) {
                cout << "\nДобавляем заказ клиента: " << tokens[0] << endl;
                // Обрабатываем товары в заказе
                for (int i = 4; i < tokenCount - 1; i += 2) {
                    if (itemCount >= MAX_ORDER_ITEMS) {
                        // Проверка на переполнение массива товаров
                        cout << "Достигнуто максимальное количество товаров!" << endl;
                        break;
                    }

                    try {
                        // Извлекаем артикул и количество товара
                        int article = stoi(tokens[i]);
                        int quantity = stoi(tokens[i + 1]);
                        // Проверяем, есть ли уже такой товар в заказе поставщику
                        bool alreadyExists = false;
                        for (int j = 0; j < itemCount; j++) {
                            if (articles[j] == article) {
                                // Если товар уже есть - увеличиваем количество
                                quantities[j] += quantity;
                                alreadyExists = true;
                                break;
                            }
                        }
                        // Если товара нет - добавляем новый
                        if (!alreadyExists) {
                            articles[itemCount] = article;
                            quantities[itemCount] = quantity;
                            itemCount++;// увеличиваем счетчик товаров
                        }
                        // Получаем название товара из файла products.txt
                        string productName = "Неизвестный товар";
                        ifstream productFile("products.txt");
                        if (productFile.is_open()) {
                            string productLine;// переменная для хранения одной строки из файла товаров
                            while (getline(productFile, productLine)) {
                                // Создаем строковый поток из текущей строки файла
                                stringstream productSS(productLine);
                                string productToken;// переменная для хранения одного поля товара
                                getline(productSS, productToken, '|');// Читаем первое поле до разделителя '|' 
                                try {
                                    // Преобразуем строку с артикулом в число и сравниваем с искомым артикулом
                                    if (stoi(productToken) == article) {
                                        // Если артикулы совпали - читаем следующее поле (название товара)
                                        getline(productSS, productToken, '|');
                                        productName = productToken;// сохраняем найденное название товара
                                        break;
                                    }
                                }
                                catch (...) {}// Игнорируем любые ошибки преобразования
                            }
                            productFile.close();
                        }

                        cout << "Добавлен товар: " << productName << " (арт: " << article
                            << ") - " << quantity << " шт." << endl;
                    }
                    catch (const exception& e) {
                        continue;
                    }
                }
            }
            file.close();
            return true;
        }
        currentIndex++;// Увеличиваем счетчик текущего индекса заказа
    }
    file.close();
    return false;
}
// Метод отображения списка клиентских заказов
void ProviderOrder::displayClientOrders() {
    // Открываем файл с заказами клиентов
    ifstream file("orders.txt");
    if (!file.is_open()) {
        cout << "Файл заказов клиентов не найден!" << endl;
        return;
    }

    cout << "\n=== СПИСОК ЗАКАЗОВ КЛИЕНТОВ ===" << endl;
    string line;
    int orderNumber = 1;// счетчик заказов для отображения
    // Читаем файл построчно
    while (getline(file, line)) {
        if (line.empty()) continue;// пропускаем пустые строки
        // Создаем строковый поток для разбора строки заказа
        stringstream ss(line);
        string token;
        string tokens[10];// массив для хранения полей заказа
        int tokenCount = 0;
        // Разбиваем строку на поля по разделителю '|'
        while (getline(ss, token, '|') && tokenCount < 10) {
            tokens[tokenCount++] = token;
        }
        // Проверяем что заказ содержит минимум 4 поля (основная информация)
        if (tokenCount >= 4) {
            // Выводим основную информацию о заказе
            cout << orderNumber << ". Заказ №" << tokens[0] << " | Клиент: " << tokens[1]
                << " | Статус: " << tokens[2] << " | Стоимость: " << tokens[3] << " руб." << endl;
            // Если есть товары в заказе (поля с 4-го и далее)
            if (tokenCount > 4) {
                cout << "   Товары: ";
                // Обрабатываем пары "артикул|количество"
                for (int i = 4; i < tokenCount - 1; i += 2) {
                    try {
                        int article = stoi(tokens[i]);
                        int quantity = stoi(tokens[i + 1]);
                        // Ищем название товара по артикулу в файле products.txt
                        string productName = "Неизвестный";
                        ifstream productFile("products.txt");
                        if (productFile.is_open()) {
                            string productLine;
                            while (getline(productFile, productLine)) {
                                stringstream productSS(productLine);
                                string productToken;
                                getline(productSS, productToken, '|');// Читаем артикул товара
                                try {
                                    // Сравниваем артикулы
                                    if (stoi(productToken) == article) {
                                        // Если нашли - читаем название товара
                                        getline(productSS, productToken, '|');
                                        productName = productToken;
                                        break;
                                    }
                                }
                                catch (...) {}
                            }
                            productFile.close();
                        }
                        cout << productName << "(" << quantity << " шт.) ";// Выводим информацию о товаре
                    }
                    catch (...) {}
                }
                cout << endl;
            }
        }
        orderNumber++;// увеличиваем счетчик заказов
        cout << endl;
    }
    file.close();
}
// Метод отправки заказа поставщику
bool ProviderOrder::sending() {
    cout << "\n=== ОТПРАВКА ЗАКАЗА ПОСТАВЩИКУ ===" << endl;
    if (itemCount == 0) {
        cout << "Заказ пуст! Сначала добавьте товары через пункт 'Добавить товар в заказ'." << endl;
        return false;
    }

    cout << "Текущий заказ поставщика №" << numberProviderOrder << ":" << endl;
    cout << "Количество позиций: " << itemCount << endl;
    cout << "Товары: ";
    // Перебираем все товары в заказе поставщика
    for (int i = 0; i < itemCount; i++) {
        // Ищем название товара по артикулу
        string productName = "Неизвестный товар";
        ifstream productFile("products.txt");
        if (productFile.is_open()) {
            string productLine;
            while (getline(productFile, productLine)) {
                stringstream productSS(productLine);
                string productToken;
                getline(productSS, productToken, '|');
                try {
                    if (stoi(productToken) == articles[i]) {
                        getline(productSS, productToken, '|');
                        productName = productToken;
                        break;
                    }
                }
                catch (...) {}
            }
            productFile.close();
        }
        cout << productName << "(" << quantities[i] << " шт.) ";// Выводим информацию о товаре
    }
    cout << endl;
    // Запрашиваем подтверждение отправки
    char confirmation;
    cout << "\nВы уверены, что хотите отправить этот заказ поставщику? (y/n): ";
    cin >> confirmation;

    if (confirmation != 'y' && confirmation != 'Y') {
        cout << "Отправка отменена." << endl;
        return false;
    }

    cout << "Отправка заказа поставщику '" << provider << "'..." << endl;
    cout << "Адрес доставки: " << deliveryAddress << endl;
    cout << "Заказ успешно отправлен поставщику!" << endl;
    status = "Отправлен поставщику";
    cout << "Статус заказа изменен на: '" << status << "'" << endl;
    // Создаем копию заказа и добавляем в магазин
    ProviderOrder* newProviderOrder = new ProviderOrder(*this);
    musicShop.AddProviderOrder(newProviderOrder);

    return true;
}
// Метод обновления информации о товарах на складе после доставки
void ProviderOrder::NewInfoStock() {
    cout << "\n=== ОБНОВЛЕНИЕ ИНФОРМАЦИИ О ТОВАРАХ ===" << endl;
    if (itemCount == 0) {// Проверяем что есть товары для обновления
        cout << "Нет товаров для обновления! Сначала добавьте товары и отправьте заказ." << endl;
        return;
    }
    // Проверяем что заказ был отправлен поставщику
    if (status != "Отправлен поставщику") {
        cout << "Заказ еще не отправлен поставщику! Сначала отправьте заказ." << endl;
        return;
    }

    cout << "Поставщик '" << provider << "' доставил товары:" << endl;
    // Перебираем все товары в заказе
    for (int i = 0; i < itemCount; i++) {
        string productName = "Неизвестный товар";
        ifstream nameFile("products.txt"); // Ищем название товара
        if (nameFile.is_open()) {
            string productLine;
            while (getline(nameFile, productLine)) {
                stringstream productSS(productLine);
                string productToken;
                getline(productSS, productToken, '|');
                try {
                    if (stoi(productToken) == articles[i]) {
                        getline(productSS, productToken, '|');
                        productName = productToken;
                        break;
                    }
                }
                catch (...) {}
            }
            nameFile.close();
        }
        cout << "• " << productName << " (арт." << articles[i] << ") - " << quantities[i] << " шт." << endl;

        ifstream inFile("products.txt");// Обновляем количество товара на складе
        ofstream outFile("temp_products.txt");// временный файл
        string line;
        bool updated = false;

        if (inFile.is_open() && outFile.is_open()) {// Читаем исходный файл и записываем обновленный
            while (getline(inFile, line)) {
                if (line.empty()) continue;
                stringstream ss(line);
                string token;
                string tokens[10];
                int tokenCount = 0;

                while (getline(ss, token, '|') && tokenCount < 10) {
                    tokens[tokenCount++] = token;
                }

                if (tokenCount == 10) {
                    try {
                        int currentArticle = stoi(tokens[0]); // Если нашли нужный товар - обновляем количество
                        if (currentArticle == articles[i]) {// Увеличиваем количество на складе
                            int newQuantity = stoi(tokens[7]) + quantities[i];
                            tokens[7] = to_string(newQuantity);
                            tokens[8] = newQuantity > 0 ? "1" : "0";// Обновляем статус наличия
                            updated = true;
                            cout << "   Обновлено количество: " << tokens[7] << " шт." << endl;
                        }
                        // Записываем строку (обновленную)
                        for (int j = 0; j < tokenCount; j++) {
                            outFile << tokens[j];
                            if (j < tokenCount - 1) outFile << "|";
                        }
                        outFile << endl;
                    }
                    catch (const exception& e) {
                        outFile << line << endl;// При ошибке записываем исходную строку
                    }
                }
                else {
                    outFile << line << endl;// Если не все поля - записываем как есть
                }
            }
            inFile.close();
            outFile.close();
            // Заменяем исходный файл обновленным
            remove("products.txt");
            rename("temp_products.txt", "products.txt");
        }
    }

    cout << "Информация о складе успешно обновлена!" << endl;
    status = "Выполнен"; // Обновляем статус заказа
    cout << "Статус заказа изменен на: 'Выполнен'" << endl;

    itemCount = 0;   // Очищаем заказ после выполнения
    for (int i = 0; i < MAX_ORDER_ITEMS; i++) {
        articles[i] = 0;
        quantities[i] = 0;
    }
}
// Метод генерации уникального номера заказа поставщику
std::string ProviderOrder::generateOrderNumber() {
    srand(static_cast<unsigned int>(time(0)));
    int randomNum = rand() % 10000;// генерация случайного числа от 0 до 9999
    return "PROV" + to_string(randomNum);
}
