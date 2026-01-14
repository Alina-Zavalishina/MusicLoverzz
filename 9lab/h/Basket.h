#ifndef BASKET_HPP
#define BASKET_HPP

#include <string>
#include <iostream>
#include <functional>
#include <type_traits>
#include <array>

// структура товара для constexpr вычислений
struct SimpleProduct {
    constexpr SimpleProduct() : article(0), price(0.0), quantity(0) {}
    constexpr SimpleProduct(int art, double pr, int qty)
        : article(art), price(pr), quantity(qty) {}

    int article;
    double price;
    int quantity;

    constexpr double getTotal() const {
        return price * quantity;
    }
};

struct BasketConfig {
    constexpr BasketConfig() : maxProducts(100), defaultTaxRate(0.2), maxDiscount(50.0) {}
    constexpr BasketConfig(int maxProd, double tax, double discount)
        : maxProducts(maxProd), defaultTaxRate(tax), maxDiscount(discount) {}

    int maxProducts;
    double defaultTaxRate;
    double maxDiscount;
};

// Внешние constexpr функции для вычислений на этапе компиляции
constexpr double calculateBasketCostCompileTime(double price, int quantity);
constexpr double calculateDiscount(double price, double discountPercent);
constexpr double calculateTax(double price, double taxRate);


extern const BasketConfig basketConfig;


class SimpleBasket {
private:
    static constexpr int MAX_PRODUCTS = 10;
    std::array<SimpleProduct, MAX_PRODUCTS> products;
    int productCount;

public:

    constexpr SimpleBasket() : productCount(0), products() {}

    // constexpr метод добавления товара
    constexpr void addProduct(const SimpleProduct& product) {
        if (productCount < MAX_PRODUCTS) {
            products[productCount] = product;
            productCount++;
        }
    }

    // constexpr метод вычисления общей стоимости
    constexpr double calculateTotal() const {
        double total = 0.0;
        for (int i = 0; i < productCount; ++i) {
            total += products[i].getTotal();
        }
        return total;
    }

    // constexpr метод вычисления стоимости со скидкой
    constexpr double calculateDiscountedTotal(double discountPercent) const {
        double total = calculateTotal();
        return total * (100.0 - discountPercent) / 100.0;
    }

    // constexpr метод вычисления стоимости с налогом
    constexpr double calculateTotalWithTax(double taxRate) const {
        double total = calculateTotal();
        return total * (1.0 + taxRate / 100.0);
    }

    // Геттеры
    constexpr int getProductCount() const { return productCount; }
    constexpr int getMaxProducts() const { return MAX_PRODUCTS; }
    constexpr const SimpleProduct& getProduct(int index) const {
        return products[index];
    }
};

// Основной класс Basket 
#include "Product.h"  

class Basket {
private:
    static constexpr int MAX_PRODUCTS = 100;
    Product* products[MAX_PRODUCTS];
    int quantities[MAX_PRODUCTS];
    int productCount;

public:
    // Обычные конструкторы
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

    // ШАБЛОННАЯ ФУНКЦИЯ
    template<typename T, typename Func>
    T calculateBasketTotal(Func costCalculator) const {
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

// Внешние constexpr функции
constexpr double calculateBasketCostCompileTime(double price, int quantity) {
    return price * quantity;
}

constexpr double calculateDiscount(double price, double discountPercent) {
    return price * (100.0 - discountPercent) / 100.0;
}

constexpr double calculateTax(double price, double taxRate) {
    return price * (1.0 + taxRate / 100.0);
}

// Функция для демонстрации constexpr вычислений
constexpr SimpleBasket createSampleBasket();

#endif 

