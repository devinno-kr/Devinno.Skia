using Devinno.Skia.Containers;
using Devinno.Skia.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Collections
{
    public class DvSubPageCollection : Dictionary<string, DvSubPage>
    {
        #region Member Variable
        private DvContainer container;
        #endregion

        #region Event
        public event EventHandler Changed;
        #endregion

        #region Constructor
        public DvSubPageCollection(DvContainer container)
        {
            this.container = container;
        }
        #endregion

        #region Method
        #region Add
        private new void Add(string key, DvSubPage value)
        {
            if (value != null)
            {
                value.ParentContainer = container;

                base.Add(key, value);
                InvokeChanged();
            }
            else throw new Exception("페이지가 null 일 수 없습니다.");
        }

        public void Add(DvSubPage value)
        {
            if (value != null)
            {
                value.ParentContainer = container;

                if (value.Name != null)
                {
                    if (!ContainsKey(value.Name)) Add(value.Name, value);
                    else throw new Exception("동일한 이름의 페이지가 존재합니다.");
                }
                else throw new Exception("페이지 이름은 null 일 수 없습니다.");
            }
            else throw new Exception("페이지가 null 일 수 없습니다.");
        }
        #endregion
        #region Remove
        public void Remove(DvSubPage value)
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
        public bool Rename(DvSubPage target, string NewName)
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
                this[vk].ParentContainer = container;
        }
        #endregion

        #region InvokeChanged
        protected void InvokeChanged() => Changed?.Invoke(this, null);
        #endregion
        #endregion
    }
}
