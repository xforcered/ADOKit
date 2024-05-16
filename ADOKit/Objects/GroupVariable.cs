using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOKit.Objects
{
    class GroupVariable
    {

        public string name { get; set; }
        public string value { get; set; }
        public string groupName { get; set; }

        public GroupVariable(string name, string value, string groupName)
        {
            this.name = name;
            this.value = value;
            this.groupName = groupName;
        }


    }
}
