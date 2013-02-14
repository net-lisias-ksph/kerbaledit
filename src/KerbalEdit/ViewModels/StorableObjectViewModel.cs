// -----------------------------------------------------------------------
// <copyright file="StorableObjectViewModel.cs" company="OpenSauceSolutions">
// � 2013 OpenSauce Solutions
// </copyright>
// -----------------------------------------------------------------------

namespace KerbalEdit.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using KerbalData;
    using KerbalData.Models;

    /// <summary>
    /// TODO: Class Summary
    /// </summary>
    public class StorableObjectViewModel<T> : KerbalDataObjectViewModel where T : class, IStorable, new()
    {
        private bool childrenLoaded;

        //private T obj;
        /// <summary>
        /// Initializes a new instance of the <see cref="StorableObjectViewModel" /> class.
        /// </summary>	
        public StorableObjectViewModel(string displayName, StorableObjectsViewModel<T> parent) : base(displayName, parent)
        {
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                LoadChildren();
                base.IsSelected = value;
            }
        }

        public override IKerbalDataObject Object
        {
            get
            {
                if (base.Object == null)
                {
                    Object = ((StorableObjectsViewModel<T>)Parent).Objects[DisplayName];
                }

                return base.Object;
            }
            protected set
            {
                base.Object = value;
            }
        }

        protected override void LoadChildren()
        {
            if (childrenLoaded)
            {
                return;
            }

            RemoveDummyChild();

            foreach (var prop in Object.GetType().GetProperties())
            {
                if (prop.PropertyType.GetInterfaces().Any(i => i.FullName.Contains("IKerbalDataObject")))
                {
                    var obj = (IKerbalDataObject)prop.GetValue(Object);

                    if (obj != null)
                    {
                        Children.Add(new KerbalDataObjectViewModel(this, obj));
                    }
                }

                if (prop.PropertyType.IsGenericType && (prop.PropertyType.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    var val = prop.GetValue(Object);
                    if (val != null && val.GetType().GetGenericArguments()[0].GetInterfaces().Contains(typeof(IKerbalDataObject)))
                    {
                        Children.Add(new KerbalDataObjectListViewModel(prop.Name, this, (ICollection)prop.GetValue(Object)));
                    }
                }
            }

            childrenLoaded = true;
        }
    }
}
