using System.Collections.Generic;
using AmazingCloudSearch.Contract;
using AmazingCloudSearch.Contract.Result;
using AmazingCloudSearch.Query;

namespace AmazingCloudSearch
{
    public interface ICloudSearch<T> where T : SearchDocument, new()
    {
        AddResult Add(List<T> toAdd);
        AddResult Add(T toAdd);
        UpdateResult Update(T toUpdate);
        DeleteResult Delete(SearchDocument toDelete);
        SearchResult<T> Search(SearchQuery<T> query);
        SearchResult<T> SearchWithException(SearchQuery<T> query);
    }
}