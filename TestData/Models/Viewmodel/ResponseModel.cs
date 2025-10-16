namespace Testdata.Viewmodel
{
    public class ResponseModel<T>
    {
        public bool Status { get; set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public Exception Exception { get; set; }

        public ResponseModel()
        {
            Status = true;
        }
    }
}
