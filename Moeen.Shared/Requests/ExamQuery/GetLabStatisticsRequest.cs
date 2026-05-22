using System;

namespace Moeen.Shared.Requests.ExamQuery
{
    public class GetLabStatisticsRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}