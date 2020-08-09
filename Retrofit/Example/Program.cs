using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using Retrofit.Net;

namespace Retrofit.Example
{
    public class Program
    {
        // https://github.com/jordan-thoms/Retrofit.Net

        public static void Main(string[] args)
        {
            var obj = new Program();
            obj.GetPeople();
            obj.GetPeople();
        }

        private IRestClient restClient;
        private RestAdapter adapter;
        private IPeopleService client;

        public Program()
        {
            restClient = new RestClient();
            adapter = new RestAdapter(restClient);
            client = adapter.Create<IPeopleService>();
        }

        public List<Person> GetPeople()
        {
            var people = client.GetPeople();
            return people.Data;
        }

        public Person GetPerson()
        {
            var people = client.GetPerson(2);
            return people.Data;
        }

        public Person GetPersonQuery()
        {
            var people = client.GetPerson(2, "blah");
            return people.Data;
        }

        public Person AddPerson()
        {
            var person = new Person { Name = "name_1" };
            var people = client.AddPerson(person);
            return people.Data;
        }

        public Person UpdatePerson()
        {
            var person = new Person { Name = "name_1" };
            var people = client.UpdatePerson(2, person);
            return people.Data;
        }

        public HttpStatusCode HeadPerson()
        {
            var people = client.HeadPerson(2);
            return people.StatusCode;
        }

        public HttpStatusCode DeletePerson()
        {
            var people = client.DeletePerson(2);
            return people.StatusCode;
        }
    }
}
