#ifndef SEARCH_HPP
#define SEARCH_HPP

#include <string>
#include <fstream>
#include <iostream>
#include <sstream>

//класс поиска товаров
class Search {
private:
    std::string request;//запрос на поиск товара
    int resultCount;//количество найденных товаров 
    //статические переменные-общие для всего класса
    static const int MAX_RESULTS = 1000;  //для поиска
    static const int MAX_HISTORY = 10;    //для истории поиска

    // Структура для хранения результатов поиска
    struct SearchResult {
        int article; //артикул
        std::string name; //название
        std::string brand;//бренд
        std::string producerCountry;//страна-производитель
        std::string category;//категория
        std::string typeProduct;//тип
        std::string description;//описание
        unsigned int quantityStock;  //количество
        bool statusStock;//статус
        int price;// цена
    } searchResults[MAX_RESULTS];

    std::string searchHistory[MAX_HISTORY];//история поиска
    int historyCount;//счетчик записи в историю

public:
    Search();//конструктор
    //поиск по различным критериям
    void SearchByName(const std::string& filename, const std::string& name);
    void SearchByArticle(const std::string& filename, int article);
    void SearchByBrand(const std::string& filename, const std::string& brand);
    void SearchByCountry(const std::string& filename, const std::string& country);
    void SearchByCategory(const std::string& filename, const std::string& category);
    void SearchByType(const std::string& filename, const std::string& type);
    void SearchByPrice(const std::string& filename, int minPrice, int maxPrice);
    void DisplayResults() const;//найденные товары
    int DisplayResultCount() const;//показать количество найденных товаров
    void ClearResults();//очистить историю поиска перед поиском каждого нового товара
    void AddToHistory(const std::string& searchInfo);
    void DisplaySearchHistory() const;//история поиска(const-не изменяет состояние объекта)
    static void UniversalSearch(const std::string& filename);//статический метод можно вызвать без создания объекта класса
};

#endif