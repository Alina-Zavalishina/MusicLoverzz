#ifndef BASKET_HPP
#define BASKET_HPP

#include "Product.h"
#include <string>
#include <iostream>

class Basket {
private:
    static const int MAX_PRODUCTS = 100;
    Product* products[MAX_PRODUCTS];
    int quantities[MAX_PRODUCTS];
    int productCount;

public:
    // Конструкторы
    Basket();
    Basket(const Basket& other);  // Конструктор копирования

    // Деструктор
    ~Basket();

    // Оператор присваивания
    Basket& operator=(const Basket& other);

    // Основные методы
    double GetBasketCost() const;
    unsigned int GetBasketCount() const;
    void DisplayBasket() const;
    void AddBasket(Product* product, int quantity = 1);
    void DeleteBasket(int productIndex);
    int UpdateQuantity(int productIndex, int newQuantity);
    bool IsEmpty() const;
    void ClearBasket();
    int FindProductIndex(int article) const;

    // Геттеры
    int getProductCount() const { return productCount; }
    Product* getProduct(int index) const;
    int getQuantity(int index) const;


    // 1. Дружественная функция для вывода корзины в поток
    friend std::ostream& operator<<(std::ostream& os, const Basket& basket);

    // 2. Дружественная функция для объединения двух корзин
    friend Basket mergeBaskets(const Basket& basket1, const Basket& basket2);

    // 3. Дружественная функция для расчета скидки
    friend double calculateBasketDiscount(const Basket& basket, double discountPercent);

    // 4. Дружественная функция для сравнения двух корзин
    friend bool areBasketsEqual(const Basket& basket1, const Basket& basket2);

    // 5. Дружественная функция для добавления товара 
    friend void addProductToBasket(Basket& basket, Product* product, int quantity);

    // 6. Дружественная функция для проверки, содержит ли корзина товар
    friend bool basketContainsProduct(const Basket& basket, int article);

    // 7. Дружественная функция для копирования корзины с фильтром по цене
    friend Basket copyBasketWithPriceFilter(const Basket& basket, int maxPrice);

private:
    // Вспомогательные методы
    void deepCopy(const Basket& other);
    void clearProducts();
};

#endif
#pragma once
