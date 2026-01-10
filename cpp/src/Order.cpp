#include "Order.h"
#include <iostream>
#include <iomanip>
#include <cstdlib>//генерация случайных чисел
#include <ctime>
#include <fstream>
#include <sstream>
#include <limits>//пределы типов данных

using namespace std;

#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#endif

//конструктор по умолчанию
Order::Order() : itemCount(0), countOrder(0), costOrder(0.0) {//создание "пустого" заказа
    for (int i = 0; i < MAX_ORDER_ITEMS; i++) {
        orderItems[i] = nullptr;
        quantities[i] = 0;
    }
    numberOrder = generateOrderNumber();
    statusOrder = "Ожидает оплаты";
    dateOrder = getCurrentDate();
    shopOrder = "Музыкальный магазин 'MusicLoverzz'";
    payWay = "Не выбран";
}

// Конструктор с параметрами (клиент и корзина)
Order::Order(const std::string& clientId, Basket& basket) :
    clientOrder(clientId), itemCount(0), countOrder(0), costOrder(0.0) {

    for (int i = 0; i < MAX_ORDER_ITEMS; i++) {
        orderItems[i] = nullptr;
        quantities[i] = 0;
    }

    numberOrder = generateOrderNumber();
    statusOrder = "Ожидает оплаты";
    dateOrder = getCurrentDate();
    shopOrder = "Музыкальный магазин 'MusicLoverzz'";
    payWay = "Не выбран";

    CopyProductsFromBasket(basket);//копирование информации из корзины
}

// Деструктор класса Order - освобождение памяти
Order::~Order() {
    // Проходим по всем товарам в заказе
    for (int i = 0; i < itemCount; i++) {
        // Проверяем что указатель не нулевой
        if (orderItems[i] != nullptr) {
            delete orderItems[i];// освобождаем память каждого товара
        }
    }
}

// Метод копирования товаров из корзины в заказ
void Order::CopyProductsFromBasket(Basket& basket) {
    if (basket.IsEmpty()) {//проверка не пустая ли корзина
        return;// если корзина пуста - выходим
    }

    // Копируем основную информацию из корзины
    itemCount = basket.getProductCount();// количество различных товаров
    countOrder = basket.GetBasketCount();// общее количество единиц товара
    costOrder = basket.GetBasketCost();// общая стоимость

    // Копируем каждый товар из корзины в заказ
    for (int i = 0; i < itemCount; i++) {
        // Получаем товар из корзины
        Product* basketProduct = basket.getProduct(i);
        // Проверяем что товар существует
        if (basketProduct != nullptr) {
            // Создаем копию товара
            orderItems[i] = new Product(
                // Создаем ПОЛНУЮ КОПИЮ товара с помощью конструктора
                basketProduct->getArticle(),
                basketProduct->getName(),
                basketProduct->getBrand(),
                basketProduct->getProducerCountry(),
                basketProduct->getCategory(),
                basketProduct->getTypeProduct(),
                basketProduct->getDescription(),
                basketProduct->getQuantityStock(),
                basketProduct->getPrice()
            );
            // Копируем количество товара
            quantities[i] = basket.getQuantity(i);
        }
    }
}
// Статический метод для создания заказа (можно вызывать без объекта)
bool Order::MakeOrder(const std::string& clientId, Basket& basket) {
    // Проверка что корзина не пустая
    if (basket.IsEmpty()) {
        cout << "Корзина пуста! Добавьте товары перед оформлением заказа." << endl;
        return false;// возвращаем false если корзина пуста
    }

    // Запрос подтверждения у пользователя
    char confirmation;
    cout << "\n=== ПОДТВЕРЖДЕНИЕ ЗАКАЗА ===" << endl;
    basket.DisplayBasket();
    cout << "\nВы точно хотите оформить заказ? (y/n): ";
    cin >> confirmation;

    if (confirmation != 'y' && confirmation != 'Y') {
        cout << "Оформление заказа отменено." << endl;
        return false;
    }

    // Создаем новый заказ из корзины
    Order* newOrder = new Order(clientId, basket);

    // Проверяем оплату
    if (newOrder->Payment()) {
        // Если оплата прошла успешно
        newOrder->ChangeStatus("Оплачен");
        cout << "\n=== ЗАКАЗ УСПЕШНО ОФОРМЛЕН ===" << endl;
        newOrder->InfoOrder();// показываем информацию о заказе
        newOrder->SaveToFile();// сохраняем заказ в файл

        // Очищаем корзину
        basket.ClearBasket();
        delete newOrder; // освобождаем память
        return true;
    }
    else {
        newOrder->ChangeStatus("Ошибка оплаты");
        cout << "Ошибка при оформлении заказа!" << endl;
        delete newOrder; // освобождаем память
        return false;
    }
}

// Метод сохранения заказа в файл
void Order::SaveToFile() {
    // Открываем файл для добавления
    ofstream file("orders.txt", ios::app);
    if (file.is_open()) {
        // Записываем основную информацию о заказе
        file << numberOrder << "|" << clientOrder << "|" << statusOrder << "|" << costOrder;
        // Записываем информацию о каждом товаре в заказе
        for (int i = 0; i < itemCount; i++) {
            if (orderItems[i] != nullptr) {
                file << "|" << orderItems[i]->getArticle() << "|" << quantities[i];//записать артикул, количество
            }
        }
        file << endl;
        file.close();
    }
}

// Метод изменения статуса заказа
void Order::ChangeStatus(const std::string& newStatus) {
    statusOrder = newStatus;
    if (newStatus == "Оплачен") {
        cout << "\n=== ВАШ ЗАКАЗ УСПЕШНО ОФОРМЛЕН! ===" << endl;
        cout << "Номер заказа: " << numberOrder << endl;
        cout << "Статус: " << statusOrder << endl;
        cout << "==================================" << endl;
    }
    else {
        cout << "Статус заказа изменен на: " << statusOrder << endl; // Для других статусов просто выводим сообщение
    }
}

// Метод обработки оплаты заказа
bool Order::Payment() {
    cout << "\n=== ОПЛАТА ЗАКАЗА ===" << endl;
    cout << "Сумма к оплате: " << costOrder << " руб." << endl;

    int paymentChoice;// хранит число, которое введет пользователь
    bool validChoice = false;// изначально false - выбор еще не сделан или неверен

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
        // Проверка допустимости выбора
        if (paymentChoice >= 1 && paymentChoice <= 3) {
            validChoice = true;// выбор корректен
        }
        else {
            cout << "Неверный выбор! Введите 1, 2 или 3." << endl;
        }
    }
    // Установка способа оплаты в зависимости от выбора
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
// Метод вывода полной информации о заказе
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
    // Вывод состава заказа если есть товары
    if (itemCount > 0) {
        cout << "\nСостав заказа:" << endl;
        cout << "----------------------------------" << endl;
        // Перебор всех товаров в заказе
        for (int i = 0; i < itemCount; i++) {
            if (orderItems[i] != nullptr) {
                // Вывод информации о каждом товаре
                cout << (i + 1) << ". " << orderItems[i]->getName()
                    << " - " << quantities[i] << " шт. x "
                    << orderItems[i]->getPrice() << " руб. = "
                    << (quantities[i] * orderItems[i]->getPrice()) << " руб." << endl;
            }
        }
        cout << "----------------------------------" << endl;
    }
    cout << "==================================" << endl;
}

// Метод генерации уникального номера заказа
std::string Order::generateOrderNumber() {
    srand(static_cast<unsigned int>(time(0)));// инициализация генератора случайных чисел
    int randomNum = rand() % 10000;// генерация случайного числа от 0 до 9999
    return "ORD" + std::to_string(randomNum);// создание номера
}

// Метод получения текущей даты и времени
std::string Order::getCurrentDate() {
    time_t now = time(0);// получение текущего времени
    // получение локального времени
#ifdef _MSC_VER
    struct tm timeinfo;
    localtime_s(&timeinfo, &now);// безопасное получение локального времени
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", &timeinfo);// форматирование даты
#else
    tm* localTime = localtime(&now);// получение локального времени
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", localTime);// форматирование даты
#endif
    return std::string(buffer);// возврат даты в виде строки
}