#ifndef REVIEW_HPP
#define REVIEW_HPP

#include <string>
#include <fstream>
#include <iostream>

class Review {
private:
    std::string idReview;// уникальный идентификатор отзыва
    std::string productName;// название товара, к которому относится отзыв
    std::string clientId;// идентификатор клиента, оставившего отзыв
    int rating; // оценка товара (например, от 1 до 5)
    std::string comment;// текстовый комментарий к отзыву
    std::string dateReview;// дата создания отзыва

public:
    Review();
    Review(const std::string& product, const std::string& client,
        int rating, const std::string& comment);

    void SaveToFile();
    static void PublicReview();  //показывает все отзывы из файла
    static double AverageRating(const std::string& productName);   // автоматический расчет среднего рейтинга товара

    // методы доступа к приватным полям
    std::string getIdReview() const { return idReview; }
    std::string getProductName() const { return productName; }
    std::string getClientId() const { return clientId; }
    int getRating() const { return rating; }
    std::string getComment() const { return comment; }
    std::string getDateReview() const { return dateReview; }

    // ДОБАВЛЕННЫЕ СЕТТЕРЫ (для установки значений полей)
    void setProductName(const std::string& name) { productName = name; }
    void setClientId(const std::string& id) { clientId = id; }
    void setRating(int r) {
        if (r >= 1 && r <= 5) rating = r;
        else rating = 5; // значение по умолчанию при некорректном вводе
    }
    void setComment(const std::string& com) { comment = com; }

private:
    // Приватные вспомогательные методы
    std::string generateReviewId();// генерация уникального ID отзыва
    std::string getCurrentDate();// получение текущей даты
};

#endif
