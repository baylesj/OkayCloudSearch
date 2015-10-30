using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace OkayCloudSearch.Contract
{
    public class SearchDocument
    {
        public SearchDocument() //constructor
        {
        }

        [ScriptIgnore]
        public string id { get; set; }
    }
}