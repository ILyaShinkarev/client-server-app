using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    /// <summary>
    /// Модель записи, хранящейся в базе данных.
    /// </summary>
    /// <remarks>
    /// Содержит идентификатор, дату создания (UTC) и текстовое сообщение.  
    /// </remarks>
    public class Record
    {
        /// <summary>
        /// Уникальный идентификатор записи.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Время создания записи в формате UTC.
        /// </summary>
        /// <remarks>
        /// Значение назначается автоматически при генерации записи фоновым сервисом.
        /// </remarks>
        /// <example>2025-10-27T14:30:00Z</example>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Краткое сообщение, ограниченное 10 символами.
        /// </summary>
        /// <remarks>
        /// Поле обязательно для заполнения и имеет максимальную длину 10 символов.
        /// </remarks>
        /// <example>"1234567890"</example>
        [Required]
        [MaxLength(10)]
        public string Message { get; set; } = string.Empty;
    }
}
