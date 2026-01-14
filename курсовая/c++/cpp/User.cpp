#define _CRT_SECURE_NO_WARNINGS
#include "User.h"
#include <ctime>
#include <sstream>
#include <iomanip>
#include <iostream>

using namespace std;

// Конструктор по умолчанию
User::User() : id(""), name(""), email(""), phone(""), registrationDate("") {
    if (id.empty()) {
        id = generateUserId("USR");
    }
}

// Конструктор с параметрами
User::User(const std::string& id, const std::string& name,
    const std::string& email, const std::string& phone)
    : id(id.empty() ? generateUserId("USR") : id),
    name(name), email(email), phone(phone) {

    time_t now = time(0);
    tm* localTime = localtime(&now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", localTime);
    registrationDate = buffer;
}

// Конструктор копирования
User::User(const User& other)
    : id(other.id + "_COPY"), name(other.name),
    email(other.email), phone(other.phone),
    registrationDate(other.registrationDate) {

    if (id.find("_COPY") == string::npos) {
        id += "_COPY";
    }
}

// ВИРТУАЛЬНЫЙ деструктор
User::~User() {
}

// Виртуальная функция displayInfo
void User::displayInfo() const {
    cout << *this;
}

// Невиртуальная функция
void User::showBasicInfo() const {
    cout << "\n=== БАЗОВАЯ ИНФОРМАЦИЯ ===" << endl;
    cout << "ID: " << id << endl;
    cout << "Имя: " << name << endl;
    cout << "Email: " << (email.empty() ? "не указан" : email) << endl;
    cout << "Телефон: " << (phone.empty() ? "не указан" : phone) << endl;
    cout << "Дата регистрации: " << registrationDate << endl;
    cout << "================================" << endl;
}

// Базовая реализация метода для расчета скидки
double User::calculateDiscount() const {
    return 0.0;
}

// Оператор сравнения
bool User::operator==(const User& other) const {
    return id == other.id && email == other.email;
}

bool User::operator!=(const User& other) const {
    return !(*this == other);
}

// Оператор присваивания
User& User::operator=(const User& other) {
    if (this != &other) {
        id = other.id + "_ASSIGNED";
        name = other.name;
        email = other.email;
        phone = other.phone;
    }
    return *this;
}

// Статический метод для генерации ID
std::string User::generateUserId(const std::string& prefix) {
    static int counter = 0;
    stringstream ss;
    ss << prefix << setw(4) << setfill('0') << ++counter;
    return ss.str();
}

// Protected метод для инициализации
void User::initializeUser(const std::string& id, const std::string& name,
    const std::string& email, const std::string& phone) {

    this->id = id.empty() ? generateUserId("USR") : id;
    this->name = name;
    this->email = email;
    this->phone = phone;

    time_t now = time(0);
    tm* localTime = localtime(&now);
    char buffer[80];
    strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", localTime);
    registrationDate = buffer;
}

// Глобальный оператор вывода
std::ostream& operator<<(std::ostream& os, const User& user) {
    os << "\n=== ИНФОРМАЦИЯ О ПОЛЬЗОВАТЕЛЕ ===" << endl;
    os << "ID: " << user.getId() << endl;
    os << "Имя: " << user.getName() << endl;
    os << "Email: " << (user.getEmail().empty() ? "не указан" : user.getEmail()) << endl;
    os << "Телефон: " << (user.getPhone().empty() ? "не указан" : user.getPhone()) << endl;
    os << "Дата регистрации: " << user.getRegistrationDate() << endl;
    os << "Тип: Базовый пользователь" << endl;
    os << "================================" << endl;
    return os;
}