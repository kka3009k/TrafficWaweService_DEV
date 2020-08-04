using System.Runtime.Serialization;

namespace TrafficWaveService.Client
{
    /// <summary>
    /// Параметры передаваемых и возвращаемых полей поиска
    /// </summary>
    [DataContract]
    public class ClientQuery
    {
        /// <summary>
        /// Строка запроса для информации по клиенту
        /// </summary>
        [DataMember]
        public string RequestStringClient { get; set; }

        /// <summary>
        /// Строка запроса для доп. информации
        /// </summary>
        [DataMember]
        public string RequestStringDopInfo { get; set; }

        ///// <summary>
        ///// Фамилия
        ///// </summary>
        //[DataMember]
        //public string FirstName { get; set; }

        ///// <summary>
        ///// Имя
        ///// </summary>
        //[DataMember]
        //public string LastName { get; set; }

        ///// <summary>
        ///// Отчество
        ///// </summary>
        //[DataMember]
        //public string SecondName { get; set; }

        /// <summary>
        /// Резульатат
        /// </summary>
        [DataMember]
        public Result Error = new Result();
    }
}