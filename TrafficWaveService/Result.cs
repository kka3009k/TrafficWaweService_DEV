using System.Runtime.Serialization;

namespace TrafficWaveService
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}