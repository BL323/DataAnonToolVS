using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.MVVM.Updates
{
    public class UpdateBase : INotifyPropertyChanged
    {
        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                return;

            var body = expression.Body as MemberExpression;
            if (body != null)
                OnPropertyChanged(body.Member.Name);
        }
    }
}
