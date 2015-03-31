using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using KAnonymisation.Core.Hierarchy;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AnonTool.Core.Preprocessing
{
    public class PreprocessColumnVm : UpdateBase
    {
        private int _k;
        private string _header;
        private Type _dataType;
        private IdentifierType _attributeType;
        private IKAnonymisation _selectedAnonymisation;

        public int K
        {
            get { return _k; }
            set
            {
                if(_k != value)
                {
                    _k = value;
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
        public AnonymisationHierarchy AnonymisationHierarchy { get; set; }
        public IKAnonymisation SelectedAnonymisation
        {
            get { return _selectedAnonymisation; }
            set
            {
                if (_selectedAnonymisation != value)
                {
                    _selectedAnonymisation = value;
                    RaisePropertyChanged(() => SelectedAnonymisation);
                }
            }
        }
    }
}
