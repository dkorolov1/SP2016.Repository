using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP2016.Repository.Caml.ChainedBuilder
{
    public class ChainedCamlBuilder
    {
        public FieldExpression Where()
        {
            return new FieldExpression(this);
        }
    }
}
