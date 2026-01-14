#define _CRT_SECURE_NO_WARNINGS
#include "Search.h"
#include "Product.h"
#include <iostream>
#include <fstream>
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <functional>
#include <memory>
#include <variant>

using namespace std;

// Конструктор
Search::Search() {
    searchResults.reserve(100);
    searchHistory.reserve(50);
}

// Деструктор
Search::~Search() {
    ClearResults();
}

// Загрузка продуктов из файла
vector<Product*> Search::loadProductsFromFile(const string& filename) {
    vector<Product*> products;
    ifstream file(filename);

    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return products;
    }

    string line;
    while (getline(file, line)) {
        if (line.empty()) continue;

        stringstream ss(line);
        string token;
        string tokens[10];
        int tokenCount = 0;

        while (getline(ss, token, '|') && tokenCount < 10) {
            tokens[tokenCount++] = token;
        }

        if (tokenCount == 10) {
            try {
                Product* product = new Product(
                    stoi(tokens[0]),      // article
                    tokens[1],            // name
                    tokens[2],            // brand
                    tokens[3],            // country
                    tokens[4],            // category
                    tokens[5],            // type
                    tokens[6],            // description
                    stoi(tokens[7]),      // quantity
                    stoi(tokens[9])       // price
                );
                products.push_back(product);
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();
    return products;
}

// Проверка соответствия критериям
bool Search::matchesCriteria(const Product* product, const string& field, const string& value) {
    if (!product || value.empty()) return false;

    string productValue;
    string searchValue = value;

    // Приводим к нижнему регистру
    transform(searchValue.begin(), searchValue.end(), searchValue.begin(), ::tolower);

    if (field == "name") {
        productValue = product->getName();
    }
    else if (field == "brand") {
        productValue = product->getBrand();
    }
    else if (field == "country") {
        productValue = product->getProducerCountry();
    }
    else if (field == "category") {
        productValue = product->getCategory();
    }
    else if (field == "type") {
        productValue = product->getTypeProduct();
    }
    else {
        return false;
    }

    transform(productValue.begin(), productValue.end(), productValue.begin(), ::tolower);
    return productValue.find(searchValue) != string::npos;
}

// Проверка ценового диапазона
bool Search::matchesPriceRange(const Product* product, int minPrice, int maxPrice) {
    if (!product) return false;
    int price = product->getPrice();
    return price >= minPrice && price <= maxPrice;
}


// Поиск по названию
void Search::SearchByName(const string& filename, const string& name) {
    ClearResults();
    auto products = loadProductsFromFile(filename);

    // Используем std::copy_if для фильтрации
    copy_if(products.begin(), products.end(), back_inserter(searchResults),
        [&](const Product* p) {
            string productName = p->getName();
            string searchName = name;
            transform(productName.begin(), productName.end(), productName.begin(), ::tolower);
            transform(searchName.begin(), searchName.end(), searchName.begin(), ::tolower);
            return productName.find(searchName) != string::npos;
        });

    // Освобождаем память исходного массива
    for (auto p : products) {
        if (find(searchResults.begin(), searchResults.end(), p) == searchResults.end()) {
            delete p;
        }
    }

    SaveSearchToHistory("По названию: " + name);
}

// Поиск по артикулу
void Search::SearchByArticle(const string& filename, int article) {
    ClearResults();
    auto products = loadProductsFromFile(filename);

    // Используем std::find_if
    auto it = find_if(products.begin(), products.end(),
        [&](const Product* p) { return p->getArticle() == article; });

    if (it != products.end()) {
        searchResults.push_back(*it);
        products.erase(it);
    }

    for (auto p : products) delete p;

    SaveSearchToHistory("По артикулу: " + to_string(article));
}

// Поиск по бренду
void Search::SearchByBrand(const string& filename, const string& brand) {
    ClearResults();
    auto products = loadProductsFromFile(filename);

    copy_if(products.begin(), products.end(), back_inserter(searchResults),
        [&](const Product* p) { return matchesCriteria(p, "brand", brand); });

    for (auto p : products) {
        if (find(searchResults.begin(), searchResults.end(), p) == searchResults.end()) {
            delete p;
        }
    }

    SaveSearchToHistory("По бренду: " + brand);
}

// Поиск по стране
void Search::SearchByCountry(const string& filename, const string& country) {
    ClearResults();
    auto products = loadProductsFromFile(filename);

    copy_if(products.begin(), products.end(), back_inserter(searchResults),
        [&](const Product* p) { return matchesCriteria(p, "country", country); });

    for (auto p : products) {
        if (find(searchResults.begin(), searchResults.end(), p) == searchResults.end()) {
            delete p;
        }
    }

    SaveSearchToHistory("По стране: " + country);
}

// Поиск по категории
void Search::SearchByCategory(const string& filename, const string& category) {
    ClearResults();
    auto products = loadProductsFromFile(filename);

    copy_if(products.begin(), products.end(), back_inserter(searchResults),
        [&](const Product* p) { return matchesCriteria(p, "category", category); });

    for (auto p : products) {
        if (find(searchResults.begin(), searchResults.end(), p) == searchResults.end()) {
            delete p;
        }
    }

    SaveSearchToHistory("По категории: " + category);
}

// Поиск по типу
void Search::SearchByType(const string& filename, const string& type) {
    ClearResults();
    auto products = loadProductsFromFile(filename);

    copy_if(products.begin(), products.end(), back_inserter(searchResults),
        [&](const Product* p) { return matchesCriteria(p, "type", type); });

    for (auto p : products) {
        if (find(searchResults.begin(), searchResults.end(), p) == searchResults.end()) {
            delete p;
        }
    }

    SaveSearchToHistory("По типу: " + type);
}

// Поиск по цене
void Search::SearchByPrice(const string& filename, int minPrice, int maxPrice) {
    ClearResults();
    auto products = loadProductsFromFile(filename);

    copy_if(products.begin(), products.end(), back_inserter(searchResults),
        [&](const Product* p) { return matchesPriceRange(p, minPrice, maxPrice); });

    for (auto p : products) {
        if (find(searchResults.begin(), searchResults.end(), p) == searchResults.end()) {
            delete p;
        }
    }

    SaveSearchToHistory("По цене: " + to_string(minPrice) + "-" + to_string(maxPrice));
}

// Универсальный поиск
void Search::UniversalSearch(const string& filename) {
    ClearResults();
    auto products = loadProductsFromFile(filename);

    cout << "\n=== УНИВЕРСАЛЬНЫЙ ПОИСК ===" << endl;
    cout << "Введите критерии поиска (оставьте пустым для пропуска):" << endl;

    string name, brand, country, category, type;
    int minPrice = 0, maxPrice = INT_MAX;

    cin.ignore();
    cout << "Название: "; getline(cin, name);
    cout << "Бренд: "; getline(cin, brand);
    cout << "Страна: "; getline(cin, country);
    cout << "Категория: "; getline(cin, category);
    cout << "Тип: "; getline(cin, type);
    cout << "Минимальная цена: ";
    string priceInput; getline(cin, priceInput);
    if (!priceInput.empty()) minPrice = stoi(priceInput);
    cout << "Максимальная цена: ";
    getline(cin, priceInput);
    if (!priceInput.empty()) maxPrice = stoi(priceInput);

    // Используем std::copy_if 
    copy_if(products.begin(), products.end(), back_inserter(searchResults),
        [&](const Product* p) {
            bool matches = true;
            if (!name.empty()) matches &= matchesCriteria(p, "name", name);
            if (!brand.empty()) matches &= matchesCriteria(p, "brand", brand);
            if (!country.empty()) matches &= matchesCriteria(p, "country", country);
            if (!category.empty()) matches &= matchesCriteria(p, "category", category);
            if (!type.empty()) matches &= matchesCriteria(p, "type", type);
            matches &= matchesPriceRange(p, minPrice, maxPrice);
            return matches;
        });

    for (auto p : products) {
        if (find(searchResults.begin(), searchResults.end(), p) == searchResults.end()) {
            delete p;
        }
    }

    SaveSearchToHistory("Универсальный поиск");
}

// Найти товар с минимальной ценой (std::min_element)
Product* Search::findMinPriceProduct() const {
    if (searchResults.empty()) return nullptr;

    auto it = min_element(searchResults.begin(), searchResults.end(),
        [](const Product* a, const Product* b) {
            return a->getPrice() < b->getPrice();
        });

    return *it;
}

// Найти товар с максимальной ценой (std::max_element)
Product* Search::findMaxPriceProduct() const {
    if (searchResults.empty()) return nullptr;

    auto it = max_element(searchResults.begin(), searchResults.end(),
        [](const Product* a, const Product* b) {
            return a->getPrice() < b->getPrice();
        });

    return *it;
}

// Фильтрация результатов 
vector<Product*> Search::filterByCriteria(function<bool(const Product*)> predicate) const {
    vector<Product*> filtered;
    copy_if(searchResults.begin(), searchResults.end(), back_inserter(filtered), predicate);
    return filtered;
}

// Трансформация результатов
vector<Product*> Search::transformProducts(function<Product* (const Product*)> transformer) const {
    vector<Product*> transformed;
    transformed.reserve(searchResults.size());
    transform(searchResults.begin(), searchResults.end(), back_inserter(transformed), transformer);
    return transformed;
}

// Проверка наличия хотя бы одного элемента (std::any_of)
bool Search::anyProductMatches(function<bool(const Product*)> predicate) const {
    return any_of(searchResults.begin(), searchResults.end(), predicate);
}

// Удаление элементов по условию (std::remove_if)
void Search::removeFromResults(function<bool(const Product*)> predicate) {
    auto newEnd = remove_if(searchResults.begin(), searchResults.end(),
        [&](Product* p) {
            if (predicate(p)) {
                delete p;
                return true;
            }
            return false;
        });

    searchResults.erase(newEnd, searchResults.end());
}

// Копирование с условием
void Search::copyProducts(const vector<Product*>& source,
    vector<Product*>& destination,
    function<bool(const Product*)> predicate) {
    if (predicate) {
        copy_if(source.begin(), source.end(), back_inserter(destination), predicate);
    }
    else {
        copy(source.begin(), source.end(), back_inserter(destination));
    }
}

// Сортировка результатов (std::sort)
void Search::SortResults(bool ascending) {
    sort(searchResults.begin(), searchResults.end(),
        [ascending](const Product* a, const Product* b) {
            if (ascending) {
                return a->getPrice() < b->getPrice();
            }
            else {
                return a->getPrice() > b->getPrice();
            }
        });
}

// Вывод результатов
void Search::DisplayResults() const {
    cout << "\n=== РЕЗУЛЬТАТЫ ПОИСКА ===" << endl;

    if (searchResults.empty()) {
        cout << "Ничего не найдено!" << endl;
        return;
    }

    cout << "Найдено товаров: " << searchResults.size() << endl;

    // Используем std::for_each для вывода
    int i = 1;
    for (const auto& product : searchResults) {
        cout << "\n--- Товар " << i++ << " ---" << endl;
        product->DisplayInfo();
    }
    // Показываем мин/макс цены 
    auto minProduct = findMinPriceProduct();
    auto maxProduct = findMaxPriceProduct();

    if (minProduct && maxProduct) {
        cout << "\n=== СТАТИСТИКА ===" << endl;
        cout << "Самый дешевый: " << minProduct->getName()
            << " (" << minProduct->getPrice() << " руб.)" << endl;
        cout << "Самый дорогой: " << maxProduct->getName()
            << " (" << maxProduct->getPrice() << " руб.)" << endl;
    }
}

// Очистка результатов
void Search::ClearResults() {
    // Используем std::for_each для удаления
    for_each(searchResults.begin(), searchResults.end(),
        [](Product* p) { delete p; });
    searchResults.clear();
}

// Сохранение в историю
void Search::SaveSearchToHistory(const string& query) {
    searchHistory.push_back(query + " | Найдено: " + to_string(searchResults.size()));

    // Ограничиваем историю последними 10 запросами
    if (searchHistory.size() > 10) {
        searchHistory.erase(searchHistory.begin());
    }
}

// Показать историю поиска
void Search::DisplaySearchHistory() const {
    cout << "\n=== ИСТОРИЯ ПОИСКА ===" << endl;

    if (searchHistory.empty()) {
        cout << "История поиска пуста!" << endl;
        return;
    }

    // Используем std::for_each с индексом
    int i = 1;
    for_each(searchHistory.rbegin(), searchHistory.rend(),
        [&i](const string& entry) {
            cout << i++ << ". " << entry << endl;
        });
}

// Очистка истории
void Search::ClearSearchHistory() {
    searchHistory.clear();
}
