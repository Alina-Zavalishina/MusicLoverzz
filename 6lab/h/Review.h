#ifndef REVIEW_HPP
#define REVIEW_HPP

#include <string>
#include <iostream>
#include <ctime>
#include <memory>

class Review {
protected:
    std::string idReview;           // уникальный идентификатор отзыва
    std::string clientId;           // идентификатор клиента
    int rating;                     // оценка (1-5)
    std::string comment;            // текстовый комментарий
    std::string dateReview;         // дата создания отзыва
    bool isVerified;                // проверен ли отзыв (админом)
    int helpfulVotes;               // количество "полезно"
    int unhelpfulVotes;             // количество "бесполезно"

public:
    // Конструктор
    Review(const std::string& clientId = "", int rating = 0,
        const std::string& comment = "", bool verified = false,
        const std::string& id = "", const std::string& date = "");

    // Виртуальный деструктор 
    virtual ~Review() {}


    // Получить тип отзыва 
    virtual std::string getReviewType() const = 0;

    // Отобразить детали отзыва 
    virtual void displayDetails() const = 0;

    // Получить предмет отзыва 
    virtual std::string getSubject() const = 0;

    // Проверить валидность отзыва 
    virtual bool validate() const = 0;

    // Клонирование отзыва 
    virtual Review* clone() const = 0;


    // Рассчитать вес отзыва для рейтинга
    virtual double calculateWeight() const;

    // Подготовить данные для записи в файл
    virtual std::string prepareForFile() const;

    // Экспортировать в строку
    virtual std::string exportToString() const;


    // Сохранить в файл
    void SaveToFile(const std::string& filename = "reviews.txt") const;

    // Статический метод для отображения всех отзывов
    static void DisplayAllReviews(const std::string& filename = "reviews.txt");

    // Статический метод для расчета средней оценки
    static double AverageRating(const std::string& productName);

    // Методы оценки полезности отзыва
    void markAsHelpful();
    void markAsUnhelpful();
    double getHelpfulnessRatio() const;

    //  ГЕТТЕРЫ 
    std::string getIdReview() const { return idReview; }
    std::string getClientId() const { return clientId; }
    int getRating() const { return rating; }
    std::string getComment() const { return comment; }
    std::string getDateReview() const { return dateReview; }
    bool getIsVerified() const { return isVerified; }
    int getHelpfulVotes() const { return helpfulVotes; }
    int getUnhelpfulVotes() const { return unhelpfulVotes; }

    // СЕТТЕРЫ
    void setIdReview(const std::string& id) { idReview = id; }
    void setDateReview(const std::string& date) { dateReview = date; }
    void setRating(int r) {
        if (r >= 1 && r <= 5) rating = r;
    }
    void setComment(const std::string& com) { comment = com; }
    void verifyReview() { isVerified = true; }
    void unverifyReview() { isVerified = false; }
    void setClientId(const std::string& id) { clientId = id; }
    void setHelpfulVotes(int votes) { helpfulVotes = votes; }
    void setUnhelpfulVotes(int votes) { unhelpfulVotes = votes; }

    //  СТАТИЧЕСКИЕ МЕТОДЫ
    static std::string generateReviewId(const std::string& prefix = "REV");
    static std::string getCurrentDate();

protected:
    // Вспомогательные методы
    int generateRandomNumber() const;
};

#endif
#pragma once
