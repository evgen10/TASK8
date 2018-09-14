using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Attributes
{
    /// <summary>
    /// Атрибут указывает, что свойство является ключевым
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]    
    public class KeyAttribute: Attribute
    {
    }
}
