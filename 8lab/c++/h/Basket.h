#ifndef BASKET_HPP
#define BASKET_HPP

#include "Product.h"
#include <string>
#include <iostream>
#include <functional>
#include <type_traits>

class Basket {
private:
    static const int MAX_PRODUCTS = 100;
    Product* products[MAX_PRODUCTS];
    int quantities[MAX_PRODUCTS];
    int productCount;

public:
    // Конструкторы
    Basket();
    Basket(const Basket& other);
    ~Basket();
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


    // ШАБЛОННАЯ ФУНКЦИЯ - ВЫЗЫВАЕТСЯ В .CPP
    template<typename T, typename Func>
    T calculateBasketTotal(Func costCalculator) const {
        // Ограничение на уровне компиляции
        static_assert(std::is_arithmetic<T>::value,
            "Тип T должен быть арифметическим (int, float, double и т.д.)");

        T total = T(0);

        for (int i = 0; i < productCount; ++i) {
            if (products[i] != nullptr) {
                total += costCalculator(*products[i], quantities[i]);
            }
        }

        return total;
    }

    // Методы, использующие шаблонную функцию
    double calculateCostWithTax(double taxRate) const;
    double calculateCostWithDiscount(double discountPercent) const;
    int calculateTotalQuantity() const;

    // Дружественные функции
    friend std::ostream& operator<<(std::ostream& os, const Basket& basket);
    friend Basket mergeBaskets(const Basket& basket1, const Basket& basket2);
    friend double calculateBasketDiscount(const Basket& basket, double discountPercent);
    friend bool areBasketsEqual(const Basket& basket1, const Basket& basket2);
    friend void addProductToBasket(Basket& basket, Product* product, int quantity);
    friend bool basketContainsProduct(const Basket& basket, int article);
    friend Basket copyBasketWithPriceFilter(const Basket& basket, int maxPrice);

private:
    void deepCopy(const Basket& other);
    void clearProducts();
};

#endif
