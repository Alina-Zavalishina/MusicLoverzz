#include "Order.h"
#include <iostream>
#include <iomanip>
#include <cstdlib>
#include <ctime>
#include <fstream>
#include <sstream>
#include <limits>
#include <algorithm>

using namespace std;

#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#endif

// Конструктор по умолчанию
Order::Order() : itemCount(0), countOrder(0), costOrder(0.0) {
    // Инициализация массива указателей
    for (int i = 0; i < MAX_ORDER_ITEMS; i++) {
        orderItems[i] = nullptr;
        quantities[i] = 0;
    }

    numberOrder = generateOrderNumber();
    statusOrder = "Ожидает оплаты";
    dateOrder = getCurrentDate();
    shopOrder = "Музыкальный магазин 'MusicLoverzz'";
    payWay = "Не выбран";

    cout << "Вызван конструктор по умолчанию Order: " << numberOrder << endl;
}

// Конструктор с параметрами
Order::Order(const std::string& clientId, Basket& basket) :
    clientOrder(clientId), itemCount(0), countOrder(0), costOrder(0.0) {

    // Инициализация массива указателей
    for (int i = 0; i < MAX_ORDER_ITEMS; i++) {
        orderItems[i] = nullptr;
        quantities[i] = 0;
    }

    numberOrder = generateOrderNumber();
    statusOrder = "Ожидает оплаты";
    dateOrder = getCurrentDate();
    shopOrder = "Музыкальный магазин 'MusicLoverzz'";
    payWay = "Не выбран";

    CopyProductsFromBasket(basket);

    cout << "Вызван конструктор с параметрами Order: " << numberOrder << endl;
}

// Деструктор
Order::~Order() {
    cout << "Вызван деструктор Order: " << numberOrder << endl;
    clearOrderItems();
}

// Очистка массива товаров
void Order::clearOrderItems() {
    for (int i = 0; i < itemCount; i++) {
        if (orderItems[i] != nullptr) {
            delete orderItems[i];
            orderItems[i] = nullptr;
        }
    }
    itemCount = 0;
}

// Копирование товаров из корзины
void Order::CopyProductsFromBasket(Basket& basket) {
    if (basket.IsEmpty()) {
        return;
    }

    itemCount = basket.getProductCount();
    countOrder = basket.GetBasketCount();
    costOrder = basket.GetBasketCost();

    for (int i = 0; i < itemCount; i++) {
        Product* basketProduct = basket.getProduct(i);
        if (basketProduct != nullptr) {
            orderItems[i] = new Product(*basketProduct);
            quantities[i] = basket.getQuantity(i);
        }
    }
}

// Статический метод для создания заказа
bool Order::MakeOrder(const std::string& clientId, Basket& basket) {
    if (basket.IsEmpty()) {
        cout << "Корзина пуста! Добавьте товары перед оформлением заказа." << endl;
        return false;
    }

    char confirmation;
    cout << "\n=== ПОДТВЕРЖДЕНИЕ ЗАКАЗА ===" << endl;
    basket.DisplayBasket();
    cout << "\nВы точно хотите оформить заказ? (y/n): ";
    cin >> confirmation;

    if (confirmation != 'y' && confirmation != 'Y') {
        cout << "Оформление заказа отменено." << endl;
        return false;
    }

    // Создаем заказ
    Order* newOrder = new Order(clientId, basket);

    if (newOrder->Payment()) {
        newOrder->ChangeStatus("Оплачен");
        cout << "\n=== ЗАКАЗ УСПЕШНО ОФОРМЛЕН ===" << endl;
        newOrder->InfoOrder();
        newOrder->SaveToFile();
        basket.ClearBasket();
        delete newOrder;
        return true;
    }
    else {
        newOrder->ChangeStatus("Ошибка оплаты");
        cout << "Ошибка при оформлении заказа!" << endl;
        delete newOrder;
        return false;
    }
}

// Сохранение заказа в файл
void Order::SaveToFile() {
    ofstream file("orders.txt", ios::app);
    if (file.is_open()) {
        file << numberOrder << "|" << clientOrder << "|" << statusOrder << "|" << costOrder;
        for (int i = 0; i < itemCount; i++) {
            if (orderItems[i] != nullptr) {
                file << "|" << orderItems[i]->getArticle() << "|" << quantities[i];
            }
        }
        file << endl;
        file.close();
    }
}

// Изменение статуса заказа
void Order::ChangeStatus(const std::string& newStatus) {
    statusOrder = newStatus;
    if (newStatus == "Оплачен") {
        cout << "\n=== ВАШ ЗАКАЗ УСПЕШНО ОФОРМЛЕН! ===" << endl;
        cout << "Номер заказа: " << numberOrder << endl;
        cout << "Статус: " << statusOrder << endl;
        cout << "==================================" << endl;
    }
    else {
        cout << "Статус заказа изменен на: " << statusOrder << endl;
    }
}

// Оплата заказа
bool Order::Payment() {
    cout << "\n=== ОПЛАТА ЗАКАЗА ===" << endl;
    cout << "Сумма к оплате: " << costOrder << " руб." << endl;

    int paymentChoice;
    bool validChoice = false;

    while (!validChoice) {
        cout << "Выберите способ оплаты:" << endl;
        cout << "1. Банковская карта" << endl;
        cout << "2. Электронный кошелек" << endl;
        cout << "3. Наложенный платеж" << endl;
        cout << "Ваш выбор: ";
        cin >> paymentChoice;

        if (cin.fail()) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка! Введите число от 1 до 3." << endl;
            continue;
        }

        if (paymentChoice >= 1 && paymentChoice <= 3) {
            validChoice = true;
        }
        else {
            cout << "Неверный выбор! Введите 1, 2 или 3." << endl;
        }
    }

    switch (paymentChoice) {
    case 1:
        payWay = "Банковская карта";
        break;
    case 2:
        payWay = "Электронный кошелек";
        break;
    case 3:
        payWay = "Наложенный платеж";
        break;
    }

    cout << "Обработка платежа..." << endl;
    cout << "Способ оплаты: " << payWay << endl;
    cout << "Оплата прошла успешно!" << endl;
    return true;
}

// Вывод информации о заказе
void Order::InfoOrder() const {
    cout << "\n=== ИНФОРМАЦИЯ О ЗАКАЗЕ ===" << endl;
    cout << "Номер заказа: " << numberOrder << endl;
    cout << "Клиент: " << clientOrder << endl;
    cout << "Дата заказа: " << dateOrder << endl;
    cout << "Статус: " << statusOrder << endl;
    cout << "Способ оплаты: " << payWay << endl;
    cout << "Количество товаров: " << countOrder << endl;
    cout << "Общая стоимость: " << costOrder << " руб." << endl;
    cout << "Магазин: " << shopOrder << endl;

    if (itemCount > 0) {
        cout << "\nСостав заказа:" << endl;
        cout << "------------------------------------" << endl;
        for (int i = 0; i < itemCount; i++) {
            if (orderItems[i] != nullptr) {
                cout << (i + 1) << ". " << orderItems[i]->getName()
                    << " - " << quantities[i] << " шт. x "
                    << orderItems[i]->getPrice() << " руб. = "
                    << (quantities[i] * orderItems[i]->getPrice()) << " руб." << endl;
            }
        }
        cout << "------------------------------------" << endl;
    }
    cout << "==================================" << endl;
}

// Генерация номера заказа
std::string Order::generateOrderNumber() {
    srand(static_cast<unsigned int>(time(0)));
    int randomNum = rand() % 10000;
    return "ORD" + std::to_string(randomNum);
}

// Получение текущей даты
std::string Order::getCurrentDate() {
    time_t now = time(0);

#ifdef _MSC_VER
    struct tm timeinfo;
    localtime_s(&timeinfo, &now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", &timeinfo);
#else
    tm* localTime = localtime(&now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", localTime);
#endif

    return std::string(buffer);
}
