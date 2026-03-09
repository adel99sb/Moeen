namespace Moeen.Api.Shared.Responses.Mosuq
{
    public class GetAllMosqusResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<MosquDto> Mosqus { get; set; }
    }
    public class MosquDto
    {
        public string name { get; set; }
        public string address { get; set; }
        public string contact_phone { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
