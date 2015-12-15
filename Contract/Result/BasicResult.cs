using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OkayCloudSearch.Contract.Result
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Error
    {
        public string message { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class BasicResult
    {
        public bool IsError { get; set; }

        public string status { get; set; }
        public IEnumerable<Error> errors { get; set; }
        public int adds { get; set; }
        public int deletes { get; set; }
    }
}
