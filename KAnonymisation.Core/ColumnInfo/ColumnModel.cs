using KAnonymisation.Core.Hierarchy;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAnonymisation.Core.ColumnInfo
{
    public class ColumnModel
    {
        private int _k = 0;
        private string _header;
        private Type _dataType;
        private IdentifierType _attributeType;
        private IKAnonymisation _kAnonymisation;

        public int K
        {
            get { return _k; }
            set
            {
                if (_k != value)
                {
                    _k = value;
                }
            }
        }
        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                }
            }
        }
        public Type DataType
        {
            get { return _dataType; }
            set
            {
                if (_dataType != value)
                {
                    _dataType = value;
                }
            }
        }
        public IdentifierType AttributeType
        {
            get { return _attributeType; }
            set
            {
                if (_attributeType != value)
                {
                    _attributeType = value;
                }
            }
        }
        public ColumnHierarchy ColumnHierarchy { get; set; }
        public IKAnonymisation KAnonymisation
        {
            get { return _kAnonymisation; }
            set
            {
                if(_kAnonymisation != value)
                {
                    _kAnonymisation = value;
                }
            }
        }
    }
}
