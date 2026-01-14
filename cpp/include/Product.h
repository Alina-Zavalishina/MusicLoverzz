#ifndef PRODUCT_HPP
#define PRODUCT_HPP

#include <string>//работа со строками
#include <fstream>// работа с файлами (чтение и запись)
#include <iostream>//работа с консолью

//класс Товаров
class Product {
private:
    //поля 
    int article;//артику
    std::string name;//название
    std::string brand;//бренд
    std::string producerCountry;//страна-производитель
    std::string category;//категория
    std::string typeProduct;//тип
    std::string description;//описание
    unsigned int quantityStock;//количество на складе
    bool statusStock;//статус (в наличие/нет)
    int price;//цена

public:
    //конструктор и деструктор 
    Product();//конструктор
    Product(int art, const std::string& nm, const std::string& br,
        const std::string& country, const std::string& cat,
        const std::string& type, const std::string& desc,
        unsigned int quantity, int pr);
    ~Product() {}// деструктор

    //методы
    void AddStock(); //добавить на склад
    bool DeleteStock();//удалить со склада 
    void EditStock();//редактировать информацию на складе
    bool FindStock();//поиск на складе
    void DisplayInfo();//информация о складе

    // Методы ввода/вывода данных
    void InputFromKeyboard();// Ввод данных с клавиатуры
    void WriteToFile(const std::string& filename);// Запись в файл
    void ReadFromFile(const std::string& filename);// Чтение из файла
    static void DisplayProductsTable(const std::string& filename);// Отображение таблицы товаров

    // Статические методы для работы с файлом товаров
    static void DeleteProductsFromFile(const std::string& filename);// Удаление товаров из файла
    static bool DeleteProductByArticle(const std::string& filename, int articleToDelete);// Удаление по артикулу

    static void EditProductsInFile(const std::string& filename);// Редактирование товаров в файле
    static bool EditProductByIndex(const std::string& filename, int productIndex); // Редактирование по индексу

    static void FindProductsByArticle(const std::string& filename);// Поиск товаров по артикулу(несколько)
    static bool FindProductByArticle(const std::string& filename, int articleToFind);// Поиск конкретного товара(один)

    static bool validateString(const std::string& str, int maxLength = 30);// Проверка строки
    static bool validateNumber(long long number, int maxDigits = 10);// Проверка числа
    static bool validateNonEmpty(const std::string& str); // Проверка на пустую строку

    //методы получения значений
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

    //методы установки значений
    void setArticle(int art) { article = art; }
    void setName(const std::string& nm) { name = nm; }
    void setBrand(const std::string& br) { brand = br; }
    void setProducerCountry(const std::string& country) { producerCountry = country; }
    void setCategory(const std::string& cat) { category = cat; }
    void setTypeProduct(const std::string& type) { typeProduct = type; }
    void setDescription(const std::string& desc) { description = desc; }
    void setQuantityStock(unsigned int quantity) {
        quantityStock = quantity;
        statusStock = (quantity > 0);//обновление статуса налияи
    }
    void setStatusStock(bool status) { statusStock = status; }
    void setPrice(int pr) { price = pr; }
};

#endif