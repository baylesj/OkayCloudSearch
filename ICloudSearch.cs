using System.Collections.Generic;
using OkayCloudSearch.Contract;
using OkayCloudSearch.Contract.Result;
using OkayCloudSearch.Query;

namespace OkayCloudSearch
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