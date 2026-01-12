#ifndef PROVIDERORDER_HPP
#define PROVIDERORDER_HPP

#include <string>
#include <fstream>
#include <iostream>
#include <sstream>
#include <cstdlib>//стандартные функции (например, рандом)
#include <ctime>
#include <limits>

class ProviderOrder {
private:
    static const int MAX_ORDER_ITEMS = 100; // максимальное количество товаров в заказе поставщику

    // Основная информация о заказе поставщику
    std::string numberProviderOrder;// уникальный номер заказа поставщику
    std::string provider;// название компании-поставщика
    int articles[MAX_ORDER_ITEMS]; // массив артикулов заказываемых товаров
    int quantities[MAX_ORDER_ITEMS];// массив количеств для каждого товара
    int itemCount;// текущее количество различных товаров в заказе
    std::string deliveryAddress;// адрес доставки от поставщика
    std::string status;  // текущий статус заказа

public:
    ProviderOrder();
    void AddProvider();// метод для формирования заказа
    bool sending();// метод для отправки заказа поставщику
    void NewInfoStock(); // метод для обновления информации о складе после доставки

    // Геттер для получения номера заказа поставщику
    std::string getNumberProviderOrder() const { return numberProviderOrder; }

private:
    std::string generateOrderNumber();// генерация уникального номера заказа поставщику
    void displayClientOrders();// отображение клиентских заказов
    bool addOrderToProvider(int orderIndex);// добавление заказа поставщику
};

#endif
#pragma once
#pragma once
