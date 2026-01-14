#ifndef PRODUCT_HPP
#define PRODUCT_HPP

#include <string>
#include <fstream>
#include <iostream>
#include <sstream>
#include <memory>//для работы со смарт-указателями
#include <limits>
#include <vector>//для работы с векторами
#include <algorithm>//для работы с контейнерами
#include <cctype>//работа с символами

class Product {
private:
    int article;
    std::string name;
    std::string brand;
    std::string producerCountry;
    std::string category;
    std::string typeProduct;
    std::string description;
    unsigned int quantityStock;
    bool statusStock;
    int price;

public:
    // Конструкторы
    Product();
    Product(int art, const std::string& nm, const std::string& br,
        const std::string& country, const std::string& cat,
        const std::string& type, const std::string& desc,
        unsigned int quantity, int pr);
    ~Product() = default;

    // Методы работы со складом
    void AddStock();
    bool DeleteStock();
    void EditStock();
    bool FindStock();
    void DisplayInfo();

    // Методы ввода/вывода данных
    void InputFromKeyboard();
    void WriteToFile(const std::string& filename);
    void ReadFromFile(const std::string& filename);

    // Статические методы для работы с файлами
    static void DisplayProductsTable(const std::string& filename);
    static void DeleteProductsFromFile(const std::string& filename);
    static bool DeleteProductByArticle(const std::string& filename, int articleToDelete);
    static void EditProductsInFile(const std::string& filename);
    static bool EditProductByIndex(const std::string& filename, int productIndex);
    static void FindProductsByArticle(const std::string& filename);
    static bool FindProductByArticle(const std::string& filename, int articleToFind);


    static bool validateString(const std::string& str, int maxLength = 30);
    static bool validateNumber(long long number, int maxDigits = 10);
    static bool validateNonEmpty(const std::string& str);

    // методы для работы с умными указателями

    // Загружает все товары из файла и возвращает их как вектор shared_ptr
    static std::vector<std::shared_ptr<Product>> LoadAllProductsSmart(const std::string& filename);

    // Создает объект Product из файла по указанному артикулу
    static std::shared_ptr<Product> CreateProductFromFileSmart(const std::string& filename, int articleToFind);

    // Ищет товары в файле по названию
    static std::vector<std::shared_ptr<Product>> FindProductsByNameSmart(const std::string& filename, const std::string& name);

    // Получает товар из файла по его индексу (порядковому номеру)
    static std::shared_ptr<Product> GetProductByIndexSmart(const std::string& filename, int index);

    // Обновляет информацию о товаре в файле
    static bool UpdateProductInFileSmart(const std::string& filename, std::shared_ptr<Product> updatedProduct);

    // Создает новый товар на основе ввода пользователя с клавиатуры
    static std::shared_ptr<Product> CreateProductFromInputSmart();

    // Ищет товары по списку артикулов
    static std::vector<std::shared_ptr<Product>> FindProductsByArticleSmart(const std::string& filename, const std::vector<int>& articles);

    // Геттеры
    int getArticle() const { return article; }
    std::string getName() const { return name; }
    std::string getBrand() const { return brand; }
    std::string getProducerCountry() const { return producerCountry; }
    std::string getCategory() const { return category; }
    std::string getTypeProduct() const { return typeProduct; }
    std::string getDescription() const { return description; }
    unsigned int getQuantityStock() const { return quantityStock; }
    bool getStatusStock() const { return statusStock; }
    int getPrice() const { return price; }

    // Сеттеры
    void setArticle(int art) { article = art; }
    void setName(const std::string& nm) { name = nm; }
    void setBrand(const std::string& br) { brand = br; }
    void setProducerCountry(const std::string& country) { producerCountry = country; }
    void setCategory(const std::string& cat) { category = cat; }
    void setTypeProduct(const std::string& type) { typeProduct = type; }
    void setDescription(const std::string& desc) { description = desc; }
    void setQuantityStock(unsigned int quantity) {
        quantityStock = quantity;
        statusStock = (quantity > 0);
    }
    void setStatusStock(bool status) { statusStock = status; }
    void setPrice(int pr) { price = pr; }

    // Вспомогательные методы для работы со строками
    static std::string toLower(const std::string& str);
};

#endif
#pragma once

#pragma once
