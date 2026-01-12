#ifndef USER_HPP
#define USER_HPP

#include <string>
#include <iostream>
#include <ctime>

class User {
protected:
    std::string id;
    std::string name;
    std::string email;
    std::string phone;
    std::string registrationDate;

public:
    // Конструкторы
    User();
    User(const std::string& id, const std::string& name,
        const std::string& email = "", const std::string& phone = "");

    // Конструктор копирования
    User(const User& other);

    // виртуальный деструктор
    virtual ~User();

    // Виртуальная функция для переопределения
    virtual void displayInfo() const;

    void showBasicInfo() const;

    // Виртуальный метод для расчета скидки
    virtual double calculateDiscount() const;

    // Перегрузка операторов
    bool operator==(const User& other) const;
    bool operator!=(const User& other) const;
    User& operator=(const User& other);

    // Геттеры
    std::string getId() const { return id; }
    std::string getName() const { return name; }
    std::string getEmail() const { return email; }
    std::string getPhone() const { return phone; }
    std::string getRegistrationDate() const { return registrationDate; }

    // Сеттеры
    void setName(const std::string& newName) { name = newName; }
    void setEmail(const std::string& newEmail) { email = newEmail; }
    void setPhone(const std::string& newPhone) { phone = newPhone; }

    // Статический метод для генерации ID
    static std::string generateUserId(const std::string& prefix);

protected:
    // Protected метод для инициализации
    void initializeUser(const std::string& id, const std::string& name,
        const std::string& email, const std::string& phone);
};

// Глобальный оператор вывода
std::ostream& operator<<(std::ostream& os, const User& user);

#endif