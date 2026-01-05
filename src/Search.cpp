#include "Search.h"
#include <fstream>
#include <sstream>
#include <algorithm>//работа с данными 
#include <cctype>//(нижний регистр) работа с символами 

using namespace std;//позволяет не писать std перед элементами стандартной библиотеки

Search::Search() : resultCount(0), historyCount(0) {}//объект Поиск(нет результатов поиска, нет истории поиска)

//преобразование строки в нижний регистр
string toLower(const string& str) {
    string result = str;
    transform(result.begin(), result.end(), result.begin(), ::tolower);
    return result;
}

//очистка результатов поиска
void Search::ClearResults() {
    resultCount = 0;
}

//добавить в историю поиска
void Search::AddToHistory(const std::string& searchInfo) {
    if (historyCount >= MAX_HISTORY) {//проверка переполнения истории (не больше 10)
        for (int i = 0; i < MAX_HISTORY - 1; i++) {
            searchHistory[i] = searchHistory[i + 1];
        }
        historyCount = MAX_HISTORY - 1;
    }
    searchHistory[historyCount] = searchInfo;//добавляет новый "поиск" в конец
    historyCount++;//увеличение счетчика
}

//история поиска в виде таблицы
void Search::DisplaySearchHistory() const {
    cout << "\n=== ИСТОРИЯ ПОИСКА ===" << endl;
    if (historyCount == 0) {
        cout << "История поиска пуста!" << endl;
        cout << "==========================" << endl;
        return;
    }
    cout << "Последние " << historyCount << " запросов:" << endl;
    cout << "==========================" << endl;
    for (int i = historyCount - 1; i >= 0; i--) {
        cout << "Запрос #" << (historyCount - i) << ": " << searchHistory[i] << endl;
    }
    cout << "==========================" << endl;
}

//поиск по названию
void Search::SearchByName(const std::string& filename, const std::string& name) {
    ClearResults();//очистка результатов поиска
    ifstream file(filename);//открыть файл
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;//переменная для хранения строки
    string searchNameLower = toLower(name);//преобразование в нижний регистр
    while (getline(file, line) && resultCount < MAX_RESULTS) {
        if (line.empty()) continue;//пропустить пустые строки
        stringstream ss(line);
        string token;//для хранение 1 токена
        string tokens[10];//массив для хранения токенов
        int tokenCount = 0;//количество найденных полей

        //по разделителю строка делится на токены и пишется в массив
        while (getline(ss, token, '|') && tokenCount < 10) {
            tokens[tokenCount++] = token;
        }

        //проверка наличия всех 10 токенов в массиве
        if (tokenCount == 10) {
            try {
                string productName = tokens[1];//название
                string productNameLower = toLower(productName);//название с нижним регистром
                //проверяем содержит ли строка искомое поле
                if (productNameLower.find(searchNameLower) != string::npos) {//npos - индикации отсутствия позиции символа

                    // если найдено совпадение, заполняем структуру товара:
                    searchResults[resultCount].article = stoi(tokens[0]);//stoi-строка->число
                    searchResults[resultCount].name = tokens[1];// название
                    searchResults[resultCount].brand = tokens[2];// бренд
                    searchResults[resultCount].producerCountry = tokens[3];// страна-производитель
                    searchResults[resultCount].category = tokens[4];// категория
                    searchResults[resultCount].typeProduct = tokens[5];// тип товара
                    searchResults[resultCount].description = tokens[6];// описание
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);// количество на складе
                    searchResults[resultCount].statusStock = (tokens[8] == "1");//cтатус
                    searchResults[resultCount].price = stoi(tokens[9]);// цена
                    resultCount++;// увеличиваем счетчик найденных товаров
                }
            }
            catch (const exception& e) {
                continue;//пропуск проблемной строки
            }
        }
    }
    file.close();
    DisplayResultCount();
    string historyEntry = "По названию: '" + name + "' - найдено: " + to_string(resultCount);//to_string-преобразования числа в строку
    AddToHistory(historyEntry);//записать в историю поиска
}

//поиск по артикулу
void Search::SearchByArticle(const std::string& filename, int article) {
    ClearResults();
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    while (getline(file, line) && resultCount < MAX_RESULTS) {
        if (line.empty()) continue;
        stringstream ss(line);//разбитие потока на строки
        string token;
        getline(ss, token, '|');//по | разделить
        try {
            int productArticle = stoi(token);//строка в число
            if (productArticle == article) {//если нашлось совпадение
                string tokens[10];//массив для хранения токенов
                tokens[0] = token;//артикул
                int tokenCount = 1;
                while (getline(ss, token, '|') && tokenCount < 10) {
                    tokens[tokenCount++] = token;
                }

                //нужен для проверки наличия всех 10 полей
                if (tokenCount == 10) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
                break;
            }
        }
        catch (const exception& e) {//пропуск ошибочных строк
            continue;
        }
    }
    file.close();
    DisplayResultCount();
    string historyEntry = "По артикулу: " + to_string(article) + " - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);
}

//поиск по бренду
void Search::SearchByBrand(const std::string& filename, const std::string& brand) {
    ClearResults();
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchBrandLower = toLower(brand);
    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productBrand = tokens[2];
                string productBrandLower = toLower(productBrand);
                if (productBrandLower.find(searchBrandLower) != string::npos) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();
    DisplayResultCount();
    string historyEntry = "По бренду: '" + brand + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);
}

//поиск по стране-производителю
void Search::SearchByCountry(const std::string& filename, const std::string& country) {
    ClearResults();
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchCountryLower = toLower(country);
    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productCountry = tokens[3];
                string productCountryLower = toLower(productCountry);
                if (productCountryLower.find(searchCountryLower) != string::npos) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();
    DisplayResultCount();
    string historyEntry = "По стране: '" + country + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);
}

//поиск по категории
void Search::SearchByCategory(const std::string& filename, const std::string& category) {
    ClearResults();
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchCategoryLower = toLower(category);
    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productCategory = tokens[4];
                string productCategoryLower = toLower(productCategory);
                if (productCategoryLower.find(searchCategoryLower) != string::npos) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();
    DisplayResultCount();
    string historyEntry = "По категории: '" + category + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);
}

//поиск по типу товара
void Search::SearchByType(const std::string& filename, const std::string& type) {
    ClearResults();
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    string searchTypeLower = toLower(type);
    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                string productType = tokens[5];
                string productTypeLower = toLower(productType);
                if (productTypeLower.find(searchTypeLower) != string::npos) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = stoi(tokens[9]);
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();
    DisplayResultCount();
    string historyEntry = "По типу: '" + type + "' - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);
}

//поиск по цене
void Search::SearchByPrice(const std::string& filename, int minPrice, int maxPrice) {
    ClearResults();
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;
    while (getline(file, line) && resultCount < MAX_RESULTS) {
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
                int productPrice = stoi(tokens[9]);
                if (productPrice >= minPrice && productPrice <= maxPrice) {
                    searchResults[resultCount].article = stoi(tokens[0]);
                    searchResults[resultCount].name = tokens[1];
                    searchResults[resultCount].brand = tokens[2];
                    searchResults[resultCount].producerCountry = tokens[3];
                    searchResults[resultCount].category = tokens[4];
                    searchResults[resultCount].typeProduct = tokens[5];
                    searchResults[resultCount].description = tokens[6];
                    searchResults[resultCount].quantityStock = stoi(tokens[7]);
                    searchResults[resultCount].statusStock = (tokens[8] == "1");
                    searchResults[resultCount].price = productPrice;
                    resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();
    DisplayResultCount();
    string historyEntry = "По цене: от " + to_string(minPrice) + " до " + to_string(maxPrice) + " - найдено: " + to_string(resultCount);
    AddToHistory(historyEntry);
}

//универсальный поиск
void Search::UniversalSearch(const std::string& filename) {
    cout << "\n=== УНИВЕРСАЛЬНЫЙ ПОИСК ===" << endl;
    cout << "Выберите критерии поиска (введите номера через запятую):" << endl;
    cout << "1. По названию" << endl;
    cout << "2. По артикулу" << endl;
    cout << "3. По бренду" << endl;
    cout << "4. По стране-производителю" << endl;
    cout << "5. По категории" << endl;
    cout << "6. По типу товара" << endl;
    cout << "7. По цене" << endl;
    cout << "Пример: 2,3 (поиск по артикулу и бренду)" << endl;
    cout << "Ваш выбор: ";

    string criteriaInput;//ввод пользователя сохраняется в переменную
    getline(cin, criteriaInput);

    bool selectedCriteria[7] = { false };//массив из критериев поиска
    int criteriaCount = 0;//счетчик успешно выбранных критериев
    stringstream ss(criteriaInput);//поток из введеной пользователем строки
    string token; // переменная для хранения каждого числа из строки

    while (getline(ss, token, ',')) {//считывает введеную строку в token
        try {
            int criterion = stoi(token);//преобразование строку в число
            if (criterion >= 1 && criterion <= 7) {
                selectedCriteria[criterion - 1] = true;//функция определяет какие критерии выбраны
                criteriaCount++;
            }
        }
        catch (const exception& e) {
        }
    }

    if (criteriaCount == 0) {
        cout << "Не выбрано ни одного критерия поиска!" << endl;
        return;
    }

    // Объявление переменных для хранения поисковых запросов по каждому критерию
    string searchName, searchBrand, searchCountry, searchCategory, searchType;
    int searchArticle = 0;
    int minPrice = 0, maxPrice = 0;// диапазон цен для поиска

    // Цикл по всем 7 возможным критериям поиска
    for (int i = 0; i < 7; i++) {
        // Проверяем, был ли выбран текущий критерий пользователем
        if (selectedCriteria[i]) {
            switch (i + 1) {// switch по номеру критерия (i+1, т.к. массив начинается с 0, а критерии с 1)
            case 1:
                cout << "Введите название для поиска: ";
                getline(cin, searchName);// чтение всей строки названия
                break;
            case 2:
                while (true) {
                    cout << "Введите артикул для поиска: ";
                    cin >> searchArticle;
                    // Проверка на ошибку ввода (если пользователь ввел не число)
                    if (cin.fail()) {
                        cin.clear();
                        cin.ignore(numeric_limits<streamsize>::max(), '\n');// очистка буфера ввода
                        cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                        continue;
                    }
                    break;
                }
                cin.ignore();
                break;
            case 3:
                cout << "Введите бренд для поиска: ";
                getline(cin, searchBrand);
                break;
            case 4:
                cout << "Введите страну-производитель для поиска: ";
                getline(cin, searchCountry);
                break;
            case 5:
                cout << "Введите категорию для поиска: ";
                getline(cin, searchCategory);
                break;
            case 6:
                cout << "Введите тип товара для поиска: ";
                getline(cin, searchType);
                break;
            case 7:
                while (true) {
                    cout << "Введите минимальную цену: ";
                    cin >> minPrice;
                    if (cin.fail()) {
                        cin.clear();
                        cin.ignore(numeric_limits<streamsize>::max(), '\n');// очистка буфера ввода
                        cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                        continue;
                    }
                    break;
                }
                while (true) {
                    cout << "Введите максимальную цену: ";
                    cin >> maxPrice;
                    if (cin.fail()) {
                        cin.clear();
                        cin.ignore(numeric_limits<streamsize>::max(), '\n');
                        cout << "Неверный ввод! Пожалуйста, введите целое число." << endl;
                        continue;
                    }
                    break;
                }
                cin.ignore();// очистка буфера после ввода чисел
                break;
            }
        }
    }

    // Создаем отдельный объект Search для хранения результатов универсального поиска
    Search finalResults;
    ifstream file(filename);// Открываем файл с данными о товарах
    if (!file.is_open()) {
        cout << "Файл " << filename << " не найден!" << endl;
        return;
    }

    string line;// переменная для хранения одной строки из файла
    
    while (getline(file, line) && finalResults.resultCount < MAX_RESULTS) {// Читаем файл построчно, пока есть строки И не превышен лимит результатов
        if (line.empty()) continue;
        stringstream ss(line);
        string token;// переменная для хранения одного поля
        string tokens[10];// массив для хранения всех 10 полей товара
        int tokenCount = 0;// счетчик полей
        while (getline(ss, token, '|') && tokenCount < 10) {
            tokens[tokenCount++] = token;// сохраняем поле в массив
        }
        if (tokenCount == 10) {
            try {
                bool matchesAll = true;// флаг - удовлетворяет ли товар всем критериям
                // Проверяем товар по всем выбранным критериям поиска
                for (int i = 0; i < 7 && matchesAll; i++) {
                    // Если критерий был выбран пользователем
                    if (selectedCriteria[i]) {
                        switch (i + 1) {
                        case 1:
                            // Ищем подстроку в названии товара (регистронезависимо)
                            if (toLower(tokens[1]).find(toLower(searchName)) == string::npos) {
                                matchesAll = false;// не найдено - товар не подходит
                            }
                            break;
                        case 2:
                            if (stoi(tokens[0]) != searchArticle) {//артикул
                                matchesAll = false;
                            }
                            break;
                        case 3:
                            if (toLower(tokens[2]).find(toLower(searchBrand)) == string::npos) {//бренд
                                matchesAll = false;
                            }
                            break;
                        case 4:
                            if (toLower(tokens[3]).find(toLower(searchCountry)) == string::npos) {//страна-производитель
                                matchesAll = false;
                            }
                            break;
                        case 5:
                            if (toLower(tokens[4]).find(toLower(searchCategory)) == string::npos) {//категория
                                matchesAll = false;
                            }
                            break;
                        case 6:
                            if (toLower(tokens[5]).find(toLower(searchType)) == string::npos) {//тип
                                matchesAll = false;
                            }
                            break;
                        case 7:
                            int price = stoi(tokens[9]);
                            if (price < minPrice || price > maxPrice) {//цена
                                matchesAll = false;
                            }
                            break;
                        }
                    }
                }
                // Если товар удовлетворяет ВСЕМ выбранным критериям
                if (matchesAll) {
                    // Сохраняем товар в результаты поиска
                    finalResults.searchResults[finalResults.resultCount].article = stoi(tokens[0]);
                    finalResults.searchResults[finalResults.resultCount].name = tokens[1];
                    finalResults.searchResults[finalResults.resultCount].brand = tokens[2];
                    finalResults.searchResults[finalResults.resultCount].producerCountry = tokens[3];
                    finalResults.searchResults[finalResults.resultCount].category = tokens[4];
                    finalResults.searchResults[finalResults.resultCount].typeProduct = tokens[5];
                    finalResults.searchResults[finalResults.resultCount].description = tokens[6];
                    finalResults.searchResults[finalResults.resultCount].quantityStock = stoi(tokens[7]);
                    finalResults.searchResults[finalResults.resultCount].statusStock = (tokens[8] == "1");
                    finalResults.searchResults[finalResults.resultCount].price = stoi(tokens[9]);
                    finalResults.resultCount++;
                }
            }
            catch (const exception& e) {
                continue;
            }
        }
    }
    file.close();

    cout << "\n=== РЕЗУЛЬТАТЫ УНИВЕРСАЛЬНОГО ПОИСКА ===" << endl;
    if (finalResults.resultCount == 0) {
        cout << "Товары не найдены!" << endl;
    }
    else {
        cout << "Найдено товаров, удовлетворяющих ВСЕМ критериям: " << finalResults.resultCount << endl;
        finalResults.DisplayResultCount(); // выводим количество найденных товаров
        finalResults.DisplayResults();// выводим детальную информацию о найденных товарах
    }
}

int Search::DisplayResultCount() const {
    cout << "\n=== КОЛИЧЕСТВО НАЙДЕННЫХ ТОВАРОВ ===" << endl;
    cout << "Найдено товаров: " << resultCount << endl;
    cout << "=====================================" << endl;
    return resultCount;
}

void Search::DisplayResults() const {
    if (resultCount == 0) {
        cout << "Товары не найдены!" << endl;
        return;
    }

    cout << "\n=== РЕЗУЛЬТАТЫ ПОИСКА ===" << endl;
    cout << "Найдено товаров: " << resultCount << endl;
    for (int i = 0; i < resultCount; i++) {
        cout << "\n--- Товар " << (i + 1) << " ---" << endl;
        cout << "Артикул: " << searchResults[i].article << endl;
        cout << "Название: " << searchResults[i].name << endl;
        cout << "Бренд: " << searchResults[i].brand << endl;
        cout << "Страна: " << searchResults[i].producerCountry << endl;
        cout << "Категория: " << searchResults[i].category << endl;
        cout << "Тип: " << searchResults[i].typeProduct << endl;
        cout << "Описание: " << searchResults[i].description << endl;
        cout << "Количество: " << searchResults[i].quantityStock << endl;
        cout << "Статус: " << (searchResults[i].statusStock ? "В наличии" : "Отсутствует") << endl;
        cout << "Цена: " << searchResults[i].price << endl;
    }
    cout << "==========================" << endl;
}