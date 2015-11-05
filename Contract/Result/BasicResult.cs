using System.Collections.Generic;

namespace OkayCloudSearch.Contract.Result
{
    public class Error
    {
        public string message { get; set; }
    }

    public class BasicResult
    {
        public bool IsError { get; set; }

        public string status { get; set; }
        public IEnumerable<Error> errors { get; set; }
        public int adds { get; set; }
        public int deletes { get; set; }
    }
}
