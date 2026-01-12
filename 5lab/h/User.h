#ifndef USER_HPP
#define USER_HPP

#include <string>
#include <iostream>

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
        const std::string& email = "", const std::string& phone = "",
        const std::string& regDate = "");

    // Конструктор копирования
    User(const User& other);

    // Деструктор
    virtual ~User();

    // Виртуальная функция
    virtual void displayInfo() const;

    //ПЕРЕГРУЗКА ОПЕРАТОРОВ 
    // Операторы сравнения
    bool operator==(const User& other) const;
    bool operator!=(const User& other) const;
    bool operator<(const User& other) const;
    bool operator>(const User& other) const;
    bool operator<=(const User& other) const;
    bool operator>=(const User& other) const;

    // Оператор присваивания
    User& operator=(const User& other);

    // Оператор += для добавления контактов
    User& operator+=(const std::string& newPhone);

    // Унарный оператор !
    bool operator!() const;

    // Оператор индекса []
    char operator[](size_t index) const;

    // Оператор вызова функции ()
    void operator()(const std::string& message) const;


    void showBasicInfo() const;

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

    // Статический метод
    static std::string generateUserId(const std::string& prefix);

protected:
    void initializeUser(const std::string& id, const std::string& name,
        const std::string& email, const std::string& phone,
        const std::string& regDate);
};

// Глобальные операторы
std::ostream& operator<<(std::ostream& os, const User& user);
std::istream& operator>>(std::istream& is, User& user);
User operator+(const User& u1, const User& u2);

#endif
