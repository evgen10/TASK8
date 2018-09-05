using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DAL.Attributes
{
    [AttributeUsage(AttributeTargets.Property) ]
    class NotColumnAttribute: Attribute
    {


    }
}
