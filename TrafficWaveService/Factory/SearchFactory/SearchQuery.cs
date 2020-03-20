using System.Runtime.Serialization;

namespace TrafficWaveService.Factory.SearchFactory
{
    /// <summary>
    /// Параметры передаваемых и возвращаемых полей поиска
    /// </summary>
    [DataContract]
    public class SearchQuery
    {
        /// <summary>
        /// Id запроса
        /// </summary>
        [DataMember]
        public long QueryId { get; set; }

        /// <summary>
        /// Id заявки
        /// </summary>
        [DataMember]
        public long RequestId { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [DataMember]
        public string SecondName { get; set; }

        /// <summary>
        /// Резульатат
        /// </summary>
        [DataMember]
        public Result Error = new Result();
    }
}