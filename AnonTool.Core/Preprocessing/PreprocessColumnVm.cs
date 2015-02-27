using AnonTool.MVVM.Updates;
using KAnonymisation.Core.IdentifierTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.Preprocessing
{
    public class PreprocessColumnVm : UpdateBase
    {
        private int _k = 3;
        private string _header;
        private Type _dataType;
        private IdentifierType _attributeType;

        public int K
        {
            get { return _k; }
            set
            {
                if(_k != value)
                {
                    K = _k;
                    RaisePropertyChanged(() => K);
                }
            }
        }
        public string Header
        {
            get { return _header; }
            set
            {
                if(_header != value)
                {
                    _header = value;
                    RaisePropertyChanged(() => Header);
                }
            }
        }
        public Type DataType
        {
            get { return _dataType; }
            set
            {
                if(_dataType != value)
                {
                    _dataType = value;
                    RaisePropertyChanged(() => DataType);
                }
            }
        }
        public IdentifierType AttributeType
        {
            get { return _attributeType; }
            set
            {
                if(_attributeType != value)
                {
                    _attributeType = value;
                    RaisePropertyChanged(() => AttributeType);
                }
            }
        }

    }
}
