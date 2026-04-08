using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestData.Models.Viewmodel
{
    public class ChatRequest
    {
        public string Message { get; set; }
    }
    public class LlamaResponse
    {
        public string Response { get; set; }
        public bool Done { get; set; }
    }
    public class LlamaChunk
    {
        public string Model { get; set; }
        public string Response { get; set; }
        public bool Done { get; set; }
    }
}
