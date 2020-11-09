using System;
using SqlKata;

namespace DummyCrudApi.Models
{
    public class ModelBase
    {
        [Ignore]
        public string _key { get; set; }
    }
}
