﻿using Devinno.Skia.Containers;
using Devinno.Skia.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Collections
{
    public class DvControlCollection : Dictionary<string, DvControl>
    {
        #region Member Variable
        protected IDvContainer Parent;
        #endregion

        #region Event
        public event EventHandler Changed;
        #endregion

        #region Constructor
        public DvControlCollection(IDvContainer Parent)
        {
            this.Parent = Parent;
        }
        #endregion

        #region Method
        #region Add
        private new void Add(string key, DvControl value)
        {
            if (value != null)
            {
                value.ParentContainer = Parent;

                base.Add(key, value);
                InvokeChanged();
            }
            else throw new Exception("컨트롤이 null 일 수 없습니다.");
        }

        public void Add(DvControl value)
        {
            if (value != null)
            {
                value.ParentContainer = Parent;

                if (value.Name != null)
                {
                    if (!this.ContainsKey(value.Name)) Add(value.Name, value);
                    else throw new Exception("동일한 이름의 컨트롤이 존재합니다.");
                }
                else throw new Exception("컨트롤 이름은 null 일 수 없습니다.");
            }
            else throw new Exception("컨트롤이 null 일 수 없습니다.");
        }
        #endregion
        #region Remove
        public void Remove(DvControl value)
        {
            if (value != null)
            {
                if (ContainsKey(value.Name))
                {
                    Remove(value.Name);
                    InvokeChanged();
                }
            }
        }
        #endregion
        #region Rename
        public bool Rename(DvControl target, string NewName)
        {
            var ret = false;
            if (!ContainsKey(NewName))
            {
                target.Name = NewName;
                var ls = Values.ToList();

                Clear();
                foreach (var v in ls) Add(v);
            }
            return ret;
        }
        #endregion
        #region Reset
        public void Reset()
        {
            foreach (var vk in Keys)
                this[vk].ParentContainer = Parent;
        }
        #endregion

        #region InvokeChanged
        protected void InvokeChanged() => Changed?.Invoke(this, null);
        #endregion
        #endregion
    }

    public class DvTableLayoutControlCollection : DvControlCollection
    {
        #region Constructor
        public DvTableLayoutControlCollection(DvTableLayoutPanel Parent) : base(Parent)
        {

        }
        #endregion

        #region Add
        internal new void Add(string key, DvControl value) => base.Add(key, value);
        internal new void Add(DvControl c) => base.Add(c);

        public void Add(DvControl control, int column, int row, int colspan=1, int rowspan=1)
        {
            if (column < -1) throw new ArgumentException("column");
            if (row < -1) throw new ArgumentException("row");

            var panel = (DvTableLayoutPanel)Parent;
            panel.SetCellPosition(control, column, row, colspan, rowspan);

            base.Add(control);

            InvokeChanged();
        }
        #endregion
    }

    public class DvGridLayoutControlCollection : DvControlCollection
    {
        #region Constructor
        public DvGridLayoutControlCollection(DvGridLayoutPanel Parent) : base(Parent)
        {

        }
        #endregion

        #region Add
        internal new void Add(string key, DvControl value) => base.Add(key, value);
        internal new void Add(DvControl c) => base.Add(c);

        public void Add(DvControl control, int column, int row)
        {
            if (column < -1) throw new ArgumentException("column");
            if (row < -1) throw new ArgumentException("row");
           
            var panel = (DvGridLayoutPanel)Parent;
            panel.SetCellPosition(control, column, row);
           
            base.Add(control);

            InvokeChanged();
        }
        #endregion
    }


}
