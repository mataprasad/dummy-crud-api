using System.Collections.Generic;
using RestSharp;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace Retrofit.Example
{
    public interface IPeopleService
    {
        [Get("people")]
        RestResponse<List<Person>> GetPeople();

        [Get("people/{id}")]
        RestResponse<Person> GetPerson([Path("id")] int id);

        [Get("people/{id}")]
        RestResponse<Person> GetPerson([Path("id")] int id, [Query("q")] string query);

        [Post("people")]
        RestResponse<Person> AddPerson([Body] Person person);

        [Put("people/{id}")]
        RestResponse<Person> UpdatePerson([Path("id")] int id, [Body] Person person);

        [Head("people/{id}")]
        RestResponse<Person> HeadPerson([Path("id")] int id);

        [Delete("people/{id}")]
        RestResponse<Person> DeletePerson([Path("id")] int id);
    }
}
