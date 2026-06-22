namespace Moeen.Shared.Responses.Mosuq
{
    public class GetAllMosqusResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<MosquDto> Mosqus { get; set; } = new List<MosquDto>();
    }
    public class MosquDto
    {
        public Guid Id { get; set; }
        public string name { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string contactPhone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
