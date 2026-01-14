#define _CRT_SECURE_NO_WARNINGS
#include "User.h"
#include <ctime>
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <iostream>

using namespace std;


// Конструктор по умолчанию
User::User() : id(""), name(""), email(""), phone(""), registrationDate("") {}

// Конструктор с параметрами
User::User(const std::string& id, const std::string& name,
    const std::string& email, const std::string& phone,
    const std::string& regDate)
    : id(id), name(name), email(email), phone(phone) {

    if (regDate.empty()) {
        time_t now = time(0);
        tm* localTime = localtime(&now);
        char buffer[80];
        strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", localTime);
        registrationDate = buffer;
    }
    else {
        registrationDate = regDate;
    }
}

// Конструктор копирования
User::User(const User& other)
    : id(other.id), name(other.name), email(other.email),
    phone(other.phone), registrationDate(other.registrationDate) {}

// Деструктор
User::~User() {}

// Оператор == (сравнение по ID)
bool User::operator==(const User& other) const {
    return id == other.id;
}

// Оператор !=
bool User::operator!=(const User& other) const {
    return !(*this == other);
}

// Оператор < (сортировка по имени)
bool User::operator<(const User& other) const {
    return name < other.name;
}

// Оператор >
bool User::operator>(const User& other) const {
    return name > other.name;
}

// Оператор <=
bool User::operator<=(const User& other) const {
    return name <= other.name;
}

// Оператор >=
bool User::operator>=(const User& other) const {
    return name >= other.name;
}

// Оператор присваивания
User& User::operator=(const User& other) {
    if (this != &other) {
        id = other.id;
        name = other.name;
        email = other.email;
        phone = other.phone;
        registrationDate = other.registrationDate;
    }
    return *this;
}

// Оператор += для добавления телефона
User& User::operator+=(const std::string& newPhone) {
    if (!phone.empty()) {
        phone += "; " + newPhone;
    }
    else {
        phone = newPhone;
    }
    return *this;
}

// Унарный оператор ! (проверка, заполнены ли основные поля)
bool User::operator!() const {
    return name.empty() || id.empty();
}

// Оператор индекса []
char User::operator[](size_t index) const {
    if (index < name.length()) {
        return name[index];
    }
    return '\0';
}

// Оператор вызова функции ()
void User::operator()(const std::string& message) const {
    cout << name << " говорит: \"" << message << "\"" << endl;
}


// Виртуальная функция displayInfo
void User::displayInfo() const {
    cout << *this;
}

void User::showBasicInfo() const {
    cout << "\n=== БАЗОВАЯ ИНФОРМАЦИЯ ===" << endl;
    cout << "ID: " << id << endl;
    cout << "Имя: " << name << endl;
    cout << "Email: " << (email.empty() ? "не указан" : email) << endl;
    cout << "Телефон: " << (phone.empty() ? "не указан" : phone) << endl;
    cout << "Дата регистрации: " << registrationDate << endl;
    cout << "================================" << endl;
}

std::string User::generateUserId(const std::string& prefix) {
    static int counter = 0;
    stringstream ss;
    ss << prefix << setw(4) << setfill('0') << ++counter;
    return ss.str();
}

void User::initializeUser(const std::string& id, const std::string& name,
    const std::string& email, const std::string& phone,
    const std::string& regDate) {
    this->id = id;
    this->name = name;
    this->email = email;
    this->phone = phone;

    if (regDate.empty()) {
        time_t now = time(0);
        tm* localTime = localtime(&now);
        char buffer[80];
        strftime(buffer, 80, "%d.%m.%Y %H:%M:%S", localTime);
        this->registrationDate = buffer;
    }
    else {
        this->registrationDate = regDate;
    }
}

//ГЛОБАЛЬНЫЕ ОПЕРАТОРЫ

// Оператор вывода
std::ostream& operator<<(std::ostream& os, const User& user) {
    os << "\n=== ИНФОРМАЦИЯ О ПОЛЬЗОВАТЕЛЕ ===" << endl;
    os << "ID: " << user.getId() << endl;
    os << "Имя: " << user.getName() << endl;
    os << "Email: " << (user.getEmail().empty() ? "не указан" : user.getEmail()) << endl;
    os << "Телефон: " << (user.getPhone().empty() ? "не указан" : user.getPhone()) << endl;
    os << "Дата регистрации: " << user.getRegistrationDate() << endl;
    os << "================================" << endl;
    return os;
}

// Оператор ввода
std::istream& operator>>(std::istream& is, User& user) {
    string name, email, phone;

    cout << "Введите имя: ";
    is.ignore();
    getline(is, name);

    cout << "Введите email: ";
    getline(is, email);

    cout << "Введите телефон: ";
    getline(is, phone);

    user.setName(name);
    user.setEmail(email);
    user.setPhone(phone);

    return is;
}

// Оператор + для объединения пользователей
User operator+(const User& u1, const User& u2) {
    string newId = u1.getId() + "_" + u2.getId();
    string newName = u1.getName() + " & " + u2.getName();
    string newEmail = u1.getEmail() + "; " + u2.getEmail();

    return User(newId, newName, newEmail);
}
