using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Infrastructure.DataLoading
{
    public class DataMapper
    {
        private List<string> _headers;
        private List<List<string>> _fields;
        private List<string> _types = new List<string>();

        public List<string> Headers
        {
            get
            {
                return _headers;
            }
            set
            {
                _headers = value;
            }
        }

        public List<List<string>> Fields
        {
            get
            {
                return _fields;
            }
            set
            {
                _fields = value;
            }
        }

        public List<string> Types
        {
            get
            {
                return _types;
            }
            set
            {
                _types = value;
            }
        }

        //relational data class
        public DataMapper(List<string> inputHeaders, List<List<string>> inputFields)
        {
            _headers = inputHeaders;
            _fields = inputFields;
        }

        public void Describe()
        {
            var desc = string.Join(",", _headers);
            Console.WriteLine(desc);
        }

        public void AlterTypes()
        {
            var count = 0;
            Console.WriteLine("string, int, double, long");

            foreach (var head in _headers)
            {
                Console.Write("[{0}] -> {1}... set data type: ", count++, head);
                _types.Add(Console.ReadLine());
            }

        }
    }
}
