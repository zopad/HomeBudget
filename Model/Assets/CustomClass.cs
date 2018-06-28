using HomeBudget.ViewModel.TypeEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Model.Assets
{
    [System.Serializable()]
    [CategoryAttribute("Saját kiadások")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CustomClass : CollectionBase, ICustomTypeDescriptor, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        

        public void Add(CustomProperty Value)
        {
            base.List.Add(Value);
        }

        public void Remove(string Name)
        {
            foreach (CustomProperty prop in base.List)
            {
                if (prop.Name == Name)
                {
                    base.List.Remove(prop);
                    return;
                }
            }
        }

        private void expandable_propertiesChanged(PropertiesChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.propName));
        }

        //Indexer
        public CustomProperty this[int index]
        {
            get
            {
                return (CustomProperty)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }

        #region "TypeDescriptor Implementation"
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] newProps = new PropertyDescriptor[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                CustomProperty prop = (CustomProperty)this[i];
                newProps[i] = new CustomPropertyDescriptor(ref prop, attributes);
            }

            return new PropertyDescriptorCollection(newProps);
        }

        public List<CustomProperty> GetAllProps()
        {
            var ret = new List<CustomProperty>();
            for (int i = 0; i < this.Count; i++)
            {
                CustomProperty prop = (CustomProperty)this[i];
                ret.Add(prop);
            }
            return ret;
        }

        private decimal totalValue;
        [ReadOnlyAttribute(true), Browsable(true), DisplayName("Saját kiadások összesen")]
        public decimal TotalValue
        {
            get
            {
                totalValue = calcTotal();
                return totalValue;
            }
            set
            {
                totalValue = calcTotal();
                TotalValues.Collection.Single(x => x.Name == "Custom").TotalValue = totalValue;
            }
        }

        private decimal calcTotal()
        {
            decimal sum = 0;
            for (int i = 0; i < this.Count; i++)
            {
                CustomProperty prop = this[i];
                sum += prop.Value;
            }
            return sum;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        internal void updateSum()
        {
            TotalValue = calcTotal();
        }
        #endregion
    }
    [CategoryAttribute("Saját kiadások"), RefreshProperties(RefreshProperties.All), Editor(typeof(TypeEditor12), typeof(UITypeEditor))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [System.Serializable()]
    public class CustomProperty
    {
        private string name = string.Empty;
        private decimal objValue = 0;
        private CustomClass customClass;

        public CustomProperty(string name, decimal value, Type type, CustomClass customClass)
        {
            this.name = name;
            this.objValue = value;
            this.type = type;
            this.customClass = customClass;
        }

        private Type type;
        public Type Type
        {
            get { return type; }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public decimal Value
        {
            get
            {
                return objValue;
            }
            set
            {
                objValue = value;
                customClass.updateSum();
            }
        }
    }

    /// <summary>
    /// Custom PropertyDescriptor
    /// </summary>
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        CustomProperty m_Property;
        public CustomPropertyDescriptor(ref CustomProperty myProperty, Attribute[] attrs) : base(myProperty.Name, attrs)
        {
            m_Property = myProperty;
        }

        #region PropertyDescriptor specific

        public override string ToString()
        {
            return base.Name;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override object GetValue(object component)
        {
            return m_Property.Value;
        }

        public override string Description
        {
            get { return m_Property.Name; }
        }

        public override string Category
        {
            get { return "Saját kategória"; }
        }

        public override string DisplayName
        {
            get { return m_Property.Name; }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override void ResetValue(object component)
        {
            //Have to implement
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override void SetValue(object component, object value)
        {
            m_Property.Value = (decimal)value;
        }

        public override Type PropertyType
        {
            get { return m_Property.Type; }
        }

        #endregion
    }

}
