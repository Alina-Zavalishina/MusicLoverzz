using System;

namespace MusicStore
{
    // Интерфейс для оценки
    public interface IRateable
    {
        int GetRating();
        void SetRating(int rating);
        bool IsRatingValid();
    }

    // Интерфейс для экспорта
    public interface IExportable
    {
        string ExportToString();
        string PrepareForFile();
        void SaveToFile(string filename);
    }

    // Интерфейс для клонирования
    public interface ICloneableReview
    {
        Review Clone();
    }

    // Интерфейс для валидации
    public interface IValidatable
    {
        bool Validate();
    }
}