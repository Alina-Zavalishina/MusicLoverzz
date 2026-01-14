#ifndef SEARCH_HPP
#define SEARCH_HPP

#include <string>
#include <fstream>
#include <iostream>
#include <sstream>
#include <algorithm>
#include <cctype>

class Search {
private:
    std::string request;
    int resultCount;

    static const int MAX_RESULTS = 1000;
    static const int MAX_HISTORY = 10;

    struct SearchResult {
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
    } searchResults[MAX_RESULTS];

    std::string searchHistory[MAX_HISTORY];
    int historyCount;

public:
    Search();

    // Основные методы поиска
    void SearchByName(const std::string& filename, const std::string& name);
    void SearchByArticle(const std::string& filename, int article);
    void SearchByBrand(const std::string& filename, const std::string& brand);
    void SearchByCountry(const std::string& filename, const std::string& country);
    void SearchByCategory(const std::string& filename, const std::string& category);
    void SearchByType(const std::string& filename, const std::string& type);
    void SearchByPrice(const std::string& filename, int minPrice, int maxPrice);

    // Универсальный поиск
    void UniversalSearch(const std::string& filename);

    // Методы отображения и управления
    void DisplayResults() const;
    int DisplayResultCount() const;
    void ClearResults();
    void AddToHistory(const std::string& searchInfo);
    void DisplaySearchHistory() const;

    // Новые методы для работы с историей
    std::string getSearchHistoryAsString() const;
    void clearSearchHistory();
    std::string getLastSearchRequest() const { return request; }
    bool hasSearchHistory() const { return historyCount > 0; }

private:
    // Вспомогательные методы для работы со строками
    std::string toLower(const std::string& str) const;
    std::string trim(const std::string& str) const;//удаление пробелов в конце
    bool contains(const std::string& str, const std::string& substr) const;//подстрока
    std::string joinStrings(const std::string parts[], int count, const std::string& delimiter) const;//объединение строк
};

#endif
