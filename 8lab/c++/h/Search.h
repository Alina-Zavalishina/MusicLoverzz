#ifndef SEARCH_H
#define SEARCH_H

#include "Product.h"
#include <vector>
#include <string>
#include <functional>

class Search {
private:
    std::vector<Product*> searchResults;
    std::vector<std::string> searchHistory;

    // Вспомогательные методы
    std::vector<Product*> loadProductsFromFile(const std::string& filename);
    bool matchesCriteria(const Product* product, const std::string& field, const std::string& value);
    bool matchesPriceRange(const Product* product, int minPrice, int maxPrice);
    void SaveSearchToHistory(const std::string& query);

public:
    // Конструктор и деструктор
    Search();
    ~Search();

    // Основные методы поиска
    void SearchByName(const std::string& filename, const std::string& name);
    void SearchByArticle(const std::string& filename, int article);
    void SearchByBrand(const std::string& filename, const std::string& brand);
    void SearchByCountry(const std::string& filename, const std::string& country);
    void SearchByCategory(const std::string& filename, const std::string& category);
    void SearchByType(const std::string& filename, const std::string& type);
    void SearchByPrice(const std::string& filename, int minPrice, int maxPrice);
    void UniversalSearch(const std::string& filename);

    // Алгоритмы STL
    void SortResults(bool ascending = true);
    Product* findMinPriceProduct() const;
    Product* findMaxPriceProduct() const;

    // Функциональные операции с предикатами
    std::vector<Product*> filterByCriteria(std::function<bool(const Product*)> predicate) const;
    std::vector<Product*> transformProducts(std::function<Product* (const Product*)> transformer) const;
    bool anyProductMatches(std::function<bool(const Product*)> predicate) const;
    void removeFromResults(std::function<bool(const Product*)> predicate);

    // Утилиты для работы с коллекциями
    void copyProducts(const std::vector<Product*>& source,
        std::vector<Product*>& destination,
        std::function<bool(const Product*)> predicate = nullptr);

    // Отображение и управление результатами
    void DisplayResults() const;
    void ClearResults();
    void DisplaySearchHistory() const;
    void ClearSearchHistory();

    // Геттеры
    std::vector<Product*> getResults() const { return searchResults; }
    std::vector<std::string> getHistory() const { return searchHistory; }

    // Проверка наличия результатов
    bool hasResults() const { return !searchResults.empty(); }
    size_t resultsCount() const { return searchResults.size(); }
};

#endif // SEARCH_H